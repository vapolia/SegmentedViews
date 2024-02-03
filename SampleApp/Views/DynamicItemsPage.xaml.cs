using SampleApp.ViewModels;

namespace SampleApp.Views;

public partial class DynamicItemsPage : ContentPage
{
    public DynamicItemsPage()
    {
        InitializeComponent();
        BindingContext = new DynamicItemsPageViewModel();
    }
}