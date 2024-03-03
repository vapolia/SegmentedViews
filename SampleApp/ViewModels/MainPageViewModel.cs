using System.Windows.Input;
using SampleApp.Views;

namespace SampleApp.ViewModels;

public class MainPageViewModel
{
    public int SegmentSelectedIndex { get; set; }
    public ICommand SegmentSelectionChangedCommand { get; }
    public ICommand GoAdvancedDemoPageCommand { get; }

    public MainPageViewModel(INavigation navigation)
    {
        SegmentSelectionChangedCommand = new Command(() =>
        {
            var selectedItem = SegmentSelectedIndex;
            //...
        });

        GoAdvancedDemoPageCommand = new Command(() =>
        {
            navigation.PushAsync(new DynamicItemsPage());
        });
    }
}
