using SampleApp.ViewModels;

namespace SampleApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel(Navigation);
    }
}