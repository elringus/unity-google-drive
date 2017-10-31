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
    public bool IsError { get { return WebRequest.isNetworkError || WebRequest.isHttpError; } }
    #else
    public bool IsError { get { return webRequest.isError; } }
    #endif
    public bool IsDone { get { return webRequest.isDone && isDone; } }

    private UnityWebRequest webRequest;
    private GoogleDriveRequestAsyncOperation<T> asyncOperation;
    private bool isDone;

    public GoogleDriveRequest (string url, string method)
    {
        webRequest = new UnityWebRequest(url, method);
    }

    public GoogleDriveRequestAsyncOperation<T> Send ()
    {
        if (asyncOperation != null) return null;
        asyncOperation = new GoogleDriveRequestAsyncOperation<T>(this);

        OnBeforeSend(webRequest);

        #if UNITY_2017_2_OR_NEWER
        WebRequest.SendWebRequest().completed += OnWebRequestDone;
        #else
        webRequest.Send();
        WaitForWebRequest();
        #endif

        return asyncOperation;
    }

    public void Abort ()
    {
        webRequest.Abort();
    }

    public void Dispose ()
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

    private void WaitForWebRequest ()
    {
        containerObject = new UnityEngine.GameObject("GoogleDriveRequest");
        containerObject.hideFlags = UnityEngine.HideFlags.DontSave;
        UnityEngine.Object.DontDestroyOnLoad(containerObject);
        var coroutineContainer = containerObject.AddComponent<CoroutineContainer>();
        coroutineContainer.StartCoroutine(CheckWebRequestRoutine());
    }

    private IEnumerator CheckWebRequestRoutine ()
    {
        while (!webRequest.isDone) yield return null;
        OnWebRequestDone();
        UnityEngine.Object.Destroy(containerObject);
    }
    #endif
}
