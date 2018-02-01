## Download unitypackage
[UnityGoogleDrive.unitypackage](https://github.com/Elringus/UnityGoogleDrive/releases/download/v0.2-alpha/UnityGoogleDrive.unitypackage)

## Description
[Google Drive](https://www.google.com/drive/) API library for listing, searching, creating, uploading, editing, copying, downloading, deleting and exporting files on the user's drive from within [Unity game engine](https://unity3d.com/).

Works with **Unity version 5.6 and higher**. Supports **all the major platforms** (including **WebGL**). Source code is **.NET 2.0 profile compatible** and portable (no platform-specific precompiled libraries are used).

Two main authentication schemes are used: browser redirection for WebGL builds (because of the sockets limitation) and local loopback requests for other platforms with refresh tokens support. All the credentials are stored in a scriptable object; editor script provides shortcuts to create and manage Google Console App, allows to parse credentials JSON to skip manual copy-pasting and edit common settings:

![Settings](https://i.gyazo.com/75fd0d64dd50485f208adfc56308ac20.png) 

Automated integration tests cover the main features:

![Tests](https://i.gyazo.com/81a59d10ce29ceabb4e23bb8ab5af6b1.png) 

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
The design mostly follows the official [Google APIs Client Library for .NET](https://github.com/google/google-api-dotnet-client):

```csharp
// Listing files.
GoogleDriveFiles.List().Send().OnDone += fileList => ...;

// Uploading a file.
var file = new UnityGoogleDrive.Data.File() { Name = "Image.png", Content = rawImageData };
GoogleDriveFiles.Create(file).Send();

// Downloading a file.
GoogleDriveFiles.Download(fileId).Send().OnDone += file => ...;
```

For more examples take a look at [test scripts](https://github.com/Elringus/UnityGoogleDrive/tree/master/Assets/Scripts).

## FAQ

### How to access a file using its path?
A folder in Google Drive is actually a file with the MIME type `application/vnd.google-apps.folder`. Hierarchy relationship is implemented via file's `Parents` property. To get the actual file using its path we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. Thus, we have to traverse the entire hierarchy chain using List requests. 

You can find a naive implementation of the aforementioned logic in [the example script](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/ExampleGetFileByPath.cs).

More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

### Can I access someone else's Google drive or skip authentication in the browser?
To work with anyone's Google Drive, it's mandatory to complete OAuth procedure for that user, which requires opening a browser window to login and allow the app to access their drive. It's a security measure [enforced by Google](https://developers.google.com/identity/protocols/OAuth2). 

### Can I share a drive account? 
The only legit way to allow multiple users share a drive account is to use [Team Drives](https://gsuite.google.com/learning-center/products/drive/get-started-team-drive/).

### Will this plugin appear on the Asset Store?
I'll consider publishing when (if) it'll be more in a more mature state (full API cover, more tests, less bugs); and whether that'll happen depends on the ~~amount of stars~~ feedback it'll receive :) In any case, the plugin will remain free and open-sourced.
