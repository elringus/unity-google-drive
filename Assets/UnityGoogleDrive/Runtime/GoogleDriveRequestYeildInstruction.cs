using System;
using UnityEngine;

/// <summary>
/// Yield instruction to suspend coroutines while <see cref="GoogleDriveRequest{TData}"/> is running.
/// </summary>
/// <typeparam name="T">Type of the response data of the request the instruction is serving for.</typeparam>
public class GoogleDriveRequestYeildInstruction<TData> : CustomYieldInstruction where TData : Data.ResourceData
{
    /// <summary>
    /// Event invoked when corresponding request is done running.
    /// Make sure to check for <see cref="GoogleDriveRequest.IsError"/> before using the response data.
    /// </summary>
    public event Action<TData> OnDone;

    public override bool keepWaiting { get { return !GoogleDriveRequest.IsDone; } }
    public float Progress { get { return GoogleDriveRequest.Progress; } }
    public GoogleDriveRequest<TData> GoogleDriveRequest { get; private set; }

    public GoogleDriveRequestYeildInstruction (GoogleDriveRequest<TData> googleDriveRequest)
    {
        GoogleDriveRequest = googleDriveRequest;
        GoogleDriveRequest.OnDone += HandleRequestDone;
    }

    private void HandleRequestDone (TData responseData)
    {
        GoogleDriveRequest.OnDone -= HandleRequestDone;
        if (OnDone != null)
            OnDone.Invoke(responseData);
    }
}
