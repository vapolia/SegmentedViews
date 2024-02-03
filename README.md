# PicturePicker

Platforms:
- Android (5 to 10)  
- iOS (11 to 14)  

Usage options:
- static methods on the AdvancedMediaPicker static class
- Interface IPicturePicker with a PicturePicker class on each platform

# Standard usage

```csharp
OpenPictureLibraryCommand = new Command(async () =>
{
    bool ok = false;
    var pictureCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var targetFile = Path.Combine(pictureCacheFolder, $"profilePic-{Guid.NewGuid()}.jpg");

    var hasPermission = await Permissions.CheckStatusAsync<Permissions.Photos>();
    if(hasPermission != PermissionStatus.Granted && hasPermission != PermissionStatus.Restricted)
        hasPermission = await Permissions.RequestAsync<Permissions.Photos>();
    if (hasPermission != PermissionStatus.Granted && hasPermission != PermissionStatus.Restricted)
    {
        if(await page.DisplayAlert("Denied", "You denied access to your photo library.", "Open Settings", "OK"))
            AppInfo.ShowSettingsUI();
    }
    else
        ok = await AdvancedMediaPicker.ChoosePictureFromLibrary(targetFile, maxPixelWidth: 500, maxPixelHeight: 500);

    if (ok)
        ImagePath = targetFile;
});
```

# Reference

Use AdvancedMediaPicker.[MethodName] where MethodName is the same as the one on the interface.

```csharp
    public interface IPicturePicker
    {
        Task<bool> ChoosePictureFromLibrary(string filePath, Action<Task<bool>>? saving = null, int maxPixelWidth=0, int maxPixelHeight=0, int percentQuality=80);

        /// <summary>
        /// Returns false if cancelled
        /// Note that saveToGallery can fails silently
        /// </summary>
        Task<bool> TakePicture(string filePath, Action<Task<bool>>? saving = null, int maxPixelWidth=0, int maxPixelHeight=0, int percentQuality=0, bool useFrontCamera=false, bool saveToGallery=false, CancellationToken cancel = default);

        bool HasCamera { get; }
    }
```
