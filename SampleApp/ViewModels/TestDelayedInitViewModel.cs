using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SampleApp.ViewModels;

public class TestDelayedInitViewModel : INotifyPropertyChanged
{
    private string infoText = string.Empty;
    private string infoText2 = string.Empty;
    private object? segmentSelectedItem;
    private int nextInt = 42;
    private List<Person> persons;

    public object? SegmentSelectedItem
    {
        get => segmentSelectedItem;
        set { segmentSelectedItem = value; OnPropertyChanged(); }
    }

    public ICommand SegmentSelectionChangedCommand { get; }
    
    public string InfoText
    {
        get => infoText;
        set { infoText = value; OnPropertyChanged(); }
    }
    public string InfoText2
    {
        get => infoText2;
        set { infoText2 = value; OnPropertyChanged(); }
    }

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand ClearCommand { get; }

    public List<Person> Persons
    {
        get => persons;
        private set
        {
            persons = value;
            OnPropertyChanged();
        }
    }

    public TestDelayedInitViewModel()
    {
        //Testing:
        //set selected item before the items are set
        
        var thePersons = new List<Person>
        {
            new (1, "Johnny", "Halliday"),
            new (2, "Vanessa", "Paradis"),
            new (3, "Jose", "Garcia"),
        };
        
        SegmentSelectedItem = thePersons[1];

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            Persons = thePersons;

            if ((Person?)SegmentSelectedItem != thePersons[1])
            {
                Console.WriteLine("Issue with SegmentedView 🤔");
            }
        });
        
        SegmentSelectionChangedCommand = new Command(() =>
        {
            InfoText = $"Selected item: {SegmentSelectedItem ?? "-"}";
        });

        AddItemCommand = new Command(() =>
        {
            Persons.Add(new(999, "Any", $"One {nextInt++}"));
        });

        RemoveItemCommand = new Command(() =>
        {
            if(Persons.Any())
                Persons.RemoveAt(Persons.Count-1);
        });

        ClearCommand = new Command(() =>
        {
            Persons.Clear();
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
        => PropertyChanged?.Invoke(this, new(propertyName));
}