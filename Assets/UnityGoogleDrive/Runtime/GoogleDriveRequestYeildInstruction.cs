using System;
using UnityEngine;

/// <summary>
/// Yield instruction to suspend coroutines while GoogleDriveRequest is running.
/// </summary>
/// <typeparam name="T">Type of the response data of the request the instruction is serving for.</typeparam>
public class GoogleDriveRequestYeildInstruction<T> : CustomYieldInstruction where T : GoogleDriveResource
{
    /// <summary>
    /// Event invoked when corresponding request is done running.
    /// Make sure to check for IsError before using the result.
    /// </summary>
    public event Action<T> OnDone;

    public override bool keepWaiting { get { return !GoogleDriveRequest.IsDone; } }
    public float Progress { get { return GoogleDriveRequest.Progress; } }
    public GoogleDriveRequest<T> GoogleDriveRequest { get; private set; }

    public GoogleDriveRequestYeildInstruction (GoogleDriveRequest<T> googleDriveRequest)
    {
        GoogleDriveRequest = googleDriveRequest;
        GoogleDriveRequest.OnDone += HandleRequestDone;
    }

    private void HandleRequestDone (T response)
    {
        GoogleDriveRequest.OnDone -= HandleRequestDone;
        if (OnDone != null)
            OnDone.Invoke(response);
    }
}
