using UnityEngine;

public class GoogleDriveRequestAsyncOperation<T> : CustomYieldInstruction where T : GoogleDriveRequest<T>
{
    public override bool keepWaiting { get { return !GoogleDriveRequest.IsDone; } }
    public GoogleDriveRequest<T> GoogleDriveRequest { get; private set; }

    public GoogleDriveRequestAsyncOperation (GoogleDriveRequest<T> googleDriveRequest)
    {
        GoogleDriveRequest = googleDriveRequest;
    }
}
