## Download package
For Unity 2017.3 and later: [UnityGoogleDrive.unitypackage](https://github.com/Elringus/UnityGoogleDrive/releases/download/v0.20-alpha/UnityGoogleDrive.unitypackage). Check [releases](https://github.com/Elringus/UnityGoogleDrive/releases) for previous versions support.

**In case you're not familiar with the Google Drive API, please read through the [official documentation](https://developers.google.com/drive/api/v3/about-sdk) and [FAQ](https://github.com/Elringus/UnityGoogleDrive#faq), before using this package or opening new issues.**

## Description
[Google Drive](https://www.google.com/drive/) API library for listing, searching, creating, uploading, editing, copying, downloading, deleting and exporting files on the user's drive from within [Unity game engine](https://unity3d.com/).

Supports all the major platforms: Windows, Mac, Linux, iOS, Android and WebGL.

[AppAuth-Android](https://github.com/openid/AppAuth-Android) and [AppAuth-iOS](https://github.com/openid/AppAuth-iOS) native libraries are used to perform authentication on Android and iOS respectively for better user experience; accompanying native client sources: [UnityGoogleDriveAndroid](https://github.com/Elringus/UnityGoogleDriveAndroid), [UnityGoogleDriveIOS](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/UnityGoogleDrive/Plugins/com.elringus.unitygoogledriveios.mm). [PlayServicesResolver](https://github.com/googlesamples/unity-jar-resolver) dependency file is [provided in the distributed package](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/UnityGoogleDrive/Editor/Dependencies.xml) to automatically resolve dependencies.

Three authentication schemes are used: browser redirection for WebGL builds, custom URI for iOS/Android and local loopback requests for other platforms. All the credentials are stored in a scriptable object; editor script provides shortcuts to create and manage Google Console App, allows to parse credentials JSON to skip manual copy-pasting and edit common settings:

![Settings](https://i.gyazo.com/50c58d42173658a504e0ea19ef522a2f.png) 

Automated integration tests cover the main features:

![Tests](https://i.gyazo.com/128acac61f5c719376b0f32f70144168.png) 

## Setup (editor, standalones and WebGL)
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
- Return to Unity editor, open Google Drive settings and click **Parse generic credentials JSON file**; select the downloaded credentials json file;
- When under .NET 3.5 scripting runtime, make sure to set API compatibility level to the **full .NET 2.0 profile** (not subset) to prevent [JSON parsing issues](https://github.com/Elringus/UnityGoogleDrive/issues/6) on AOT platforms.

## Additional setup for iOS and Android
- In the Unity editor navigate to `Edit -> Project Settings -> Google Drive Settings`, click **Manage Google Drive API app** web-browser will open URL to manage the console app that was created during the initial setup;
- Click **Create credentials** -> **OAuth client ID** to add a new OAuth client to be used when authenticating on iOS and Android;
- Select **iOS** for the `Application type` (it'll still work for both iOS and Android);
- Enter anything you like for the `Name` field (eg, `URI Scheme Client`);
- Enter your Unity's **application ID** for the `Bundle ID` field (eg, `com.elringus.unitygoogledrive`). Make sure your applicaton ID is **lower-cased** (both in the editor and in the creadentials). In case you're unable to change Application ID (eg, app is already published), see [FAQ for available workarounds](https://github.com/Elringus/UnityGoogleDrive#my-application-id-andriodios-is-mixed-cased-and-i-cant-change-it);
- Leave the remaining fields blank and click **Create** button;
- Download the credentials file by clicking the **Download plist** button;
- Return to Unity editor, open Google Drive settings and click **Parse URI scheme credentials PLIST file**; select the downloaded credentials plist file;
- Download and install [PlayServicesResolver](https://github.com/googlesamples/unity-jar-resolver) package to automatically resolve Android and iOS native dependencies. The dependency file is located at `./UnityGoogleDrive/Editor/Dependencies.xml`. When the PlayServicesResolver is installed and editor is switched to Android build target, [all the required .jar and .aar files](https://i.gyazo.com/8ce9d0b9ad7c45093b3c282843f3b1df.png) will automatically be downloaded to the `Assets/Plugins/Android` folder. When swithed to iOS, a [CocoaPods](https://cocoapods.org/) Pod file will be added to the generated iOS project on build post-process. When developing under Windows, you'll have to manually run `pod install` in the XCode project directory to install the iOS dependencies.

## Examples
The design mostly follows the official [Google APIs Client Library for .NET](https://github.com/google/google-api-dotnet-client):

```csharp
// Listing files.
GoogleDriveFiles.List().Send().OnDone += fileList => ...;

// Uploading a file.
var file = new UnityGoogleDrive.Data.File { Name = "Image.png", Content = rawFileData };
GoogleDriveFiles.Create(file).Send();

// Downloading a file.
GoogleDriveFiles.Download(fileId).Send().OnDone += file => ...;

// All the requests are compatible with the .NET 4 asynchronous model.
var aboutData = await GoogleDriveAbout.Get().Send();
```

For more examples take a look at [test scripts](https://github.com/Elringus/UnityGoogleDrive/tree/master/Assets/Scripts).

## Implemented APIs
The following [Google Drive APIs](https://developers.google.com/drive/api/v3/reference/) are currently implemented:
- [x] [About](https://developers.google.com/drive/api/v3/reference/about)
- [x] [Changes](https://developers.google.com/drive/api/v3/reference/changes)
- [ ] [Channels](https://developers.google.com/drive/api/v3/reference/channels)
- [ ] [Comments](https://developers.google.com/drive/api/v3/reference/comments)
- [x] [Files](https://developers.google.com/drive/api/v3/reference/files)
- [ ] [Permissions](https://developers.google.com/drive/api/v3/reference/permissions)
- [ ] [Replies](https://developers.google.com/drive/api/v3/reference/replies)
- [ ] [Revisions](https://developers.google.com/drive/api/v3/reference/revisions)
- [x] [Teamdrives](https://developers.google.com/drive/api/v3/reference/teamdrives)

## FAQ

### Why the returned properties of the response are all null?
Most of the response properties are null by default. You have to explicitly require fields in order for the drive API to return them (using `Fields` property of the request object). More info here: https://developers.google.com/drive/v3/web/performance#partial.

### How to access a file using its path?
A folder in Google Drive is actually a file with the MIME type `application/vnd.google-apps.folder`. Hierarchy relationship is implemented via file's `Parents` property. To get the actual file using its path we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. Thus, we have to traverse the entire hierarchy chain using `GoogleDriveFiles.List` requests. 

You can find a naive implementation of the aforementioned logic via Unity's coroutine in the [example script](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/ExampleGetFileByPath.cs) and use it as a reference for your own solution or utilize the [built-in async helpers](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/UnityGoogleDrive/Runtime/Utilities/Helpers.cs) `FindFilesByPathAsync` and `CreateOrUpdateFileAtPathAsync` (requires .NET 4.x).

More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

### Is it possible to download/upload large files in chunks to reduce memory usage?
To perform a partial download you have to supply `downloadRange` agrument for the `GoogleDriveFiles.Download` request specifying the bytes range you wish to get. Here is an [example script for a partial text file download](https://github.com/Elringus/UnityGoogleDrive/blob/master/Assets/Scripts/TestFilesDownloadRange.cs). More info on partial downloads can be found in the [API docs](https://developers.google.com/drive/api/v3/manage-downloads#partial_download).

For the chunked uploads you'll have to use resumable upload requests in a special manner. First, create a resumable upload request (via either `GoogleDriveFiles.CreateResumable` or `GoogleDriveFiles.UpdateResumable`), supply the file's metadata, but don't set the file's `Content` property (make sure it's `null`). Send the request and get a resumable session URI from the response. Now you can use the session URI to upload the file's content in chunks. See the [Drive API docs](https://developers.google.com/drive/api/v3/resumable-upload#upload-resumable) for detailed instructions on how to perform a chunked upload using a resumable session URI.

### Is there a way to automatically redirect user to the app when authorization in browser is complete?
When using custom URI authentication scheme on iOS/Android, redirection will be handled by the native libs automatically. On WebGL the redirection is also performed automatically. When user finishes authorization flow using loopback scheme, an HTML string is injected to the active browser window. The default content of the HTML contains a message, asking user to return to the app. You can modify the content of the injected HTML in the Google Drive Settings asset using `Loopback Response HTML` field. It’s possible to inject a javascript code to this HTML, which will be invoked right after the auth flow is completed. You can use this option to automatically redirect user back to your app using a custom URI scheme. Specific implementation will depend on the platform; eg, for Windows [bind the application to a URI scheme](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85)).

### How to logout or force user to login/select another Google account?
Use `GoogleDriveSettings.DeleteCachedAuthTokens` method to clear cached authentication tokens which will force the user to login again on the next request. You can always get `GoogleDriveSettings` instance using `GoogleDriveSettings.LoadFromResources` static method. While in editor, you can also use 'Delete cached tokens' button for the same effect.

### Can I access someone else's Google drive or skip authentication in the browser?
To work with anyone's Google Drive, it's mandatory to complete OAuth procedure for that user, which requires opening a browser window to login and allow the app to access their drive. It's a security measure [enforced by Google](https://developers.google.com/identity/protocols/OAuth2). 

### Is it possible to use an embedded browser (WebView) for the authorization?
It’s not possible. Google is intentionally [blocking authorization requests sent from any sort of embedded browsers](https://auth0.com/blog/google-blocks-oauth-requests-from-embedded-browsers/) for security reasons.

### Can I share a drive account? 
The only legit way to allow multiple users share a drive account is to use [Team Drives](https://gsuite.google.com/learning-center/products/drive/get-started-team-drive/).

### Is it possible to access shared files and folders?
This is possible. To access shared resources you'll have to specify ["Shared with me" collection](https://developers.google.com/drive/v3/web/about-organization#shared_with_me) when resolving ID of the resource. Additionally, if the shared resource has been [added to the user's drive](https://support.google.com/drive/answer/2375057?co=GENIE.Platform%3DDesktop&hl=en) it'll be accessible via the path finding method described above.

### My application ID (Andriod/iOS) is mixed-cased and I can't change it.
Application ID (aka bundle ID, package name) is used as a custom URI scheme on Android and iOS to receive authorization callback from Google’s OAuth server and redirect user back to the app. Even though the initial request sent by UnityGoogleDrive plugin preserves casing of the application ID (as it’s set in the player settings), Google’s OAuth server will forcibly convert it to lowercase when redirecting the user. That’s why it’s mandatory to use lowercased application ID. If, however, you’re unable to change it (eg, app is already published on the store), you can do the following: 
- Android: override `${applicationId}` record in the `./UnityGoogleDrive/Plugins/com.elringus.unitygoogledriveandroid.aar/AndroidManifest.xml` (you’ll have to unzip the .aar) to your application’s ID (lower-cased);
- iOS: add your application’s ID (lower-cased) to the [Supported URL schemes](https://i.gyazo.com/efafe276a3d566d7563e83005873746b.png) list in the iOS player settings. 

### Will this plugin appear on the Asset Store?
I'll consider publishing when (if) it'll be in a more mature state (full API cover, more tests, less bugs); and whether that'll happen depends on the ~~amount of stars~~ feedback it'll receive :) In any case, the plugin will remain free and open-sourced.
