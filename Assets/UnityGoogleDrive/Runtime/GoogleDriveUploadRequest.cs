using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// A request intended to communicate with the Google Drive API.
    /// Allows uploading a <see cref="Data.ResourceData"/> and (optionally) raw payload data.
    /// </summary>
    /// <typeparam name="TRequest">Type of the uploaded data.</typeparam>
    /// <typeparam name="TResponse">Type of the response data.</typeparam>
    public class GoogleDriveUploadRequest<TRequest, TResponse> : GoogleDriveRequest<TResponse> where TRequest : Data.ResourceData
    {
        /// <summary>
        /// The uploaded meta data of the request.
        /// </summary>
        public TRequest RequestData { get; protected set; }
        /// <summary>
        /// The uploaded raw payload data of the request.
        /// </summary>
        public byte[] RequestPayload { get; protected set; }
        /// <summary>
        /// MIME type of the <see cref="RequestPayload"/>.
        /// </summary>
        public string PayloadMimeType { get; protected set; }
        /// <summary>
        /// Whether the request has <see cref="RequestPayload"/>.
        /// </summary>
        public bool HasPayload => RequestPayload != null;
        /// <summary>
        /// Progress of the data upload, in 0.0 to 1.0 range.
        /// </summary>
        public override float Progress => WebRequest?.uploadProgress ?? 0;

        /// <summary>
        /// The type of upload request to the /upload URI. Acceptable values are:
        ///   - media - Simple upload. Upload the media only, without any metadata.
        ///   - multipart - Multipart upload. Upload both the media and its metadata, in a single request.
        ///   - resumable - Resumable upload. Upload the file in a resumable fashion.
        /// </summary>
        [QueryParameter] public virtual string UploadType => HasPayload ? "multipart" : null;

        private const string RequestContentType = "application/json; charset=UTF-8";
        private const string DefaultMimeType = "application/octet-stream";

        public GoogleDriveUploadRequest (string uri, string method, TRequest requestData,
            byte[] requestPayload = null, string payloadMimeType = null) : base(uri, method)
        {
            RequestData = requestData;
            if (requestPayload != null)
            {
                RequestPayload = requestPayload;
                PayloadMimeType = string.IsNullOrEmpty(payloadMimeType) ? DefaultMimeType : payloadMimeType;
            }
        }

        protected override UnityWebRequest CreateWebRequest ()
        {
            var webRequest = base.CreateWebRequest();
            return HasPayload ? CreateMultipartUpload(webRequest) : CreateSimpleUpload(webRequest);
        }

        protected UnityWebRequest CreateMultipartUpload (UnityWebRequest webRequest)
        {
            // Can't use MultipartFormDataSection utils to build multipart body,
            // because Google has added strict requirements for the body format.
            // Issue: https://github.com/Elringus/UnityGoogleDrive/issues/30).

            var newLine = "\r\n";
            var newLineDouble = newLine + newLine;
            var boundary = Encoding.ASCII.GetString(UnityWebRequest.GenerateBoundary());
            var boundaryDelimiter = newLineDouble + "--" + boundary;

            var dataList = new List<byte>();
            dataList.AddRange(Encoding.UTF8.GetBytes(
                boundaryDelimiter +
                newLine + "Content-Type: " + RequestContentType +
                newLineDouble + JsonUtils.ToJsonPrivateCamel(RequestData) +
                boundaryDelimiter +
                newLine + "Content-Type: " + DefaultMimeType +
                newLineDouble));
            dataList.AddRange(RequestPayload);
            dataList.AddRange(Encoding.UTF8.GetBytes(newLine + "--" + boundary + "--"));

            webRequest.uploadHandler = new UploadHandlerRaw(dataList.ToArray());
            webRequest.SetRequestHeader("Content-Type", string.Concat("multipart/related; boundary=", boundary));

            return webRequest;
        }

        protected UnityWebRequest CreateSimpleUpload (UnityWebRequest webRequest)
        {
            var requestJson = JsonUtils.ToJsonPrivateCamel(RequestData);
            var requestData = Encoding.UTF8.GetBytes(requestJson);
            webRequest.uploadHandler = new UploadHandlerRaw(requestData);
            webRequest.SetRequestHeader("Content-Type", RequestContentType);
            return webRequest;
        }
    }
}
