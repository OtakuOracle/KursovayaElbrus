using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Elbrus.Models;

namespace Elbrus;

public partial class AddClient : Window
{
    public AddClient()
    {
        InitializeComponent();
    }

    private void AddClient_OnClick(object? sender, RoutedEventArgs e)
    {
        using var context = new ElbrusRegionContext();

        if (string.IsNullOrWhiteSpace(FioBox.Text) ||
            string.IsNullOrWhiteSpace(CodeBox.Text) ||
            string.IsNullOrWhiteSpace(BirthdayBox.Text) ||
            string.IsNullOrWhiteSpace(AddressBox.Text) ||
            string.IsNullOrWhiteSpace(EmailBox.Text) ||
            string.IsNullOrWhiteSpace(PassportBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordBox.Text))
        {
            ShowMessage("Пожалуйста, заполните все поля!", false);
            return;
        }

        try
        {
            CorrectInput();

            var clientCode = Convert.ToInt32(CodeBox.Text);
            if (context.Clients.Any(c => c.ClientCode == clientCode))
            {
                ShowMessage("Клиент с таким кодом уже существует!", false);
                return;
            }

            var NewClient = new Client
            {
                Fio = FioBox.Text.Trim(),
                ClientCode = clientCode,
                Passport = PassportBox.Text.Trim(),
                Birthday = DateOnly.Parse(BirthdayBox.Text),
                Address = AddressBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Password = PasswordBox.Text,
                Role = 1
            };

            context.Clients.Add(NewClient);
            context.SaveChanges();

            ShowMessage("Клиент успешно добавлен!", true);
            ClearFields();
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка: {ex.Message}", false);
        }
    }

    private void ShowMessage(string text, bool isSuccess)
    {
        MessageBorder.IsVisible = true;
        MessageText.Text = text;
        MessageBorder.Background = isSuccess
            ? new SolidColorBrush(Color.FromRgb(220, 255, 220))
            : new SolidColorBrush(Color.FromRgb(255, 220, 220));
    }

    private void ClearFields()
    {
        FioBox.Text = "";
        CodeBox.Text = "";
        PassportBox.Text = "";
        BirthdayBox.Text = "";
        AddressBox.Text = "";
        EmailBox.Text = "";
        PasswordBox.Text = "";
        PhoneBox.Text = "";
        MessageBorder.IsVisible = false;
    }

    private void CorrectInput()
    {
        if (!int.TryParse(CodeBox.Text, out _) || CodeBox.Text.Length != 8)
        {
            throw new ArgumentException("Код клиента должен быть 8-значным числом");
        }

        if (PassportBox.Text.Length != 10 || !PassportBox.Text.All(char.IsDigit))
        {
            throw new ArgumentException("Паспорт должен содержать ровно 10 цифр");
        }

        if (!EmailBox.Text.Contains("@") || !EmailBox.Text.Contains("."))
        {
            throw new ArgumentException("Email должен содержать '@' и '.'");
        }

        if (PhoneBox.Text.Length != 11 || !PhoneBox.Text.All(char.IsDigit))
        {
            throw new ArgumentException("Номер телефона должен содержать 11 цифр");
        }

        if (!DateOnly.TryParse(BirthdayBox.Text, out _))
        {
            throw new ArgumentException("Некорректный формат даты рождения");
        }
    }

    private void BackOnOrder(object? sender, RoutedEventArgs e)
    {
        new SallerWindow().ShowDialog(this);
    }
}