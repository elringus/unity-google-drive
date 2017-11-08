using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#if !UNITY_2017_2_OR_NEWER
/// <summary>
/// Wrapper to use UnityWebRequest in event-driven manner.
/// In Unity 2017.2 'completed' event was introduced, which should be used instead. 
/// </summary>
public class UnityWebRequestRunner
{
    class CoroutineContainer : MonoBehaviour { }

    #pragma warning disable IDE1006 
    public event Action<AsyncOperation> completed;
    #pragma warning restore IDE1006 

    public UnityWebRequest UnityWebRequest { get; private set; }

    private AsyncOperation requestYield;
    private MonoBehaviour coroutineContainer;
    private GameObject containerObject;
    private Coroutine coroutine;

    public UnityWebRequestRunner (UnityWebRequest unityWebRequest, MonoBehaviour coroutineContainer = null)
    {
        UnityWebRequest = unityWebRequest;
        if (coroutineContainer) this.coroutineContainer = coroutineContainer;
        else this.coroutineContainer = CreateContainer();
    }

    public void Run ()
    {
        if (UnityWebRequest == null)
        {
            Debug.LogWarning("UnityGoogleDrive: Attempted to start UnityWebRequestRunner with non-valid UnityWebRequest.");
            OnComplete();
            return;
        }

        if (!coroutineContainer || !coroutineContainer.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("UnityGoogleDrive: Attempted to start UnityWebRequestRunner with non-valid container.");
            OnComplete();
            return;
        }

        coroutine = coroutineContainer.StartCoroutine(RequestRoutine());
        requestYield = UnityWebRequest.Send();
    }

    public void Abort ()
    {
        if (coroutine != null)
        {
            if (coroutineContainer)
                coroutineContainer.StopCoroutine(coroutine);
            coroutine = null;
        }

        UnityWebRequest.Abort();

        OnComplete();
    }

    private IEnumerator RequestRoutine ()
    {
        while (UnityWebRequest != null && !UnityWebRequest.isDone)
            yield return null;
        OnComplete();
    }

    private void OnComplete ()
    {
        if (completed != null)
            completed.Invoke(requestYield);
        if (containerObject)
            UnityEngine.Object.Destroy(containerObject);
    }

    private MonoBehaviour CreateContainer ()
    {
        containerObject = new GameObject("UnityWebRequest");
        containerObject.hideFlags = HideFlags.DontSave;
        UnityEngine.Object.DontDestroyOnLoad(containerObject);
        return containerObject.AddComponent<CoroutineContainer>();
    }
}
#endif

public static class UnityWebRequestExtensions
{
    #if UNITY_2017_2_OR_NEWER
    public static UnityWebRequestAsyncOperation RunWebRequest (this UnityWebRequest unityWebRequest)
    {
        return unityWebRequest.SendWebRequest();
    }
    #else
    public static UnityWebRequestRunner RunWebRequest (this UnityWebRequest unityWebRequest)
    {
        var runner = new UnityWebRequestRunner(unityWebRequest);
        runner.Run();
        return runner;
    }
    #endif
}
