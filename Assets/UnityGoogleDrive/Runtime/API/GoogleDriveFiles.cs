using UnityEngine.Networking;

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

        public CopyRequest (Data.File file) : base(string.Format(@"https://www.googleapis.com/drive/v3/files/{0}/copy", file.Id), 
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

        public CreateRequest (Data.File file) : base(file.Content != null ? @"https://www.googleapis.com/upload/drive/v3/files" : 
            @"https://www.googleapis.com/drive/v3/files", UnityWebRequest.kHttpVerbPOST, file, file.Content, file.MimeType) { }
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
    /// Gets a file's metadata by ID.
    /// </summary>
    public class GetRequest : GoogleDriveRequest<Data.File>
    {
        /// <summary>
        /// Whether the requesting application supports Team Drives. (Default: false) 
        /// </summary>
        [QueryParameter] public bool? SupportsTeamDrives { get; set; }

        public GetRequest (string fileId)
            : base(string.Concat(@"https://www.googleapis.com/drive/v3/files/", fileId), UnityWebRequest.kHttpVerbGET) { }
    }

    /// <summary>
    /// Downloads a file's content by ID.
    /// </summary>
    public class DownloadRequest : GetRequest
    {
        /// <summary>
        /// Whether the user is acknowledging the risk of downloading known malware or other abusive files. 
        /// </summary>
        [QueryParameter] public bool? AcknowledgeAbuse { get; set; }

        public DownloadRequest (string fileId) : base(fileId)
        {
            Alt = "media";
            ResponseData = new Data.File() { Id = fileId };
        }

        public DownloadRequest (Data.File file) : this(file.Id)
        {
            ResponseData = file;
        }

        protected override void HandleResponseData (DownloadHandler downloadHandler)
        {
            ResponseData.Content = downloadHandler.data;
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
        /// </summary>
        [QueryParameter] public int? PageSize { get; set; }
        /// <summary>
        /// The token for continuing a previous list request on the next page. 
        /// This should be set to the value of 'nextPageToken' from the previous response.
        /// </summary>
        [QueryParameter] public string PageToken { get; set; }
        /// <summary>
        /// A query for filtering the file results. 
        /// See the "Search for Files" guide for supported syntax.
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
    /// Creates a copy of a file and applies any requested updates with patch semantics.
    /// Response data will contain copied <see cref="Data.File"/>.
    /// </summary>
    /// <param name="fileId">The file to copy. Ensure it has a valid <see cref="Data.File.Id"/>.</param>
    public static CopyRequest Copy (Data.File file)
    {
        return new CopyRequest(file);
    }

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="fileId">
    /// The file to create. 
    /// Provide <see cref="Data.File.Content"/> to upload the content of the file.
    /// </param>
    public static CreateRequest Create (Data.File file)
    {
        return new CreateRequest(file);
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
    /// Gets a file's metadata by ID.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    public static GetRequest Get (string fileId)
    {
        return new GetRequest(fileId);
    }

    /// <summary>
    /// Downloads a file's content by ID.
    /// Only <see cref="Data.File.Id"/> and <see cref="Data.File.Content"/> fields will be returned on success.
    /// </summary>
    /// <param name="fileId">The ID of the file to download content for.</param>
    public static DownloadRequest Download (string fileId)
    {
        return new DownloadRequest(fileId);
    }

    /// <summary>
    /// Downloads a file's content by ID of the provided file.
    /// </summary>
    /// <param name="fileId">The file to download content for. File's <see cref="Data.File.Id"/> field must be valid.</param>
    public static DownloadRequest Download (Data.File file)
    {
        return new DownloadRequest(file);
    }

    /// <summary>
    /// Lists or searches files.
    /// </summary>
    public static ListRequest List ()
    {
        return new ListRequest();
    }
}
