using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// A request intended to communicate with the Google Drive API.
    /// Allows uploading a <see cref="Data.ResourceData"/> and (optinally) raw payload data in a resumable fashion.
    /// </summary>
    /// <typeparam name="TRequest">Type of the uploaded data.</typeparam>
    /// <see href="https://developers.google.com/drive/api/v3/resumable-upload"/>
    public class GoogleDriveResumableUploadRequest<TRequest> : GoogleDriveUploadRequest<TRequest, string> where TRequest : Data.ResourceData
    {
        /// <summary>
        /// Used to resume file data upload and get upload status.
        /// </summary>
        public string ResumableSessionUri { get; private set; }
        /// <summary>
        /// Progress of the data upload, in 0.0 to 1.0 range.
        /// </summary>
        public override float Progress => EvaluateUploadProgress();
        public override string ResponseData => ResumableSessionUri;

        /// <summary>
        /// The type of upload request to the /upload URI. Acceptable values are:
        ///   - media - Simple upload. Upload the media only, without any metadata.
        ///   - multipart - Multipart upload. Upload both the media and its metadata, in a single request.
        ///   - resumable - Resumable upload. Upload the file in a resumable fashion.
        /// </summary>
        [QueryParameter] public override string UploadType => "resumable";

        protected override bool AutoCompleteOnDone => false;
        protected bool ResumableSessionInitiated => !string.IsNullOrEmpty(ResumableSessionUri);
        protected int ResumeOffset { get; set; }

        private UnityWebRequest uploadRequest;
        private UnityWebRequest statusRequest;

        public GoogleDriveResumableUploadRequest (string uri, string method, TRequest requestData, byte[] requestPayload = null,
            string payloadMimeType = null, string resumableSessionUri = null) : base(uri, method, requestData, requestPayload, payloadMimeType)
        {
            ResumableSessionUri = resumableSessionUri;
        }

        protected override UnityWebRequest CreateWebRequest ()
        {
            // Base request will be used to initiate resumable upload session, upload metadata and get session URI.
            var webRequest = base.CreateWebRequest();
            return CreateSimpleUpload(webRequest);
        }

        protected override void SendWebRequest ()
        {
            // If session URI wasn't provided by user send base request to initiate a new session.
            if (!ResumableSessionInitiated) base.SendWebRequest();
            // Otherwise get status of the unfinished session and resume the upload.
            else if (HasPayload) GetStatusAndResumeUpload();
            else // User provided unfinished session URI but not the payload; cancel the request.
            {
                Debug.LogError("UnityGoogleDrive: Can't resume upload without the payload data. Make sure to set File.Data field.");
                CompleteRequest();
            }
        }

        protected override void HandleWebRequestDone (AsyncOperation requestYield)
        {
            base.HandleWebRequestDone(requestYield);
            if (IsError) { CompleteRequest(); return; }
            ResumableSessionUri = WebRequest.GetResponseHeader("Location");
            // New resumable upload session initiated and we have the payload, so start uploading.
            if (HasPayload) ResumeUpload();
            // User didn't provide the payload, so just return the session URI for manual uploading.
            else CompleteRequest();
        }

        public override void Dispose ()
        {
            base.Dispose();

            uploadRequest?.Dispose();
            statusRequest?.Dispose();
        }

        public override void Abort ()
        {
            base.Abort();

            uploadRequest?.Abort();
            statusRequest?.Abort();
        }

        private void GetStatusAndResumeUpload ()
        {
            Debug.Assert(ResumableSessionInitiated && HasPayload);

            if (statusRequest != null)
            {
                statusRequest.Abort();
                statusRequest.Dispose();
            }

            statusRequest = new UnityWebRequest(ResumableSessionUri, UnityWebRequest.kHttpVerbPUT);
            statusRequest.SetRequestHeader("Content-Range", $"bytes */{RequestPayload.Length}");
            statusRequest.SendWebRequest().completed += HandleStatusRequestCompleted;
        }

        private void HandleStatusRequestCompleted (AsyncOperation asyncOperation)
        {
            // The upload was completed, no further action is necessary.
            if (statusRequest.responseCode == 200 || statusRequest.responseCode == 201) CompleteRequest();
            // The upload session has expired and the upload needs to be restarted from the beginning.
            else if (statusRequest.responseCode == 404) { ResumeOffset = 0; ResumeUpload(); }
            // The upload wasn't completed and should be resumed.
            else if (statusRequest.responseCode == 308)
            {
                var rangeHeader = statusRequest.GetResponseHeader("Range");
                Debug.Assert(!string.IsNullOrEmpty(rangeHeader) && rangeHeader.Contains("-"));
                ResumeOffset = int.Parse(rangeHeader.Split('-')[1]) + 1;
                ResumeUpload();
            }
            // Unexpected response.
            else
            {
                AppendError($"Failed to resume upload. HTTP error: {statusRequest.error}");
                CompleteRequest();
            }

            statusRequest.Dispose();
        }

        private void ResumeUpload ()
        {
            Debug.Assert(ResumableSessionInitiated && HasPayload);

            if (uploadRequest != null)
            {
                uploadRequest.Abort();
                uploadRequest.Dispose();
            }

            if (ResumeOffset == 0) uploadRequest = UnityWebRequest.Put(ResumableSessionUri, RequestPayload);
            else
            {
                var partialPayload = new byte[RequestPayload.Length - ResumeOffset];
                Array.Copy(RequestPayload, ResumeOffset, partialPayload, 0, partialPayload.Length);
                uploadRequest = UnityWebRequest.Put(ResumableSessionUri, partialPayload);
                uploadRequest.SetRequestHeader("Content-Range", $"bytes {ResumeOffset}-{RequestPayload.Length - 1}/{RequestPayload.Length}");
            }

            uploadRequest.SendWebRequest().completed += HandleUploadRequestCompleted;
        }

        private void HandleUploadRequestCompleted (AsyncOperation asyncOperation)
        {
            if (!string.IsNullOrEmpty(uploadRequest.error))
                AppendError($"Failed to upload using resumable scheme. HTTP error: {uploadRequest.error}");

            CompleteRequest();
            uploadRequest.Dispose();
        }

        private float EvaluateUploadProgress ()
        {
            if (uploadRequest == null || !HasPayload || RequestPayload.Length == 0) return 0;

            if (ResumeOffset == 0) return uploadRequest.uploadProgress;
            var previousProgress = (float)ResumeOffset / RequestPayload.Length;
            return previousProgress + uploadRequest.uploadProgress * (1 - previousProgress);
        }
    }
}
