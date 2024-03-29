# Vapolia.SegmentedViews

[![NuGet][nuget-img]][nuget-link]  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.SegmentedViews)  
[![Publish To Nuget](https://github.com/vapolia/SegmentedViews/actions/workflows/main.yaml/badge.svg)](https://github.com/vapolia/SegmentedViews/actions/workflows/main.yaml)

```cs
dotnet add package Vapolia.SegmentedViews

builder.UseSegmentedView();
```

[nuget-link]: https://www.nuget.org/packages/Vapolia.SegmentedViews/
[nuget-img]: https://img.shields.io/nuget/v/Vapolia.SegmentedViews

![image](https://github.com/vapolia/SegmentedViews/assets/190756/27ec1108-0e5d-47a0-a85c-c592694a9b52)
![image](https://github.com/vapolia/SegmentedViews/assets/190756/3d0e359b-92aa-49a4-9e91-26883b2e6e1f)


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
    SelectedTextColor="White" TextColor="Black" SelectedBackgroundColor="Blue" DisabledColor="LightGray"
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
            SelectedTextColor="White" TextColor="Black" SelectedBackgroundColor="Blue" DisabledColor="LightGray"
            SelectionChangedCommand="{Binding SegmentSelectionChangedCommand}" />
```

## Width of segment items

The width of a segment can be set in the following 3 ways, in reverse order of priority:

* On the `ItemsDefaultWidth` property of `SegmentedView`
```xml
<segmented:SegmentedView  
    x:Name="TheSegmentView"
    SelectedIndex="0"
    SelectedTextColor="White" TextColor="Black" SelectedBackgroundColor="Blue" DisabledColor="LightGray"
    SelectionChangedCommand="{Binding SegmentSelectionChangedCommand}"
    SelectedItem="{Binding SegmentSelectedItem}"
    ItemsDefaultWidth="150" />
```

* On the `ItemsWidthDefinitions` property of `SegmentedView`
```xml
<segmented:SegmentedView  
    x:Name="TheSegmentView"
    SelectedIndex="0"
    SelectedTextColor="White" TextColor="Black" SelectedBackgroundColor="Blue" DisabledColor="LightGray"
    SelectionChangedCommand="{Binding SegmentSelectionChangedCommand}"
    SelectedItem="{Binding SegmentSelectedItem}"
    ItemsWidthDefinitions="150,Auto,*,2*">
```
This width follow the format of a Grid's ColumnsDefinition, so it should be straightforward to use.

* Directly on the `Width` property of a `Segment`
```xml
<segmented:Segment Item="Item1" Width="150" />
<segmented:Segment Item="Item1" Width="Auto" />
```
