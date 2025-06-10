using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Elbrus.Models;
using Org.BouncyCastle.Utilities.Collections;

namespace Elbrus
{
    public partial class UserDashboardWindow : Window
    {
        private readonly TimeSpan _sessionDuration = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _warningTime = TimeSpan.FromMinutes(5);
        private DateTime _sessionStartTime;
        private bool _warningShown;
        private readonly Employee _user;


        public UserDashboardWindow()
        {
            InitializeComponent();
        }

        public UserDashboardWindow(Employee user) : this()
        {
            _user = user;
            InitializeUserData();
            _sessionStartTime = DateTime.Now;
            _ = StartSessionTimerAsync();
        }

        private void InitializeUserData()
        {
            FioTextBlock.Text = _user.Fio;
            RoleTextBlock.Text = _user.Role == 3 ? "Администратор" : "Сотрудник";

            CreateOrderButton.IsVisible = _user.Role != 3;
            HistoryButton.IsVisible = _user.Role == 3;

            LoadUserImage();
        }

        private void LoadUserImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_user.Photo))
                {
                    var path = Path.Combine(AppContext.BaseDirectory, _user.Photo);
                    if (File.Exists(path))
                    {
                        UserImage.Source = new Bitmap(path);
                        return;
                    }
                }
                UserImage.Source = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                UserImage.Source = null;
            }
        }

        private async Task StartSessionTimerAsync()
        {
            while (true)
            {
                var elapsed = DateTime.Now - _sessionStartTime;
                var remaining = _sessionDuration - elapsed;

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SessionTimerText.Text = remaining > TimeSpan.Zero
                        ? $"Осталось: {remaining:mm\\:ss}"
                        : "Время вышло";

                    if (!_warningShown && remaining <= _warningTime && remaining > TimeSpan.Zero)
                    {
                        _warningShown = true;
                        SessionWarningText.Text = "Внимание: до окончания сеанса ≤5 мин!";
                    }
                });

                if (remaining <= TimeSpan.Zero)
                {
                    await Dispatcher.UIThread.InvokeAsync(CloseAndReturnToMain);
                    break;
                }

                await Task.Delay(1000);
            }
        }

        private void CloseAndReturnToMain()
        {
            new MainWindow().Show();
            Close();
        }

        private void OnCreateOrderClick(object? sender, RoutedEventArgs e)
        {
            new SallerWindow().ShowDialog(this);
        }

        private void OnViewHistoryClick(object? sender, RoutedEventArgs e)
        {
            new History().ShowDialog(this);
        }

        private void OnLogoutClick(object? sender, RoutedEventArgs e)
        {
            CloseAndReturnToMain();
        }
    }
}