using ElysiaFramework;
using ElysiaFramework.Interfaces;
using ElysiaFramework.Services;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using StudentManager.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace StudentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : AppEx
    {
        private static Mutex? Mutex;

        public static string AppVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        protected override void OnStartup(StartupEventArgs e)
        {
            //AppContext.SetSwitch(@"Switch.System.Windows.Controls.DoNotAugmentWordBreakingUsingSpeller", true);
            Mutex = new Mutex(true, "StudentManager.Lock", out var createNew);
            if (!createNew)
            {
                MessageBox.Show("应用已经在运行中，请勿重复启动第二个实例。");
                Environment.Exit(0);

            }

            var Host = Microsoft.Extensions.Hosting.Host.
                CreateDefaultBuilder().
                UseContentRoot(AppContext.BaseDirectory).
                ConfigureServices((context, services) =>
                {
                    //services.AddDbContext<AppDbContext>();
                    services.AddSingleton<IThemeService, ThemeService>();
                    //services.AddSingleton<ProfileService>();
                    //services.AddSingleton<SettingsService>();
                    services.AddSingleton<SettingsWindow>();
                    //services.AddSingleton<WallpaperPickingService>();
                    //services.AddHostedService<ThemeBackgroundService>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<CrashWindow>();
                    //services.AddSingleton<WindowFocusObserverService>();
                }).
                Build();
            _ = Host.StartAsync();
            //GetService<AppDbContext>();
            var MainWindow = GetService<MainWindow>();
            GetService<MainWindow>().Show();
            base.OnStartup(e);
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var cw = GetService<CrashWindow>();
            cw.CrashInfo = e.Exception.ToString();
            cw.Exception = e.Exception;
            cw.OpenWindow();
        }

        public static void ReleaseLock()
        {
            Mutex?.ReleaseMutex();
        }
    }

}
