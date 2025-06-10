using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Elbrus.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace Elbrus;

public partial class OrderDetailsWindow : Window
{
    private readonly Models.Order _order;
    private readonly ElbrusRegionContext _db = new();

    public OrderDetailsWindow(Models.Order order)
    {
        InitializeComponent();
        _order = order;
        LoadOrderData();
    }

    private void LoadOrderData()
    {
        var client = _db.Clients.FirstOrDefault(c => c.ClientId == _order.ClientId);
        var orderServices = _db.OrderServices
            .Include(os => os.Service)
            .Where(os => os.OrderId == _order.OrderId)
            .ToList();

        OrderNumber.Text = _order.OrderId.ToString();
        ClientName.Text = client?.Fio ?? "Не указан";
        OrderDate.Text = $"{_order.StartDate:dd.MM.yyyy} {_order.Time}";
        StatusText.Text = _order.Status ?? "Новая";

        var (totalCost, servicesText) = CalculateServicesInfo(orderServices);
        TotalCost.Text = totalCost.ToString("C2");

        // Заполняем список услуг
        ServicesList.ItemsSource = orderServices.Select(os => new
        {
            ServiceName = os.Service?.ServiceName ?? "Неизвестная услуга",
            Duration = $"{os.RentTime} мин",
            Cost = ((os.Service?.CostPerHour ?? 0) * (os.RentTime / 60m)).ToString("C2")
        });
    }

    private (decimal totalCost, string servicesText) CalculateServicesInfo(List<OrderService> orderServices)
    {
        decimal total = 0;
        string info = "";

        foreach (var os in orderServices)
        {
            if (os.Service != null)
            {
                decimal cost = (os.Service.CostPerHour ?? 0) * (os.RentTime / 60m);
                total += cost;
                info += $"{os.Service.ServiceName} - {os.RentTime} мин ({cost:C2})\n";
            }
        }

        return (total, info);
    }

    private void PrintBarcode_Click(object sender, RoutedEventArgs e)
    {
        int rentTime = _db.OrderServices
            .Where(os => os.OrderId == _order.OrderId)
            .Max(os => os.RentTime);

        new OrderBarcode(_order.OrderId, rentTime).Show();
        Close();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}