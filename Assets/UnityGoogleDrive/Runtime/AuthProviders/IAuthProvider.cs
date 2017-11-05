using System;

/// <summary>
/// Implementation is able to retrieve access and refresh tokens using provided settings.
/// </summary>
public interface IAuthProvider
{
    event Action<IAuthProvider> OnDone;

    bool IsDone { get; }
    bool IsError { get; }
    string AccessToken { get; }
    string RefreshToken { get; }

    void ProvideAuth (GoogleDriveSettings googleDriveSettings);
}
