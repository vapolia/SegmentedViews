# SegmentedViews

Platforms:
- Android
- iOS

Supports both static segments and `ItemsSource` to build segments dynamically.

# Quick start

Add the above nuget package to your Maui project   
then add this line to your maui app builder:

```c#
using Vapolia.SegmentedViews;
...
builder.UseSegmentedView();
```

# Examples

See the SampleApp in this repo.

Declare the namespace:
```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
         ...
         xmlns:segmented="https://vapolia.eu/Vapolia.SegmentedViews">
```

Add a static segment view:
```xaml
<segmented:SegmentedView  
    x:Name="TheSegmentView"
    SelectedIndex="0"
    SelectedTextColor="White" TextColor="Black" TintColor="Blue" DisabledColor="LightGray"
    SelectionChangedCommand="{Binding SegmentSelectionChangedCommand}"
    SelectedItem="{Binding SegmentSelectedItem}">
    
    <segmented:Segment Item="Item1" />
    <segmented:Segment Item="Item2" />
    <segmented:Segment Item="Item3" />
    <segmented:Segment Item="Item4" />
    <segmented:Segment Item="{Binding Item5Title}" />
    
</segmented:SegmentedView>
```

Or a dynamic segment view:
```xaml
        <segmented:SegmentedView
            ItemsSource="{Binding Persons}"
            TextPropertyName="LastName"
            SelectedIndex="0"
            SelectedItem="{Binding SegmentSelectedItem}"
            SelectedTextColor="White" TextColor="Black" TintColor="Blue" DisabledColor="LightGray"
            SelectionChangedCommand="{Binding SegmentSelectionChangedCommand}" />
```
