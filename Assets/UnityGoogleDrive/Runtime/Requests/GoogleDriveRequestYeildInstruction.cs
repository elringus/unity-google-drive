using UnityEngine;

public class GoogleDriveRequestYeildInstruction<T> : CustomYieldInstruction where T : GoogleDriveRequest<T>
{
    public override bool keepWaiting { get { return !GoogleDriveRequest.IsDone; } }
    public GoogleDriveRequest<T> GoogleDriveRequest { get; private set; }

    public GoogleDriveRequestYeildInstruction (GoogleDriveRequest<T> googleDriveRequest)
    {
        GoogleDriveRequest = googleDriveRequest;
    }
}
