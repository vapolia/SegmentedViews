using SampleApp.Views;

namespace SampleApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = PlatformAppTheme;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()));
    }
}