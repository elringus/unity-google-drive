using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// The <see cref="Data.File"/> resource collection of methods.
    /// Prototype: https://developers.google.com/drive/v3/reference/files.
    /// </summary>
    public static class GoogleDriveFiles
    {
        /// <summary>
        /// Creates a copy of a file and applies any requested updates with patch semantics.
        /// Response data will contain copied <see cref="Data.File"/>.
        /// </summary>
        public class CopyRequest : GoogleDriveUploadRequest<Data.File, Data.File>
        {
            /// <summary>
            /// Whether to ignore the domain's default visibility settings for the created file.
            /// Domain administrators can choose to make all uploaded files visible to the domain
            /// by default; this parameter bypasses that behavior for the request. Permissions
            /// are still inherited from parent folders.
            /// </summary>
            [QueryParameter] public bool? IgnoreDefaultVisibility { get; set; }
            /// <summary>
            /// Whether to set the 'keepForever' field in the new head revision. This is only
            /// applicable to files with binary content in Drive.
            /// </summary>
            [QueryParameter] public bool? KeepRevisionForever { get; set; }
            /// <summary>
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            /// </summary>
            [QueryParameter] public string OcrLanguage { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }

            public CopyRequest (Data.File file) : base($@"https://www.googleapis.com/drive/v3/files/{file.Id}/copy",
                UnityWebRequest.kHttpVerbPOST, file)
            {
                // API don't like receiving ID in the request body.
                file.Id = null;
            }
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        public class CreateRequest : GoogleDriveUploadRequest<Data.File, Data.File>
        {
            /// <summary>
            /// Whether to ignore the domain's default visibility settings for the created file.
            /// Domain administrators can choose to make all uploaded files visible to the domain
            /// by default; this parameter bypasses that behavior for the request. Permissions
            /// are still inherited from parent folders.
            /// </summary>
            [QueryParameter] public bool? IgnoreDefaultVisibility { get; set; }
            /// <summary>
            /// Whether to set the 'keepForever' field in the new head revision. This is only
            /// applicable to files with binary content in Drive.
            /// </summary>
            [QueryParameter] public bool? KeepRevisionForever { get; set; }
            /// <summary>
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            /// </summary>
            [QueryParameter] public string OcrLanguage { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether to use the uploaded content as indexable text.
            /// </summary>
            [QueryParameter] public bool? UseContentAsIndexableText { get; set; }

            public CreateRequest (Data.File file, string uploadMimeType = null) : base(file.Content != null ? @"https://www.googleapis.com/upload/drive/v3/files" :
                @"https://www.googleapis.com/drive/v3/files", UnityWebRequest.kHttpVerbPOST, file, file.Content, uploadMimeType ?? file.MimeType) { }
        }

        /// <summary>
        /// Creates a new file and upload file's data in a resumable fashion.
        /// </summary>
        public class ResumableCreateRequest : GoogleDriveResumableUploadRequest<Data.File>
        {
            /// <summary>
            /// Whether to ignore the domain's default visibility settings for the created file.
            /// Domain administrators can choose to make all uploaded files visible to the domain
            /// by default; this parameter bypasses that behavior for the request. Permissions
            /// are still inherited from parent folders.
            /// </summary>
            [QueryParameter] public bool? IgnoreDefaultVisibility { get; set; }
            /// <summary>
            /// Whether to set the 'keepForever' field in the new head revision. This is only
            /// applicable to files with binary content in Drive.
            /// </summary>
            [QueryParameter] public bool? KeepRevisionForever { get; set; }
            /// <summary>
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            /// </summary>
            [QueryParameter] public string OcrLanguage { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether to use the uploaded content as indexable text.
            /// </summary>
            [QueryParameter] public bool? UseContentAsIndexableText { get; set; }

            public ResumableCreateRequest (Data.File file, string resumableSessionUri = null, string uploadMimeType = null) : base(@"https://www.googleapis.com/upload/drive/v3/files",
                UnityWebRequest.kHttpVerbPOST, file, file.Content, uploadMimeType ?? file.MimeType, resumableSessionUri) { }
        }

        /// <summary>
        /// Permanently deletes a file owned by the user without moving it to the trash.
        /// If the file belongs to a Team Drive the user must be an organizer on the parent.
        /// If the target is a folder, all descendants owned by the user are also deleted.
        /// </summary>
        public class DeleteRequest : GoogleDriveRequest<string>
        {
            /// <summary>
            /// Whether the requesting application supports Team Drives. (Default: false)
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }

            public DeleteRequest (string fileId)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), UnityWebRequest.kHttpVerbDELETE) { }
        }

        /// <summary>
        /// Permanently deletes all of the user's trashed files.
        /// </summary>
        public class EmptyTrashRequest : GoogleDriveRequest<string>
        {
            public EmptyTrashRequest ()
                : base(@"https://www.googleapis.com/drive/v3/files/trash", UnityWebRequest.kHttpVerbDELETE) { }
        }

        /// <summary>
        /// Exports a Google Doc to the requested MIME type and returns the exported content.
        /// Please note that the exported content is limited to 10MB.
        /// </summary>
        public sealed class ExportRequest : GoogleDriveRequest<Data.File>
        {
            /// <summary>
            /// The MIME type of the format requested for this export.
            /// </summary>
            [QueryParameter] public string MimeType { get; private set; }

            public ExportRequest (string fileId, string mimeType)
                : base($@"https://www.googleapis.com/drive/v3/files/{fileId}/export", UnityWebRequest.kHttpVerbGET)
            {
                MimeType = mimeType;
                ResponseData = new Data.File { Id = fileId };
            }

            protected override void HandleResponseData (DownloadHandler downloadHandler)
            {
                if (IsError) base.HandleResponseData(downloadHandler);
                else ResponseData.Content = downloadHandler.data;
            }
        }

        /// <summary>
        /// Generates a set of file IDs which can be provided in create requests.
        /// </summary>
        public class GenerateIdsRequest : GoogleDriveRequest<Data.GeneratedIds>
        {
            /// <summary>
            /// The number of IDs to return.
            /// </summary>
            [QueryParameter] public int? Count { get; private set; }
            /// <summary>
            /// The space in which the IDs can be used to create new files.
            /// Supported values are 'drive' and 'appDataFolder'.
            /// </summary>
            [QueryParameter] public string Space { get; private set; }

            public GenerateIdsRequest (int? count = null, string space = null)
                : base(@"https://www.googleapis.com/drive/v3/files/generateIds", UnityWebRequest.kHttpVerbGET)
            {
                Count = count;
                Space = space;
            }
        }

        /// <summary>
        /// Gets a file's metadata by ID.
        /// </summary>
        public class GetRequest : GoogleDriveRequest<Data.File>
        {
            /// <summary>
            /// Whether the requesting application supports Team Drives. (Default: false)
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }

            public GetRequest (string fileId)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), UnityWebRequest.kHttpVerbGET)
            {
                Debug.Assert(!string.IsNullOrEmpty(fileId), "Missing file ID.");
            }
        }

        /// <summary>
        /// Downloads a file's content by ID.
        /// </summary>
        public sealed class DownloadRequest : GetRequest
        {
            /// <summary>
            /// Portion of the file's content to download (byte range). Will download the full file when null (default).
            /// For more info see <see href="https://developers.google.com/drive/api/v3/manage-downloads#partial_download"/>.
            /// </summary>
            public RangeInt? DownloadRange { get; private set; }

            /// <summary>
            /// Whether the user is acknowledging the risk of downloading known malware or other abusive files.
            /// </summary>
            [QueryParameter] public bool? AcknowledgeAbuse { get; set; }

            public DownloadRequest (string fileId, RangeInt? downloadRange = null) : base(fileId)
            {
                Alt = "media";
                ResponseData = new Data.File() { Id = fileId };
                DownloadRange = downloadRange;
            }

            public DownloadRequest (Data.File file, RangeInt? downloadRange = null) : this(file.Id, downloadRange)
            {
                ResponseData = file;
            }

            protected override UnityWebRequest CreateWebRequest ()
            {
                var webRequest = base.CreateWebRequest();
                if (DownloadRange.HasValue)
                    webRequest.SetRequestHeader("Range",
                        $"bytes={DownloadRange.Value.start}-{DownloadRange.Value.end}");
                return webRequest;
            }

            protected override void HandleResponseData (DownloadHandler downloadHandler)
            {
                if (IsError) base.HandleResponseData(downloadHandler);
                else ResponseData.Content = downloadHandler.data;
            }
        }

        /// <summary>
        /// Downloads a file's content by ID and creates an <see cref="AudioClip"/> based on the retrieved data.
        /// Using this class significantly reduces memory reallocation compared to downloading raw bytes and creating an audio clip manually in script.
        /// </summary>
        public sealed class DownloadAudioRequest : GoogleDriveRequest<Data.AudioFile>
        {
            /// <summary>
            /// Whether the requesting application supports Team Drives. (Default: false)
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether the user is acknowledging the risk of downloading known malware or other abusive files.
            /// </summary>
            [QueryParameter] public bool? AcknowledgeAbuse { get; set; }

            /// <summary>
            /// The type of audio encoding for the downloaded audio clip.
            /// </summary>
            public AudioType AudioType { get; private set; }

            public DownloadAudioRequest (string fileId, AudioType audioType)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), UnityWebRequest.kHttpVerbGET)
            {
                Alt = "media";
                AudioType = audioType;
                ResponseData = new Data.AudioFile() { Id = fileId };
            }

            protected override UnityWebRequest CreateWebRequest ()
            {
                var webRequest = UnityWebRequestMultimedia.GetAudioClip(Uri, AudioType);
                SetAuthorizationHeader(webRequest);
                SetQueryPayload(webRequest);
                return webRequest;
            }

            protected override void HandleResponseData (DownloadHandler downloadHandler)
            {
                if (IsError) base.HandleResponseData(downloadHandler);
                else
                {
                    ResponseData.Content = downloadHandler.data;
                    ResponseData.AudioClip = DownloadHandlerAudioClip.GetContent(WebRequest);
                }
            }
        }

        /// <summary>
        /// Downloads a file's content by ID and creates a <see cref="Texture2D"/> based on the retrieved data.
        /// Using this class significantly reduces memory reallocation compared to downloading raw bytes and creating a texture manually in script.
        /// </summary>
        public sealed class DownloadTextureRequest : GoogleDriveRequest<Data.TextureFile>
        {
            /// <summary>
            /// Whether the requesting application supports Team Drives. (Default: false)
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether the user is acknowledging the risk of downloading known malware or other abusive files.
            /// </summary>
            [QueryParameter] public bool? AcknowledgeAbuse { get; set; }

            /// <summary>
            /// If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.
            /// </summary>
            public bool NonReadable { get; set; }

            public DownloadTextureRequest (string fileId, bool nonReadable = false)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), UnityWebRequest.kHttpVerbGET)
            {
                Alt = "media";
                NonReadable = nonReadable;
                ResponseData = new Data.TextureFile() { Id = fileId };
            }

            protected override UnityWebRequest CreateWebRequest ()
            {
                var webRequest = UnityWebRequestTexture.GetTexture(Uri, NonReadable);
                SetAuthorizationHeader(webRequest);
                SetQueryPayload(webRequest);
                return webRequest;
            }

            protected override void HandleResponseData (DownloadHandler downloadHandler)
            {
                if (IsError) base.HandleResponseData(downloadHandler);
                {
                    ResponseData.Content = downloadHandler.data;
                    ResponseData.Texture = DownloadHandlerTexture.GetContent(WebRequest);
                }
            }
        }

        /// <summary>
        /// Lists or searches files.
        /// </summary>
        public class ListRequest : GoogleDriveRequest<Data.FileList>
        {
            /// <summary>
            /// Comma-separated list of bodies of items (files/documents) to which the query
            /// applies. Supported bodies are 'user', 'domain', 'teamDrive' and 'allTeamDrives'.
            /// 'allTeamDrives' must be combined with 'user'; all other values must be used in
            /// isolation. Prefer 'user' or 'teamDrive' to 'allTeamDrives' for efficiency.
            /// </summary>
            [QueryParameter] public string Corpora { get; set; }
            /// <summary>
            /// Whether Team Drive items should be included in results.
            /// </summary>
            [QueryParameter] public bool? IncludeTeamDriveItems { get; set; }
            /// <summary>
            /// A comma-separated list of sort keys. Valid keys are 'createdTime', 'folder',
            /// 'modifiedByMeTime', 'modifiedTime', 'name', 'name_natural', 'quotaBytesUsed',
            /// 'recency', 'sharedWithMeTime', 'starred', and 'viewedByMeTime'.
            /// Each key sorts ascending by default, but may be reversed with the 'desc' modifier.
            /// Example usage: ?orderBy=folder,modifiedTime desc,name.
            /// Please note that there is a current limitation for users with approximately
            /// one million files in which the requested sort order is ignored.
            /// </summary>
            [QueryParameter] public string OrderBy { get; set; }
            /// <summary>
            /// The maximum number of files to return per page. Partial or empty result pages
            /// are possible even before the end of the files list has been reached.
            /// Acceptable values are 1 to 1000, inclusive. (Default: 100)
            /// </summary>
            [QueryParameter] public int? PageSize { get; set; }
            /// <summary>
            /// The token for continuing a previous list request on the next page.
            /// This should be set to the value of 'nextPageToken' from the previous response.
            /// </summary>
            [QueryParameter] public string PageToken { get; set; }
            /// <summary>
            /// A query for filtering the file results.
            /// See <see href="https://developers.google.com/drive/v3/web/search-parameters"/> for the supported syntax.
            /// </summary>
            [QueryParameter] public string Q { get; set; }
            /// <summary>
            /// A comma-separated list of spaces to query within the corpus.
            /// Supported values are 'drive', 'appDataFolder' and 'photos'.
            /// </summary>
            [QueryParameter] public string Spaces { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// ID of Team Drive to search.
            /// </summary>
            [QueryParameter] public string TeamDriveId { get; set; }

            public ListRequest ()
                : base(@"https://www.googleapis.com/drive/v3/files", UnityWebRequest.kHttpVerbGET) { }
        }

        /// <summary>
        /// Subscribes to changes to a file.
        /// </summary>
        public class WatchRequest : GoogleDriveUploadRequest<Data.Channel, Data.Channel>
        {
            /// <summary>
            /// Whether the user is acknowledging the risk of downloading known malware or other
            /// abusive files. This is only applicable when alt=media.
            /// </summary>
            [QueryParameter] public bool? AcknowledgeAbuse { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }

            public WatchRequest (string fileId, Data.Channel channel) : base($@"https://www.googleapis.com/drive/v3/files/{fileId}/watch",
                UnityWebRequest.kHttpVerbPOST, channel) { }
        }

        /// <summary>
        /// Updates a file's metadata and/or content with patch semantics.
        /// </summary>
        public class UpdateRequest : GoogleDriveUploadRequest<Data.File, Data.File>
        {
            /// <summary>
            /// A comma-separated list of parent IDs to add.
            /// </summary>
            [QueryParameter] public string AddParents { get; set; }
            /// <summary>
            /// Whether to set the 'keepForever' field in the new head revision. This is only
            /// applicable to files with binary content in Drive.
            /// </summary>
            [QueryParameter] public bool? KeepRevisionForever { get; set; }
            /// <summary>
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            /// </summary>
            [QueryParameter] public string OcrLanguage { get; set; }
            /// <summary>
            /// A comma-separated list of parent IDs to remove.
            /// </summary>
            [QueryParameter] public string RemoveParents { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether to use the uploaded content as indexable text.
            /// </summary>
            [QueryParameter] public bool? UseContentAsIndexableText { get; set; }

            public UpdateRequest (string fileId, Data.File file, string uploadMimeType = null) : base(file.Content != null ? string.Concat(@"https://www.googleapis.com/upload/drive/v3/files/", fileId) :
                string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), "PATCH", file, file.Content, uploadMimeType ?? file.MimeType) { }
        }

        /// <summary>
        /// Updates a file's metadata and/or content with patch semantics and uploads the data in a resumable fashion.
        /// </summary>
        public class ResumableUpdateRequest : GoogleDriveResumableUploadRequest<Data.File>
        {
            /// <summary>
            /// A comma-separated list of parent IDs to add.
            /// </summary>
            [QueryParameter] public string AddParents { get; set; }
            /// <summary>
            /// Whether to set the 'keepForever' field in the new head revision. This is only
            /// applicable to files with binary content in Drive.
            /// </summary>
            [QueryParameter] public bool? KeepRevisionForever { get; set; }
            /// <summary>
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            /// </summary>
            [QueryParameter] public string OcrLanguage { get; set; }
            /// <summary>
            /// A comma-separated list of parent IDs to remove.
            /// </summary>
            [QueryParameter] public string RemoveParents { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// Whether to use the uploaded content as indexable text.
            /// </summary>
            [QueryParameter] public bool? UseContentAsIndexableText { get; set; }

            public ResumableUpdateRequest (string fileId, Data.File file, string resumableSessionUri = null, string uploadMimeType = null)
                : base(string.Concat(@"https://www.googleapis.com/upload/drive/v3/files/", fileId), "PATCH", file, file.Content, uploadMimeType ?? file.MimeType, resumableSessionUri) { }
        }

        /// <summary>
        /// Creates a copy of a file and applies any requested updates with patch semantics.
        /// Response data will contain copied <see cref="Data.File"/>.
        /// </summary>
        /// <param name="file">The file to copy. Ensure it has a valid <see cref="Data.File.Id"/>.</param>
        public static CopyRequest Copy (Data.File file)
        {
            return new CopyRequest(file);
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="file">The file to create. Provide <see cref="Data.File.Content"/> to upload the content of the file.</param>
        /// <param name="uploadMimeType">When the uploaded content differs from the target type (Eg when uploading plain text to create a google document), specify the uploaded content MIME type here.</param>
        public static CreateRequest Create (Data.File file, string uploadMimeType = null)
        {
            return new CreateRequest(file, uploadMimeType);
        }

        /// <summary>
        /// Creates a new file and (optionally) uploads the file's content in a resumable fashion.
        /// In case the upload is interrupted get <see cref="GoogleDriveResumableUploadRequest{TRequest}.ResumableSessionUri"/> property of the failed request and start a new one.
        /// In case you wish to manually upload the file's data (for example using a chunked transfer), don't set <see cref="Data.File.Content"/>, so the request will just initiate
        /// a new resumable upload session. You can then use the returned session URI to manually upload the data. For more info see <see href="https://developers.google.com/drive/api/v3/resumable-upload#upload-resumable"/>.
        /// </summary>
        /// <param name="file">The file to create. Provide <see cref="Data.File.Content"/> to upload the content of the file.</param>
        /// <param name="resumableSessionUri">Session URI to resume previously unfinished upload. Will upload from start when not provided.</param>
        /// <param name="uploadMimeType">When the uploaded content differs from the target type (Eg when uploading plain text to create a google document), specify the uploaded content MIME type here.</param>
        public static ResumableCreateRequest CreateResumable (Data.File file, string resumableSessionUri = null, string uploadMimeType = null)
        {
            return new ResumableCreateRequest(file, resumableSessionUri, uploadMimeType);
        }

        /// <summary>
        /// Permanently deletes a file owned by the user without moving it to the trash.
        /// If the file belongs to a Team Drive the user must be an organizer on the parent.
        /// If the target is a folder, all descendants owned by the user are also deleted.
        /// </summary>
        /// <param name="fileId">The ID of the file to delete.</param>
        public static DeleteRequest Delete (string fileId)
        {
            return new DeleteRequest(fileId);
        }

        /// <summary>
        /// Permanently deletes all of the user's trashed files.
        /// </summary>
        public static EmptyTrashRequest EmptyTrash ()
        {
            return new EmptyTrashRequest();
        }

        /// <summary>
        /// Exports a Google Doc to the requested MIME type and returns the exported content.
        /// Please note that the exported content is limited to 10MB.
        /// </summary>
        /// <param name="fileId">The ID of the file to export.</param>
        /// <param name="mimeType">The MIME type of the format requested for this export.</param>
        public static ExportRequest Export (string fileId, string mimeType)
        {
            return new ExportRequest(fileId, mimeType);
        }

        /// <summary>
        /// Generates a set of file IDs which can be provided in create requests.
        /// </summary>
        /// <param name="count">The number of IDs to return.</param>
        /// <param name="space">
        /// The space in which the IDs can be used to create new files.
        /// Supported values are 'drive' and 'appDataFolder'.
        /// </param>
        public static GenerateIdsRequest GenerateIds (int? count = null, string space = null)
        {
            return new GenerateIdsRequest(count, space);
        }

        /// <summary>
        /// Gets a file's metadata by ID.
        /// </summary>
        /// <param name="fileId">The ID of the file.</param>
        public static GetRequest Get (string fileId)
        {
            return new GetRequest(fileId);
        }

        /// <summary>
        /// Downloads a file's content by ID. Only <see cref="Data.File.Id"/> and <see cref="Data.File.Content"/> fields will be returned on success.
        /// For a partial download provide <paramref name="downloadRange"/> argument. More info: <see href="https://developers.google.com/drive/api/v3/manage-downloads#partial_download"/>.
        /// </summary>
        /// <param name="fileId">The ID of the file to download content for.</param>
        /// <param name="downloadRange">The portion of the file you want to download (a byte range). Will download the full file when null (default).</param>
        public static DownloadRequest Download (string fileId, RangeInt? downloadRange = null)
        {
            return new DownloadRequest(fileId, downloadRange);
        }

        /// <summary>
        /// Downloads a file's content by ID and creates an <see cref="AudioClip"/> based on the retrieved data.
        /// Using this method significantly reduces memory reallocation compared to downloading raw bytes and creating an audio clip manually in script.
        /// Be aware, that Unity support for encoding formats is limited depending on the platform.
        /// Eg: mp3 not supported on editor and standalone, ogg not available on WebGL, etc.
        /// </summary>
        /// <param name="fileId">The ID of the audio file to download.</param>
        /// <param name="audioType">The type of audio encoding for the downloaded audio clip.</param>
        public static DownloadAudioRequest DownloadAudio (string fileId, AudioType audioType)
        {
            return new DownloadAudioRequest(fileId, audioType);
        }

        /// <summary>
        /// Downloads a file's content by ID and creates a <see cref="Texture2D"/> based on the retrieved data.
        /// Using this class significantly reduces memory reallocation compared to downloading raw bytes and creating a texture manually in script.
        /// </summary>
        /// <param name="fileId">The ID of the texture file to download.</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        public static DownloadTextureRequest DownloadTexture (string fileId, bool nonReadable = false)
        {
            return new DownloadTextureRequest(fileId, nonReadable);
        }

        /// <summary>
        /// Lists or searches files.
        /// </summary>
        /// <param name="query">A query for filtering the file results.</param>
        /// <param name="fields">Selector specifying a subset of fields to include in the response.</param>
        /// <param name="spaces">A comma-separated list of spaces to query within the corpus.</param>
        /// <param name="pageToken">The token for continuing a previous list request on the next page.</param>
        public static ListRequest List (string query = null, List<string> fields = null, string spaces = "drive", string pageToken = null)
        {
            var listRequest = new ListRequest();
            listRequest.Q = query;
            listRequest.Fields = fields;
            listRequest.Spaces = spaces;
            listRequest.PageToken = pageToken;
            return listRequest;
        }

        /// <summary>
        /// Updates a file's metadata and/or content with patch semantics.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="file">Updated metadata of the file. Provide <see cref="Data.File.Content"/> to update the content of the file.</param>
        /// <param name="uploadMimeType">When the uploaded content differs from the target type (Eg when uploading plain text to create a google document), specify the uploaded content MIME type here.</param>
        public static UpdateRequest Update (string fileId, Data.File file, string uploadMimeType = null)
        {
            return new UpdateRequest(fileId, file, uploadMimeType);
        }

        /// <summary>
        /// Updates a file's metadata and content with patch semantics and upload the content in resumable fashion.
        /// In case the upload is interrupted get <see cref="GoogleDriveResumableUploadRequest{TRequest}.ResumableSessionUri"/> property of the failed request and start a new one.
        /// In case you wish to manually upload the file's data (for example using a chunked transfer), don't set <see cref="Data.File.Content"/>, so the request will just initiate
        /// a new resumable upload session. You can then use the returned session URI to manually upload the data. For more info see <see href="https://developers.google.com/drive/api/v3/resumable-upload#upload-resumable"/>.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="file">Updated metadata of the file. Provide <see cref="Data.File.Content"/> to update the content of the file.</param>
        /// <param name="resumableSessionUri">Session URI to resume previously unfinished upload. Will upload from start when not provided.</param>
        /// <param name="uploadMimeType">When the uploaded content differs from the target type (Eg when uploading plain text to create a google document), specify the uploaded content MIME type here.</param>
        public static ResumableUpdateRequest UpdateResumable (string fileId, Data.File file, string resumableSessionUri = null, string uploadMimeType = null)
        {
            return new ResumableUpdateRequest(fileId, file, resumableSessionUri, uploadMimeType);
        }

        /// <summary>
        /// Subscribes to changes to a file.
        /// </summary>
        /// <param name="fileId">The ID of the file to watch for.</param>
        /// <param name="channel">The body of the request.</param>
        public static WatchRequest Watch (string fileId, Data.Channel channel)
        {
            return new WatchRequest(fileId, channel);
        }
    }
}
