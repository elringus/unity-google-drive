using System;
using UnityEngine;

namespace UnityGoogleDrive
{
    public abstract class GoogleDriveRequestYieldInstruction : CustomYieldInstruction
    {
        public event Action OnDoneNonGeneric;
        public abstract bool IsDone { get; }

        protected void InvokeOnDoneNonGeneric ()
        {
            if (OnDoneNonGeneric != null)
                OnDoneNonGeneric.Invoke();
        }
    }

    /// <summary>
    /// Yield instruction to suspend coroutines while <see cref="GoogleDriveRequest{TData}"/> is running.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response data of the request the instruction is serving for.</typeparam>
    public class GoogleDriveRequestYieldInstruction<TResponse> : GoogleDriveRequestYieldInstruction
    {
        /// <summary>
        /// Event invoked when corresponding request is done running.
        /// Make sure to check for <see cref="UnityGoogleDrive.GoogleDriveRequest.IsError"/> before using the response data.
        /// </summary>
        public event Action<TResponse> OnDone;

        public override bool IsDone => GoogleDriveRequest.IsDone;
        public override bool keepWaiting => !IsDone;
        public float Progress => GoogleDriveRequest.Progress;
        public GoogleDriveRequest<TResponse> GoogleDriveRequest { get; private set; }

        public GoogleDriveRequestYieldInstruction (GoogleDriveRequest<TResponse> googleDriveRequest)
        {
            GoogleDriveRequest = googleDriveRequest;
            GoogleDriveRequest.OnDone += HandleRequestDone;
        }

        private void HandleRequestDone (TResponse responseData)
        {
            GoogleDriveRequest.OnDone -= HandleRequestDone;
            if (OnDone != null)
                OnDone.Invoke(responseData);
            InvokeOnDoneNonGeneric();
        }
    }
}
