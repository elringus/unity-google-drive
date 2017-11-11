using UnityEngine.Networking;

/// <summary>
/// The <see cref="Data.File"/> resource collection of methods.
/// Prototype: https://developers.google.com/drive/v3/reference/files.
/// </summary>
public static class GoogleDriveFiles
{
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
    /// Lists or searches files.
    /// </summary>
    public static ListRequest List ()
    {
        return new ListRequest();
    }
}
