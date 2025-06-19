using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Interactivity;
using Elbrus.Models;

namespace Elbrus;

public partial class History : Window
{
    private ObservableCollection<Employee> employees = new();
    public List<Employee> AllEmployees = new();

    public int SuccessfulLoginsCount => employees.Count(e => IsSuccessStatus(e.EnterStatus));
    public int FailedLoginsCount => employees.Count(e => IsFailStatus(e.EnterStatus));

    public History()
    {
        InitializeComponent();
        LoadData();
        DataContext = this;


        LoginComboBox.SelectionChanged += ComboBox_SelectionChanged;
        SortComboBox.SelectionChanged += ComboBox_SelectionChanged;
        StatusComboBox.SelectionChanged += ComboBox_SelectionChanged;
        ResetButton.Click += ResetButton_Click;
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private bool IsSuccessStatus(string? status)
    {
        return status != null && status.Contains("Успешно");
    }

    private bool IsFailStatus(string? status)
    {
        return status != null && status.Contains("Неуспешно");
    }

    private void LoadData()
    {
        using var context = new ElbrusRegionContext();
        AllEmployees = context.Employees
            .OrderByDescending(e => e.LastEnter)
            .Select(e => new Employee
            {
                Id = e.Id,
                Login = e.Login,
                LastEnter = e.LastEnter,
                EnterStatus = e.EnterStatus ?? "Неизвестно"
            }).ToList();

        employees = new ObservableCollection<Employee>(AllEmployees);
        LastEnterBox.ItemsSource = employees;

        LoginComboBox.ItemsSource = new List<string> { "Все пользователи" }
            .Concat(AllEmployees.Select(e => e.Login).Distinct().OrderBy(l => l));
        LoginComboBox.SelectedIndex = 0;
    }

    private void ApplyFilters()
    {
        var filtered = AllEmployees.AsEnumerable();

        if (LoginComboBox.SelectedItem is string selectedLogin && selectedLogin != "Все пользователи")
        {
            filtered = filtered.Where(e => e.Login == selectedLogin);
        }

        if (StatusComboBox.SelectedIndex == 1)
        {
            filtered = filtered.Where(e => IsSuccessStatus(e.EnterStatus));
        }
        else if (StatusComboBox.SelectedIndex == 2)
        {
            filtered = filtered.Where(e => IsFailStatus(e.EnterStatus));
        }

        filtered = SortComboBox.SelectedIndex == 0
            ? filtered.OrderByDescending(e => e.LastEnter)
            : filtered.OrderBy(e => e.LastEnter);

        employees.Clear();
        foreach (var emp in filtered)
        {
            employees.Add(emp);
        }
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        LoginComboBox.SelectedIndex = 0;
        SortComboBox.SelectedIndex = 0;
        StatusComboBox.SelectedIndex = 0;
        ApplyFilters();
    }
}

public class StatusToColorConverter : IValueConverter
{
    public static StatusToColorConverter Instance { get; } = new StatusToColorConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string status) return Brushes.Black;
        return status.Contains("Успешно") ? Brushes.Green : Brushes.Red;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}