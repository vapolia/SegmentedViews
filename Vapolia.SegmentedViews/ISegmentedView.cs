using System.Collections.ObjectModel;

namespace Vapolia.SegmentedViews;

public interface ISegmentedView : IView, ITextStyle
{
    public Color SelectedBackgroundColor { get; }
    public Color SelectedTextColor { get; }
    public Color DisabledColor { get; }
    public Color BackgroundColor { get; }
    public Color BorderColor { get; }
    // public double BorderWidth { get; }
    public Thickness ItemPadding { get; set; }

    public int SelectedIndex { get; }
    internal void SetSelectedIndex(int i);
    public bool IsSelectionRequired { get; }
    
    internal string? TextPropertyName { get; }
    internal IValueConverter? TextConverter { get; }
    internal  ObservableCollection<Segment> Children { get; }
    internal WidthDefinitionCollection? WidthDefinitions { get; }
    internal GridLength ItemsDefaultWidth { get; }
}
