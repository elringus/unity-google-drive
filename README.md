## Download package
For Unity 2017.4 and later: [UnityGoogleDrive.unitypackage](https://github.com/Elringus/UnityGoogleDrive/releases/download/v0.16-alpha/UnityGoogleDrive.unitypackage). Check [releases](https://github.com/Elringus/UnityGoogleDrive/releases) for previous versions support.

**In case you're not familiar with the Google Drive API, please read through the [official documentation](https://developers.google.com/drive/api/v3/about-sdk) and [FAQ](https://github.com/Elringus/UnityGoogleDrive#faq), before using this package or opening new issues.**

## Description
[Google Drive](https://www.google.com/drive/) API library for listing, searching, creating, uploading, editing, copying, downloading, deleting and exporting files on the user's drive from within [Unity game engine](https://unity3d.com/).

Supports **all the major platforms** (including **WebGL**). Source code is **.NET 2.0 profile compatible** and portable (no platform-specific precompiled libraries are used).

Two main authentication schemes are used: browser redirection for WebGL builds (because of the sockets limitation) and local loopback requests for other platforms with refresh tokens support. All the credentials are stored in a scriptable object; editor script provides shortcuts to create and manage Google Console App, allows to parse credentials JSON to skip manual copy-pasting and edit common settings:

![Settings](https://i.gyazo.com/50c58d42173658a504e0ea19ef522a2f.png) 

Automated integration tests cover the main features:

![Tests](https://i.gyazo.com/128acac61f5c719376b0f32f70144168.png) 

## Setup
- Import the package;
- In the Unity editor navigate to `Edit -> Project Settings -> Google Drive Settings`; **GoogleDriveSettings.asset** file will be automatically created at `Assets/UnityGoogleDrive/Resources`, select the file (if it wasn't selected automatically);
- Click **Create Google Drive API app** button; web-browser will open URL to setup the app:
  - Select **Create a new project** and click continue;
  - Click **Go to credentials**;
  - Click **Cancel**;
  - Select **OAuth consent screen** tab and enter required info, click **Save**;
  - Return to **Credentials** tab and click **Create credentials** -> **OAuth client ID**;
  - Select **Web application** for 'Application type', give your app a name and enter the following restrictions:
    - Authorised JavaScript origins: enter host names wich will serve WebGL builds *(not required for platforms other than WebGL)*;
    - Authorised redirect URIs:
      - Add redirect URI for the local loopback requests: **http://localhost**;
      - Add full URIs to the WebGL builds locations *(not required for platforms other than WebGL)*.
    - Final result may [look like that](https://i.gyazo.com/9d28c9b1e0201cb92ed6d8f3fc6dcfaf.png).
  - Click **Save**;
  - Close the appeared popup and click [**Download JSON** button](https://i.gyazo.com/d6b620221f1326aada98b02e011b9094.png) to get the credentials json file.
- Return to Unity editor, open Google Drive settings and click **Parse credentials JSON file**; select the downloaded credentials json file.

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

// All the requests are compatible with the .NET 4 asynchronous model.
var aboutData = await GoogleDriveAbout.Get().Send();
```

For more examples take a look at [test scripts](https://github.com/Elringus/UnityGoogleDrive/tree/master/Assets/Scripts).

## FAQ

### Why the returned properties of the response are all null?
Most of the response properties are null by default. You have to explicitly require fields in order for the drive API to return them (using `Fields` property of the request object). More info here: https://developers.google.com/drive/v3/web/performance#partial.

### How to access a file using its path?
A folder in Google Drive is actually a file with the MIME type `application/vnd.google-apps.folder`. Hierarchy relationship is implemented via file's `Parents` property. To get the actual file using its path we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. Thus, we have to traverse the entire hierarchy chain using List requests. 

You can find a naive implementation of the aforementioned logic in [the example script](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/ExampleGetFileByPath.cs) or use [built-in async helpers](https://github.com/Elringus/UnityGoogleDrive/blob/12e06a0851691c8cb3cadc266c1caf12a6261bc3/Assets/UnityGoogleDrive/Runtime/Utilities/Helpers.cs#L43) to find files via path (requires .NET 4.x).

More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

### Is it possible to download/upload large files in chunks to reduce memory usage?
To perform a partial download you have to supply `downloadRange` agrument for the `GoogleDriveFiles.Download` request specifying the bytes range you wish to get. Here is an [example script for a partial text file download](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/TestFilesDownloadRange.cs). More info on partial downloads can be found in the [API docs](https://developers.google.com/drive/api/v3/manage-downloads#partial_download).

For the chunked uploads you'll have to use resumable upload requests in a special manner. First, create a resumable upload request (via either `GoogleDriveFiles.CreateResumable` or `GoogleDriveFiles.UpdateResumable`), supply the file's metadata, but don't set the file's content. Send the request and get a resumable session URI from the response. Now you can use the session URI to upload the file's content in chunks. See the [Drive API docs](https://developers.google.com/drive/api/v3/resumable-upload#upload-resumable) for detailed instructions on how to perform a chunked upload using a resumable session URI.

### Is there a way to automatically redirect user to the app when authorization in browser is complete? 
When user finishes authorization flow on mobile and standalone platforms, an HTML string is injected to the active browser window. The default content of the HTML contains a message, asking user to return to the app. You can modify the content of the injected HTML in the Google Drive Settings asset using ‘Loopback Response HTML’ field. It’s possible to inject a javascript code to this HTML, which will be invoked right after the auth flow is completed. You can use this option to automatically redirect user back to your app using a custom URI scheme. Specific implementation will depend on the platform: for android you’ll have to [add an intent filter to the manifest](https://stackoverflow.com/questions/2958701/launch-custom-android-application-from-android-browser), for iOS use [universal links feature](https://developer.apple.com/library/content/documentation/General/Conceptual/AppSearch/UniversalLinks.html) and for Windows [bind the application to a URI scheme](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85)).

### How to logout or force user to login/select another Google account?
Use `GoogleDriveSettings.DeleteCachedAuthTokens()` method to clear cached authentication tokens which will force the user to login again on the next request. You can always get `GoogleDriveSettings` instance using `GoogleDriveSettings.LoadFromResources()` static method. While in editor, you can also use 'Delete cached tokens' button for the same effect.

### Can I access someone else's Google drive or skip authentication in the browser?
To work with anyone's Google Drive, it's mandatory to complete OAuth procedure for that user, which requires opening a browser window to login and allow the app to access their drive. It's a security measure [enforced by Google](https://developers.google.com/identity/protocols/OAuth2). 

### Is it possible to use an embedded browser (WebView) for the authorization?
It’s not possible. Google is intentionally [blocking authorization requests sent from any sort of embedded browsers](https://auth0.com/blog/google-blocks-oauth-requests-from-embedded-browsers/) for security reasons.

### Can I share a drive account? 
The only legit way to allow multiple users share a drive account is to use [Team Drives](https://gsuite.google.com/learning-center/products/drive/get-started-team-drive/).

### Is it possible to access shared files and folders?
This is possible. To access shared resources you'll have to specify ["Shared with me" collection](https://developers.google.com/drive/v3/web/about-organization#shared_with_me) when resolving ID of the resource. Additionally, if the shared resource has been [added to the user's drive](https://support.google.com/drive/answer/2375057?co=GENIE.Platform%3DDesktop&hl=en) it'll be accessible via the path finding method described above.

### Issues with authentication on iOS Devices.
In case you're having issues when authenticating on iOS, consider switching to the [.NET 4.x scripting runtime]( https://docs.unity3d.com/Manual/ScriptingRuntimeUpgrade.html). When under .NET 4.x an async HTTP listener will be used while waiting for the auth response, which could resolve the issues in [some cases](https://forum.unity.com/threads/google-drive-sdk-for-unity-free-open-sourced.515360/page-2#post-3498766).

### Will this plugin appear on the Asset Store?
I'll consider publishing when (if) it'll be in a more mature state (full API cover, more tests, less bugs); and whether that'll happen depends on the ~~amount of stars~~ feedback it'll receive :) In any case, the plugin will remain free and open-sourced.
