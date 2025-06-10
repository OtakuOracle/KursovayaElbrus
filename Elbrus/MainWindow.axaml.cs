using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Elbrus.Models;
using Tmds.DBus.Protocol;

namespace Elbrus;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }



    private void TogglePasswordVisibility(object? sender, RoutedEventArgs e)
    {
        PasswordBox.PasswordChar = PasswordBox.PasswordChar == '*' ? '\0' : '*';
    }

    private void AuthorizeButton(object? sender, RoutedEventArgs e)
    {
        using var context = new ElbrusRegionContext();
        var user = context.Employees.FirstOrDefault(it => it.Login == LoginBox.Text && it.Password == PasswordBox.Text);

        if (user != null)
        {
            var functionWindow = new UserDashboardWindow(user);
            {
                DataContext = user;
            }
            ;
            functionWindow.ShowDialog(this);
        }
        else
        {
            ErrorBlock.Text = "Неверный пароль";
        }
    }
}