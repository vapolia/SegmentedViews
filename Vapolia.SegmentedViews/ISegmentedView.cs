using System.Collections.ObjectModel;

namespace Vapolia.SegmentedViews;

public enum SegmentDisplayMode
{
    /// <summary>
    /// All segments will have the same width
    /// </summary>
    EqualWidth,
    /// <summary>
    /// Each segment will fit its content
    /// </summary>
    Content
}

public interface ISegmentedView : IView
{
    SegmentDisplayMode DisplayMode { get; set; }

    public Color TextColor { get; }
    public Color TintColor { get; }
    public Color SelectedTextColor { get; }
    public Color DisabledColor { get; }
    // public Color BorderColor { get; }
    // public double BorderWidth { get; }
    public double FontSize { get; }
    public string? FontFamily { get; }
    public Thickness ItemPadding { get; set; }

    public int SelectedIndex { get; }
    internal void SetSelectedIndex(int i);
    
    internal string? TextPropertyName { get; }
    internal IValueConverter? TextConverter { get; }
    internal  ObservableCollection<Segment> Children { get; }
}
