using System;
using System.Collections;
using UnityEngine.Networking;

#if !UNITY_2017_2_OR_NEWER
class CoroutineContainer : UnityEngine.MonoBehaviour { }
#endif

/// <summary>
/// Base class for all the requests intended to communicate with Google Drive API.
/// </summary>
/// <typeparam name="T">
/// Auto-constrainted generic type to fake return type covariance.
/// Use derived class type here (eg, DownloadRequest : GoogleDriveRequest<DownloadRequest>).
/// </typeparam>
public abstract class GoogleDriveRequest<T> : IDisposable where T : GoogleDriveRequest<T>
{
    public event Action<T> OnDone;

    #if UNITY_2017_2_OR_NEWER
    public virtual bool IsError { get { return WebRequest.isNetworkError || WebRequest.isHttpError; } }
    #else
    public virtual bool IsError { get { return webRequest.isError; } }
    #endif
    public virtual bool IsDone { get { return webRequest.isDone && isDone; } }

    protected static GoogleDriveSettings Settings { get; private set; }

    private UnityWebRequest webRequest;
    private GoogleDriveRequestAsyncOperation<T> requestAsyncOperation;
    private bool isDone;

    public GoogleDriveRequest (string url, string method)
    {
        if (!Settings) Settings = GoogleDriveSettings.LoadFromResources();
        webRequest = new UnityWebRequest(url, method);
    }

    public GoogleDriveRequestAsyncOperation<T> Send ()
    {
        if (requestAsyncOperation != null) return null; // Request is already running.
        requestAsyncOperation = new GoogleDriveRequestAsyncOperation<T>(this);

        OnBeforeSend(webRequest);

        #if UNITY_2017_2_OR_NEWER
        WebRequest.SendWebRequest().completed += OnWebRequestDone;
        #else
        webRequest.Send();
        WaitForResponse();
        #endif

        return requestAsyncOperation;
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

    private void OnWebRequestDone ()
    {
        OnBeforeDone(webRequest);

        isDone = true;

        if (OnDone != null)
            OnDone.Invoke(this as T);
    }

    #if !UNITY_2017_2_OR_NEWER
    private UnityEngine.GameObject containerObject;

    private void WaitForResponse ()
    {
        containerObject = new UnityEngine.GameObject("GoogleDriveRequest");
        containerObject.hideFlags = UnityEngine.HideFlags.DontSave;
        UnityEngine.Object.DontDestroyOnLoad(containerObject);
        var coroutineContainer = containerObject.AddComponent<CoroutineContainer>();
        coroutineContainer.StartCoroutine(WaitForResponseRoutine());
    }

    private IEnumerator WaitForResponseRoutine ()
    {
        while (!webRequest.isDone) yield return null;
        OnWebRequestDone();
        UnityEngine.Object.Destroy(containerObject);
    }
    #endif
}
