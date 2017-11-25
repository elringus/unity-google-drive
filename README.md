## Download unitypackage
[UnityGoogleDrive.unitypackage](https://github.com/Elringus/UnityGoogleDrive/releases/download/v0.1-alpha/UnityGoogleDrive.unitypackage)

## Description
SDK for [Google Drive](https://www.google.com/drive/) providing API for listing, searching, creating, uploading, editing, copying, downloading and deleting files on the User's drive from within [Unity game engine](https://unity3d.com/).

Works with Unity version 5.6 and higher. Supports all major target platforms (including WebGL).

Core runtime is **.NET 2.0 Subset** profile compatible; however full **.NET 2.0** profile is required for [Json.Net.Unity3D
](https://github.com/SaladLab/Json.Net.Unity3D) library to work correctly (used for API data serialization).

## Setup
- Import the package
- **GoogleDriveSettings**.asset will be automatically created in Assets/UnityGoogleDrive/Resources; select it
- Click **Create Google Drive API app** button; web-browser will open URL to setup the app
  - Select **Create a new project** and click continue
  - Click **Go to credentials**
  - Click **Cancel** 
  - Select **OAuth consent screen** tab and enter required info; click **Save**
  - Return to **Credentials** tab and click **Create credentials** -> **OAuth client ID**
  - Select **Web application** for 'Application type', give your app a name and enter the following restrictions:
    - Authorised JavaScript origins: enter host names wich will serve WebGL builds; *not required for platforms other than WebGL*
    - Authorised redirect URIs:
      - Add redirect URI for local loopback requests: **http://localhost**
      - Add full URIs to the WebGL builds locations; *not required for platforms other than WebGL*
    - Final result may [look like that](https://i.gyazo.com/34c05f3b5262c249b3f9b45d7daabd44.png) 
  - Click **Save**
  - Close the appeared popup and click [**Download JSON** button](https://i.gyazo.com/d6b620221f1326aada98b02e011b9094.png) to get the credentials json file
- Return to Unity editor, open the settings (you can also access them via Edit -> Project Settings -> Google Drive Settings) and click **Parse credentials JSON file**; select the downloaded credentials json file

## Examples

### Listing files
```csharp
GoogleDriveFiles.List().Send().OnDone += fileList => ...;
```

### Uploading file
```csharp
var file = new UnityGoogleDrive.Data.File() { Name = "Image.png", Content = rawImageData, MimeType = "image/png" };
GoogleDriveFiles.Create(file).Send();
```

### Downloading file
```csharp
GoogleDriveFiles.Download(fileId).Send().OnDone += file => ...;
```
For more examples take a look at [test scripts](https://github.com/Elringus/UnityGoogleDrive/tree/master/Assets/Scripts).

### Working with folders and file path
A folder in Google Drive is actually a file with the MIME type `application/vnd.google-apps.folder`. Hierarchy relationship is implemented via File's `Parents` property. To get the actual file using it's path we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. Thus, we need to traverse the entire hierarchy chain using List requests. 

You can find a naive implementation of the aforementioned logic in [the example script](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/ExampleGetFileByPath.cs).

More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.
