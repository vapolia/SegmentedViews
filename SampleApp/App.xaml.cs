using SampleApp.Views;

namespace SampleApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage());
        UserAppTheme = PlatformAppTheme;
    }
}