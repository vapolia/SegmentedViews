# SegmentedViews

[![NuGet][nuget-img]][nuget-link]  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.SegmentedViews)

[![Publish To Nuget](https://github.com/vapolia/SegmentedViews/actions/workflows/main.yaml/badge.svg)](https://github.com/vapolia/SegmentedViews/actions/workflows/main.yaml)

[nuget-link]: https://www.nuget.org/packages/Vapolia.SegmentedViews/
[nuget-img]: https://img.shields.io/nuget/v/Vapolia.SegmentedViews


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

![image](https://github.com/vapolia/SegmentedViews/assets/190756/0bd93272-739e-4bbe-85b6-dc407b1cab13)


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
