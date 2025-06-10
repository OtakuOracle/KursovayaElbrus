using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Elbrus.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using ReactiveUI;
using System.ComponentModel;
using System.IO;

namespace Elbrus;

public partial class SallerWindow : Window, INotifyPropertyChanged, IReactiveObject, INotifyPropertyChanging
{
    private readonly ElbrusRegionContext _context = new();
    public ObservableCollection<Client> ClientList { get; } = new();
    public ObservableCollection<Service> ServiceList { get; } = new();
    public ObservableCollection<ServiceWithTime> BasketServices { get; } = new();

    public class ServiceWithTime : Service
    {
        public int TimeInMinutes { get; set; } = 30;
        public string CurrentStatus { get; set; } = "Новая услуга";
    }

    private Client _chosenClient;
    public Client ChosenClient
    {
        get => _chosenClient;
        set => this.RaiseAndSetIfChanged(ref _chosenClient, value);
    }

    private Service _chosenService;
    public Service ChosenService
    {
        get => _chosenService;
        set => this.RaiseAndSetIfChanged(ref _chosenService, value);
    }

    private int _minutesSelected = 30;
    public int MinutesSelected
    {
        get => _minutesSelected;
        set => this.RaiseAndSetIfChanged(ref _minutesSelected, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event PropertyChangingEventHandler PropertyChanging;

    void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

    public SallerWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadInitialData();
        OrderNumberField.Text = "Генерация при оформлении";
    }

    private async void LoadInitialData()
    {
        try
        {
            await _context.Clients.LoadAsync();
            await _context.Services.LoadAsync();

            ClientList.Clear();
            ServiceList.Clear();

            foreach (var cl in _context.Clients.Local.ToList())
                ClientList.Add(cl);

            foreach (var svc in _context.Services.Local.ToList())
                ServiceList.Add(svc);
        }
        catch (Exception ex)
        {
            InfoText.Text = $"Ошибка при загрузке: {ex.Message}";
        }
    }

    private async void AddClientClick(object sender, RoutedEventArgs e)
    {
        var addClientWindow = new AddClient();
        var resultClient = await addClientWindow.ShowDialog<Client>(this);

        if (resultClient != null)
        {
            try
            {
                _context.Clients.Add(resultClient);
                await _context.SaveChangesAsync();
                ClientList.Add(resultClient);
                ChosenClient = resultClient;
            }
            catch (Exception ex)
            {
                InfoText.Text = $"Ошибка добавления клиента: {ex.Message}";
            }
        }
    }

    private void AddServiceClick(object sender, RoutedEventArgs e)
    {
        if (ChosenService != null)
        {
            if (BasketServices.All(s => s.ServiceId != ChosenService.ServiceId))
            {
                BasketServices.Add(new ServiceWithTime
                {
                    ServiceId = ChosenService.ServiceId,
                    ServiceName = ChosenService.ServiceName,
                    CostPerHour = ChosenService.CostPerHour,
                    TimeInMinutes = MinutesSelected,
                    CurrentStatus = "Новая услуга"
                });
            }
            else
            {
                InfoText.Text = "Такая услуга уже выбрана.";
            }
        }
    }

    private void RemoveServiceClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ServiceWithTime svc)
            BasketServices.Remove(svc);
    }

    private async void CompleteOrderClick(object sender, RoutedEventArgs e)
    {
        if (ChosenClient == null)
        {
            InfoText.Text = "Не выбран клиент.";
            return;
        }

        if (BasketServices.Count == 0)
        {
            InfoText.Text = "Не выбраны услуги.";
            return;
        }

        try
        {
            string generatedOrderNum = $"{new Random().Next(100000, 999999)}/{DateTime.Now:yyyyMMdd}";
            var newOrder = new Models.Order
            {
                ClientId = ChosenClient.ClientId,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Status = "Оформлен",
                OrderCode = generatedOrderNum
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            foreach (var svc in BasketServices)
            {
                _context.OrderServices.Add(new OrderService
                {
                    OrderId = newOrder.OrderId,
                    ServiceId = svc.ServiceId,
                });
            }

            await _context.SaveChangesAsync();

            CreateReceipt(newOrder);

            int maxTime = BasketServices.Max(x => x.TimeInMinutes);
            new OrderBarcode(newOrder.OrderId, maxTime).Show();
            Close();
        }
        catch (Exception ex)
        {
            InfoText.Text = $"Ошибка при оформлении: {ex.Message}";
        }
    }

    private void CreateReceipt(Models.Order order)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string outputFile = Path.Combine(documentsPath, $"OrderReceipt_{order.OrderCode.Replace("/", "_")}.pdf");

        using (var doc = new Document(PageSize.A4, 40, 40, 40, 40))
        {
            PdfWriter.GetInstance(doc, new FileStream(outputFile, FileMode.Create));
            doc.Open();

            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
            var titleFont = new Font(baseFont, 18, Font.BOLD);
            var textFont = new Font(baseFont, 12);

            var title = new Paragraph("Receipt of Payment", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            doc.Add(title);

            PdfPTable details = new(2) { WidthPercentage = 100 };
            AddRow(details, "Order Number:", order.OrderCode, textFont);
            AddRow(details, "Order Date:", $"{order.StartDate:dd.MM.yyyy} {order.Time:hh\\:mm}", textFont);
            AddRow(details, "Client Name:", ChosenClient?.Fio ?? "Unknown", textFont);
            AddRow(details, "Order Status:", order.Status, textFont);
            doc.Add(details);

            doc.Add(new Paragraph(" "));

            PdfPTable servicesTable = new(3) { WidthPercentage = 100 };
            foreach (var header in new[] { "Service", "Hourly Rate", "Minutes" })
                servicesTable.AddCell(new PdfPCell(new Phrase(header, textFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, Padding = 5 });

            foreach (var item in BasketServices)
            {
                servicesTable.AddCell(new Phrase(item.ServiceName, textFont));
                servicesTable.AddCell(new Phrase($"{item.CostPerHour:N2} ₽", textFont));
                servicesTable.AddCell(new Phrase($"{item.TimeInMinutes}", textFont));
            }
            doc.Add(servicesTable);

            decimal totalSum = BasketServices.Sum(s => (s.CostPerHour ?? 0) * (s.TimeInMinutes / 60m));
            var total = new Paragraph($"Total: {totalSum:N2} ₽", textFont) { Alignment = Element.ALIGN_RIGHT, SpacingBefore = 20 };
            doc.Add(total);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = outputFile,
            UseShellExecute = true
        });
    }

    private void AddRow(PdfPTable table, string label, string value, Font font)
    {
        table.AddCell(new Phrase(label, font));
        table.AddCell(new Phrase(value, font));
    }
}
