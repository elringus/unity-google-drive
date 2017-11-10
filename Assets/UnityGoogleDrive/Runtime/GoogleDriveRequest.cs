using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A request intended to communicate with the Google Drive API. 
/// Handles base networking and authorization flow.
/// </summary>
/// <typeparam name="T">Type of the response data.</typeparam>
public class GoogleDriveRequest<T> : IDisposable where T : GoogleDriveResource
{
    /// <summary>
    /// Event invoked when the request is done running.
    /// Make sure to check for IsError before using the result.
    /// </summary>
    public event Action<T> OnDone;

    public string Uri { get; private set; }
    public string Method { get; private set; }
    public GoogleDriveQueryParameters QueryParameters { get; private set; }
    public T Response { get; protected set; }
    public float Progress { get { return webRequestYeild != null ? webRequestYeild.progress : 0; } }
    public bool IsRunning { get { return yeildInstruction != null && !IsDone; } }
    public bool IsDone { get; protected set; }
    public bool IsError { get { return !string.IsNullOrEmpty(Error); } }
    public string Error { get; protected set; }

    protected static GoogleDriveSettings Settings { get; private set; }
    protected static AuthController AuthController { get; private set; }

    private UnityWebRequest webRequest = null;
    private AsyncOperation webRequestYeild = null;
    private GoogleDriveRequestYeildInstruction<T> yeildInstruction = null;

    public GoogleDriveRequest (string uri, string method, GoogleDriveQueryParameters queryParameters = null)
    {
        Uri = uri;
        Method = method;
        QueryParameters = queryParameters;

        if (Settings == null) Settings = GoogleDriveSettings.LoadFromResources();
        if (AuthController == null) AuthController = new AuthController(Settings);
    }

    /// <summary>
    /// Begin communicating with the Google Drive API to execute the request.
    /// </summary>
    /// <returns>
    /// A yeild instruction indicating the progress/completion state of the request.
    /// Yield this object to wait until the request is done or use OnDone event.
    /// </returns>
    public GoogleDriveRequestYeildInstruction<T> Send ()
    {
        if (!IsRunning)
        {
            yeildInstruction = new GoogleDriveRequestYeildInstruction<T>(this);
            SendWebRequest();
        }
        return yeildInstruction;
    }

    /// <summary>
    /// If in progress, halts the request as soon as possible.
    /// </summary>
    public virtual void Abort ()
    {
        webRequest.Abort();
    }

    /// <summary>
    /// Signals the request is no longer being used, and should clean up any resources it is using.
    /// </summary>
    public virtual void Dispose ()
    {
        webRequest.Dispose();
    }

    /// <summary>
    /// Invoked before sending the request.
    /// </summary>
    protected virtual void OnBeforeSend (UnityWebRequest webRequest) { }

    /// <summary>
    /// Invoked before completing the request.
    /// </summary>
    protected virtual void OnBeforeDone (UnityWebRequest webRequest) { }

    private void SendWebRequest ()
    {
        if (webRequest != null)
        {
            webRequest.Abort();
            webRequest.Dispose();
        }

        webRequest = new UnityWebRequest(Uri, Method);
        webRequest.SetRequestHeader("Authorization", string.Format("Bearer {0}", AuthController.AccessToken));
        webRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.REQUEST_CONTENT_TYPE);
        if (QueryParameters != null) webRequest.url += QueryParameters.GenerateRequestPayload();
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        OnBeforeSend(webRequest);

        webRequest.RunWebRequest(ref webRequestYeild).completed += HandleWebRequestDone;
    }

    private void HandleWebRequestDone (AsyncOperation requestYeild)
    {
        if (webRequest.responseCode == GoogleDriveSettings.UNAUTHORIZED_RESPONSE_CODE)
        {
            HandleUnauthorizedResponse();
            return;
        }

        Error = webRequest.error;

        var responseText = webRequest.downloadHandler.text;
        if (!string.IsNullOrEmpty(responseText))
        {
            var apiError = JsonUtility.FromJson<GoogleDriveResponseError>(responseText);
            if (apiError.IsError) Error += apiError.Error.Message;
            if (!IsError) Response = JsonUtility.FromJson<T>(responseText);
        }

        if (IsError) Debug.LogError("UnityGoogleDrive: " + Error);

        OnBeforeDone(webRequest);

        IsDone = true;

        if (OnDone != null)
            OnDone.Invoke(Response);

        webRequest.Dispose();
    }

    private void HandleUnauthorizedResponse ()
    {
        AuthController.OnAccessTokenRefreshed += HandleAccessTokenRefreshed;
        AuthController.RefreshAccessToken();
    }

    private void HandleAccessTokenRefreshed ()
    {
        AuthController.OnAccessTokenRefreshed -= HandleAccessTokenRefreshed;
        SendWebRequest();
    }
}
