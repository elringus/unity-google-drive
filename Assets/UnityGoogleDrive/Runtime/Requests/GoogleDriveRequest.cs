using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Base class for all the requests intended to communicate with the Google Drive API. 
/// Implement OnBeforeSend and OnBeforeDone methods in the derived classes to configure the request according to the API.
/// </summary>
/// <typeparam name="T">
/// Auto-constrainted generic type to fake return type covariance.
/// Use derived class type here (eg, DownloadRequest : GoogleDriveRequest<DownloadRequest>).
/// </typeparam>
public abstract class GoogleDriveRequest<T> : IDisposable where T : GoogleDriveRequest<T>
{
    public event Action<T> OnDone;

    public string Uri { get; private set; }
    public string Method { get; private set; }
    public bool IsRunning { get { return yeildInstruction != null && !IsDone; } }
    public bool IsDone { get; protected set; }
    public bool IsError { get; protected set; }
    public string Error { get; protected set; }

    protected static GoogleDriveSettings Settings { get; private set; }
    protected static AuthState AuthState { get; private set; }

    private UnityWebRequest webRequest;
    private GoogleDriveRequestYeildInstruction<T> yeildInstruction;

    public GoogleDriveRequest (string uri, string method)
    {
        Uri = uri;
        Method = method;

        if (Settings == null)
            Settings = GoogleDriveSettings.LoadFromResources();

        if (AuthState == null)
            AuthState = new AuthState(Settings.AuthCredentials, Settings.SharedRefreshToken);
    }

    public GoogleDriveRequestYeildInstruction<T> Send ()
    {
        if (!IsRunning)
        {
            yeildInstruction = new GoogleDriveRequestYeildInstruction<T>(this);
            SendWebRequest();
        }
        return yeildInstruction;
    }

    public virtual void Abort ()
    {
        webRequest.Abort();
    }

    public virtual void Dispose ()
    {
        webRequest.Dispose();
    }

    protected abstract void OnBeforeSend (UnityWebRequest webRequest);
    protected abstract void OnBeforeDone (UnityWebRequest webRequest);

    private void SendWebRequest ()
    {
        if (webRequest != null)
        {
            webRequest.Abort();
            webRequest.Dispose();
        }

        webRequest = new UnityWebRequest(Uri, Method);
        webRequest.SetRequestHeader("Authorization", string.Format("Authorization: Bearer {0}", AuthState.AccessToken));
        webRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.REQUEST_CONTENT_TYPE);

        OnBeforeSend(webRequest);

        webRequest.RunWebRequest().completed += HandleWebRequestDone;
    }

    private void HandleWebRequestDone ()
    {
        if (webRequest.responseCode == GoogleDriveSettings.UNAUTHORIZED_RESPONSE_CODE)
        {
            HandleUnauthorizedResponse();
            return;
        }

        Error = webRequest.error;
        IsError = !string.IsNullOrEmpty(Error);

        OnBeforeDone(webRequest);

        IsDone = true;

        if (OnDone != null)
            OnDone.Invoke(this as T);

        webRequest.Dispose();
    }

    private void HandleUnauthorizedResponse ()
    {
        AuthState.OnAccessTokenRefreshed += HandleAccessTokenRefreshed;
        AuthState.RefreshAccessToken();
    }

    private void HandleAccessTokenRefreshed ()
    {
        AuthState.OnAccessTokenRefreshed -= HandleAccessTokenRefreshed;
        SendWebRequest();
    }
}
