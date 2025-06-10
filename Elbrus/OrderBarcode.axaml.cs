using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Avalonia.Interactivity;
using Path = System.IO.Path;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;
using System.Linq;
using System.Text;

namespace Elbrus;

public partial class OrderBarcode : Window
{
    private readonly Random _random = new();

    public OrderBarcode()
    {
        InitializeComponent();
    }

    public OrderBarcode(int orderId, int rentalDuration) : this()
    {
        string barcodeValue = GenerateBarcodeValue(orderId, rentalDuration);
        BarcodeValueTextBlock.Text = barcodeValue;
        RenderBarcodeImage(barcodeValue);
        CreateBarcodePdfDocument(barcodeValue, orderId);
        SaveOrderTrackingLink(orderId);
    }

    private string GenerateBarcodeValue(int orderId, int rentalDuration)
    {
        string timestamp = DateTime.Now.ToString("yyMMddHHmm");
        string randomDigits = string.Concat(Enumerable.Range(0, 6).Select(_ => _random.Next(10)));
        return $"{orderId}{timestamp}{rentalDuration}{randomDigits}";
    }

    private void RenderBarcodeImage(string barcodeData)
    {
        const double mmToPx = 3.78;
        double xPosition = 3.63 * mmToPx;
        double barHeight = 22.85 * mmToPx;
        double extendedBarHeight = barHeight + (1.65 * mmToPx);

        BarcodeImageCanvas.Children.Clear();

        for (int i = 0; i < barcodeData.Length; i++)
        {
            if (!char.IsDigit(barcodeData[i])) continue;

            var digit = int.Parse(barcodeData[i].ToString());
            double barWidth = digit == 0 ? 1.35 * mmToPx : 0.15 * digit * mmToPx;

            if (digit > 0)
            {
                bool isBoundaryBar = i == 0 || i == barcodeData.Length - 1 || i == barcodeData.Length / 2;

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = isBoundaryBar ? extendedBarHeight : barHeight,
                    Fill = Brushes.Black,
                };

                Canvas.SetLeft(bar, xPosition);
                Canvas.SetTop(bar, isBoundaryBar ? 0 : (extendedBarHeight - barHeight));
                BarcodeImageCanvas.Children.Add(bar);
            }

            xPosition += barWidth + (0.2 * mmToPx);
        }
    }

    private void CreateBarcodePdfDocument(string barcodeData, int orderId)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfFilePath = Path.Combine(documentsPath, $"OrderBarcode_{orderId}.pdf");

        using var document = new Document(new iTextSharp.text.Rectangle(100f, 50f));
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));
        document.Open();

        var barcode = new Barcode128
        {
            CodeType = Barcode128.CODE128,
            Code = barcodeData,
            BarHeight = 22.85f,
        };

        var image = barcode.CreateImageWithBarcode(writer.DirectContent, null, null);
        document.Add(image);
    }

    private void SaveOrderTrackingLink(int orderId)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        string orderDetails = $"order_date={timestamp}&order_number={orderId}";
        string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(orderDetails));
        string trackingLink = $"https://wsrussia.ru/?data={base64Data}";

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string textFilePath = Path.Combine(documentsPath, $"OrderTrackingLink_{orderId}.txt");

        File.WriteAllText(textFilePath, trackingLink);
    }

    private void OnPrintButtonClick(object sender, RoutedEventArgs e) => Close();
    private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();
}