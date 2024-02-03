using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SampleApp.ViewModels;

public record Person(int Id, string FirstName, string LastName);

/// <summary>
/// Sample value converter.
/// You can also override ToString() on the Person class instead of using this converter.
/// </summary>
public class PersonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var person = (Person)value!;
        return $"{person.FirstName} {person.LastName}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotSupportedException();
}

public class DynamicItemsPageViewModel : INotifyPropertyChanged
{
    private string infoText = string.Empty;
    private string infoText2 = string.Empty;
    private int nextInt = 42;

    public object? SegmentSelectedItem { get; set; }
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
    public ObservableCollection<Person> Persons { get; }

    public DynamicItemsPageViewModel()
    {
        Persons = new(new Person[]
        {
            new (1, "Johnny", "Halliday"),
            new (1, "Vanessa", "Paradis"),
            new (1, "Jose", "Garcia"),
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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
        => PropertyChanged?.Invoke(this, new(propertyName));
}