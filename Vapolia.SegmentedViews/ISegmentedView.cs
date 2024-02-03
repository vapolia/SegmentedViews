using System.Collections.ObjectModel;

namespace Vapolia.SegmentedViews;

public interface ISegmentedView : IView
{
    public Color TextColor { get; }
    public Color TintColor { get; }
    public Color SelectedTextColor { get; }
    public Color DisabledColor { get; }
    // public Color BorderColor { get; }
    // public double BorderWidth { get; }
    public double FontSize { get; }
    public string? FontFamily { get; }

    public int SelectedIndex { get; }
    internal void SetSelectedIndex(int i);
    
    internal string? TextPropertyName { get; }
    internal IValueConverter? TextConverter { get; }
    internal  ObservableCollection<Segment> Children { get; }
}
