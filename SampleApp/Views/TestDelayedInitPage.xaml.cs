using SampleApp.ViewModels;

namespace SampleApp.Views;

public partial class TestDelayedInitPage
{
    public TestDelayedInitPage()
    {
        InitializeComponent();
        BindingContext = new TestDelayedInitViewModel();
    }
}