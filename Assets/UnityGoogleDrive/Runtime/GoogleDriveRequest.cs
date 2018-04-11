using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Property will be included in the query portion of the request URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryParameterAttribute : Attribute { }

    /// <summary>
    /// A request intended to communicate with the Google Drive API. 
    /// </summary>
    public abstract class GoogleDriveRequest : IDisposable
    {
        public abstract string Uri { get; protected set; }
        public abstract string Method { get; protected set; }
        public abstract float Progress { get; }
        public abstract bool IsRunning { get; }
        public abstract bool IsDone { get; protected set; }
        public abstract bool IsError { get; }
        public abstract string Error { get; protected set; }

        public abstract CustomYieldInstruction SendNonGeneric ();
        public abstract T GetResourceData<T> () where T : Data.ResourceData;
        public abstract void Abort ();
        public abstract void Dispose ();
    }

    /// <summary>
    /// A request intended to communicate with the Google Drive API. 
    /// Handles base networking and authorization flow.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response data.</typeparam>
    public class GoogleDriveRequest<TResponse> : GoogleDriveRequest
    {
        /// <summary>
        /// Event invoked when the request is done running.
        /// Make sure to check for <see cref="IsError"/> before using <see cref="ResponseData"/>.
        /// </summary>
        public event Action<TResponse> OnDone;

        /// <summary>
        /// The URI of the request.
        /// </summary>
        public override string Uri { get; protected set; }
        /// <summary>
        /// HTTP method of the request.
        /// </summary>
        public override string Method { get; protected set; }
        /// <summary>
        /// The response data of the request.
        /// Make sure to check for <see cref="IsDone"/> and <see cref="IsError"/> before using.
        /// </summary>
        public TResponse ResponseData { get; protected set; }
        /// <summary>
        /// Progress of the request execution, in 0.0 to 1.0 range.
        /// </summary>
        public override float Progress { get { return WebRequestYeild != null ? WebRequestYeild.progress : 0; } }
        /// <summary>
        /// Whether the request is currently executing.
        /// </summary>
        public override bool IsRunning { get { return YeildInstruction != null && !IsDone; } }
        /// <summary>
        /// Whether the request has finished executing and it's safe to use <see cref="ResponseData"/>.
        /// </summary>
        public override bool IsDone { get; protected set; }
        /// <summary>
        /// Whether the request has finished with errors.
        /// Use <see cref="Error"/> for error description.
        /// </summary>
        public override bool IsError { get { return !string.IsNullOrEmpty(Error); } }
        /// <summary>
        /// When <see cref="IsError"/> is true contains description of the occured error.
        /// </summary>
        public override string Error { get; protected set; }

        /// <summary>
        /// Used to alternate between returned content types.
        /// </summary>
        [QueryParameter] public string Alt { get; set; }
        /// <summary>
        /// Selector specifying a subset of fields to include in the response.
        /// Nested fields should be in the following format: field(nestedField1, nestedField2).
        /// </summary>
        [QueryParameter] public List<string> Fields { get; set; }
        /// <summary>
        /// Returns response with indentations and line breaks.
        /// </summary>
        [QueryParameter] public bool? PrettyPrint { get; set; }
        /// <summary>
        /// Allows to enforce per-user quotas from a server-side application even 
        /// in cases when the user's IP address is unknown. 
        /// </summary>
        [QueryParameter] public string QuotaUser { get; set; }
        /// <summary>
        /// IP address of the end user for whom the API call is being made.
        /// </summary>
        [QueryParameter] public string UserIp { get; set; }

        protected static GoogleDriveSettings Settings { get; private set; }
        protected static AuthController AuthController { get; private set; }

        protected UnityWebRequest WebRequest { get; private set; }
        protected AsyncOperation WebRequestYeild { get; private set; }
        protected GoogleDriveRequestYeildInstruction<TResponse> YeildInstruction { get; private set; }

        public GoogleDriveRequest (string uri, string method)
        {
            Uri = uri;
            Method = method;

            if (Settings == null) Settings = GoogleDriveSettings.LoadFromResources();
            if (AuthController == null) AuthController = new AuthController(Settings);
        }

        /// <summary>
        /// Begin communicating with the Google Drive API to execute the request.
        /// </summary>
        /// <returns>
        /// A yeild instruction indicating the progress/completion state of the request.
        /// Yield this object to wait until the request <see cref="IsDone"/> or use <see cref="OnDone"/> event.
        /// </returns>
        public GoogleDriveRequestYeildInstruction<TResponse> Send ()
        {
            if (!IsRunning)
            {
                YeildInstruction = new GoogleDriveRequestYeildInstruction<TResponse>(this);
                SendWebRequest();
            }
            return YeildInstruction;
        }

        public override CustomYieldInstruction SendNonGeneric ()
        {
            return Send();
        }

        /// <summary>
        /// If in progress, halts the request as soon as possible.
        /// </summary>
        public override void Abort ()
        {
            if (WebRequest != null && IsRunning)
                WebRequest.Abort();
        }

        /// <summary>
        /// Signals the request is no longer being used, and should clean up any resources it is using.
        /// </summary>
        public override void Dispose ()
        {
            if (WebRequest != null)
                WebRequest.Dispose();
        }

        public override T GetResourceData<T> ()
        {
            return ResponseData as T;
        }

        protected virtual UnityWebRequest CreateWebRequest ()
        {
            var webRequest = new UnityWebRequest(Uri, Method);
            SetAuthorizationHeader(webRequest);
            SetDefaultContentHeader(webRequest);
            SetQueryPayload(webRequest);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            return webRequest;
        }

        protected virtual void HandleResponseData (DownloadHandler downloadHandler)
        {
            var responseText = downloadHandler.text;
            if (!string.IsNullOrEmpty(responseText))
            {
                var apiError = JsonUtility.FromJson<GoogleDriveResponseError>(responseText);
                if (apiError.IsError) Error += apiError.Error.Message;
                if (!IsError) ResponseData = JsonUtils.FromJsonPrivateCamel<TResponse>(responseText);
            }
        }

        protected void SetAuthorizationHeader (UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Authorization", string.Format("Bearer {0}", AuthController.AccessToken));
        }

        protected void SetDefaultContentHeader (UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.REQUEST_CONTENT_TYPE);
        }

        protected void SetQueryPayload (UnityWebRequest webRequest)
        {
            webRequest.url = string.Concat(webRequest.url, "?", GenerateQueryString());
        }

        private void SendWebRequest ()
        {
            IsDone = false;

            if (WebRequest != null)
            {
                WebRequest.Abort();
                WebRequest.Dispose();
            }

            WebRequest = CreateWebRequest();
            WebRequest.SendWebRequest().completed += HandleWebRequestDone;
        }

        private void HandleWebRequestDone (AsyncOperation requestYeild)
        {
            if (WebRequest.responseCode == GoogleDriveSettings.UNAUTHORIZED_RESPONSE_CODE)
            {
                HandleUnauthorizedResponse();
                return;
            }

            Error = WebRequest.error;

            HandleResponseData(WebRequest.downloadHandler);

            if (IsError) Debug.LogError("UnityGoogleDrive: " + Error);

            IsDone = true;

            if (OnDone != null)
                OnDone.Invoke(ResponseData);

            WebRequest.Dispose();
        }

        private void HandleUnauthorizedResponse ()
        {
            AuthController.OnAccessTokenRefreshed += HandleAccessTokenRefreshed;
            AuthController.RefreshAccessToken();
        }

        private void HandleAccessTokenRefreshed (bool success)
        {
            AuthController.OnAccessTokenRefreshed -= HandleAccessTokenRefreshed;
            if (success) SendWebRequest();
        }

        /// <summary>
        /// Generates an HTML request query string using declared query properties.
        /// </summary>
        private string GenerateQueryString ()
        {
            // Get all query properties on the object.
            var properties = GetType().GetProperties()
                .Where(p => p.IsDefined(typeof(QueryParameterAttribute), false) && p.CanRead && p.GetValue(this, null) != null)
                .ToDictionary(p => ToFirstLower(p.Name), property => property.GetValue(this, null));

            // Get names for all IEnumerable properties (excl. string).
            var propertyNames = properties
                .Where(p => !(p.Value is string) && p.Value is IEnumerable)
                .Select(p => p.Key).ToList();

            // Concat all IEnumerable properties into a comma separated string.
            foreach (var propertyName in propertyNames)
            {
                var valueType = properties[propertyName].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[propertyName] as IEnumerable;
                    properties[propertyName] = string.Join(",", enumerable.Cast<string>().ToArray());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand.
            return string.Join("&", properties.Select(x => string.Concat(
                System.Uri.EscapeDataString(x.Key), "=",
                System.Uri.EscapeDataString(x.Value.ToString()))).ToArray());
        }

        /// <summary>
        /// Converts first letter of the string to lower case.
        /// </summary>
        private static string ToFirstLower (string content)
        {
            var firstChar = Char.ToLowerInvariant(content[0]);
            if (content.Length > 1) return string.Concat(firstChar, content.Substring(1));
            else return firstChar.ToString();
        }
    }
}
