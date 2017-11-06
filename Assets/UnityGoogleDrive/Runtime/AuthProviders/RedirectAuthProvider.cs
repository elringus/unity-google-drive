using System;

/// <summary>
/// Provides auth and refresh tokens using redirect method to retrieve authorization code.
/// Protocol: https://developers.google.com/identity/protocols/OAuth2WebServer#obtainingaccesstokens.
/// </summary>
public class RedirectAuthProvider : IAuthProvider
{
    public event Action<IAuthProvider> OnDone;

    public bool IsDone { get; private set; }
    public bool IsError { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }

    public void ProvideAuth (GoogleDriveSettings googleDriveSettings)
    {
        if (OnDone != null) OnDone.Invoke(this);
        throw new NotImplementedException();
    }
}
