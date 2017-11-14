using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

/// <summary>
/// A request intended to communicate with the Google Drive API. 
/// Allows uploading a <see cref="Data.ResourceData"/> and (optionally) raw payload data.
/// </summary>
/// <typeparam name="TRequest">Type of the uploaded resource data.</typeparam>
/// <typeparam name="TResponse">Type of the response resource data.</typeparam>
public class GoogleDriveUploadRequest<TRequest, TResponse> : GoogleDriveRequest<TResponse> 
    where TRequest : Data.ResourceData where TResponse : Data.ResourceData
{
    public TRequest RequestData { get; protected set; }
    public byte[] RequestPayload { get; protected set; }
    public string PayloadMimeType { get; protected set; }
    public bool HasPayload { get { return RequestPayload != null; } }

    /// <summary>
    /// The type of upload request to the /upload URI. Acceptable values are:
    ///   - media - Simple upload. Upload the media only, without any metadata.
    ///   - multipart - Multipart upload. Upload both the media and its metadata, in a single request.
    ///   - resumable - Resumable upload. Upload the file in a resumable fashion.
    /// </summary>
    [QueryParameter] public string UploadType { get; private set; }

    private const string REQUEST_CONTENT_TYPE = "application/json; charset=UTF-8";
    private const string DEFAULT_MIME_TYPE = "application/octet-stream";
    private const string MULTIPART_UPLOAD_TYPE = "multipart";

    public GoogleDriveUploadRequest (string uri, string method, TRequest requestData, 
        byte[] requestPayload = null, string payloadMimeType = null) : base(uri, method)
    {
        RequestData = requestData;
        if (requestPayload != null)
        {
            RequestPayload = requestPayload;
            PayloadMimeType = string.IsNullOrEmpty(payloadMimeType) ? DEFAULT_MIME_TYPE : payloadMimeType;
            UploadType = MULTIPART_UPLOAD_TYPE;
        }
    }

    protected override UnityWebRequest CreateWebRequest ()
    {
        var webRequest = base.CreateWebRequest();
        return HasPayload ? CreateMultipartUpload(webRequest) : CreateSimpleUpload(webRequest);
    }

    private UnityWebRequest CreateMultipartUpload (UnityWebRequest webRequest)
    {
        var boundary = UnityWebRequest.GenerateBoundary();
        var metadata = new MultipartFormDataSection(null, JsonUtils.ToJsonPrivateCamel(RequestData), REQUEST_CONTENT_TYPE);
        var content = new MultipartFormDataSection(null, RequestPayload, PayloadMimeType);
        var formData = UnityWebRequest.SerializeFormSections(new List<IMultipartFormSection> { metadata, content }, boundary);

        // End boundary is missing; adding it manually.
        var endBoundary = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--", Encoding.ASCII.GetString(boundary)));
        var data = new byte[formData.Length + endBoundary.Length];
        Buffer.BlockCopy(formData, 0, data, 0, formData.Length);
        Buffer.BlockCopy(endBoundary, 0, data, formData.Length, endBoundary.Length);

        webRequest.uploadHandler = new UploadHandlerRaw(data);
        webRequest.SetRequestHeader("Content-Type", string.Concat("multipart/related; boundary=", Encoding.ASCII.GetString(boundary)));

        return webRequest;
    }

    private UnityWebRequest CreateSimpleUpload (UnityWebRequest webRequest)
    {
        var requestJson = JsonUtils.ToJsonPrivateCamel(RequestData);
        var requestData = Encoding.UTF8.GetBytes(requestJson);
        webRequest.uploadHandler = new UploadHandlerRaw(requestData);
        webRequest.SetRequestHeader("Content-Type", REQUEST_CONTENT_TYPE);

        return webRequest;
    }
}
