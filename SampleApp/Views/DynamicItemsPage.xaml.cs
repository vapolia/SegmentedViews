using SampleApp.ViewModels;

namespace SampleApp.Views;

public partial class DynamicItemsPage : ContentPage
{
    public DynamicItemsPage()
    {
        BindingContext = new DynamicItemsPageViewModel(Navigation);
        InitializeComponent();
    }
}