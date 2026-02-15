using local_translate_provider.Models;
using local_translate_provider.Services;
using local_translate_provider.TrayIcon;
using Microsoft.UI.Xaml;

namespace local_translate_provider;

public partial class App : Application
{
    private MainWindow? _window;
    private TrayIconManager? _trayIcon;
    private AppSettings _settings = new();
    private TranslationService? _translationService;
    private HttpTranslationServer? _httpServer;

    public static AppSettings Settings => (Current as App)!._settings;
    public static TranslationService TranslationService => (Current as App)!._translationService!;
    public static HttpTranslationServer HttpServer => (Current as App)!._httpServer!;

    public App()
    {
        InitializeComponent();
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _settings = await SettingsService.LoadAsync();
        _translationService = new TranslationService(_settings);
        _httpServer = new HttpTranslationServer(_settings, _translationService);
        _httpServer.Start();

        _trayIcon = new TrayIconManager(ShowSettings, DoExit);

        if (_settings.MinimizeToTrayOnStartup)
        {
            // 延迟创建 MainWindow，首次点击托盘或 ShowSettings 时再创建
        }
        else
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }

    private void ShowSettings()
    {
        if (_window == null)
        {
            _window = new MainWindow();
        }
        _window.AppWindow.Show();
        _window.Activate();
    }

    private void DoExit()
    {
        _httpServer?.Stop();
        _trayIcon?.Dispose();
        Microsoft.UI.Xaml.Application.Current.Exit();
    }
}
