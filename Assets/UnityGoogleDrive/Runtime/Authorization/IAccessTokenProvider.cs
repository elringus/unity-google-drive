using System;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Implementation is able to retrieve access token.
    /// </summary>
    public interface IAccessTokenProvider
    {
        event Action<IAccessTokenProvider> OnDone;

        bool IsDone { get; }
        bool IsError { get; }
        string AccessToken { get; }

        void ProvideAccessToken ();
    }
}
