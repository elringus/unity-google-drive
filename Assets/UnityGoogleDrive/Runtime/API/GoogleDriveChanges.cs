using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// The <see cref="Data.Change"/> resource collection of methods.
    /// Prototype: https://developers.google.com/drive/v3/reference/changes.
    /// </summary>
    /// <remarks>
    /// If you wish to start tracking changes to the files in the user's drive, first
    /// execute the <see cref="GetStartPageTokenRequest"/> request and then use the returned
    /// token with the <see cref="ListRequest"/>. Returned <see cref="Data.Change"/> list will
    /// list the changes that happened after the <see cref="GetStartPageTokenRequest"/> request.
    /// </remarks>
    public static class GoogleDriveChanges
    {
        /// <summary>
        /// Gets the starting pageToken for listing future changes.
        /// </summary>
        public class GetStartPageTokenRequest : GoogleDriveRequest<Data.StartPageToken>
        {
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public virtual bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// The ID of the Team Drive for which the starting pageToken for listing future
            /// changes from that Team Drive will be returned.
            /// </summary>
            [QueryParameter] public virtual string TeamDriveId { get; set; }

            public GetStartPageTokenRequest ()
                : base(@"https://www.googleapis.com/drive/v3/changes/startPageToken", UnityWebRequest.kHttpVerbGET) { }
        }

        /// <summary>
        /// Lists the changes for a user or Team Drive.
        /// Will list the changes, that happened after <see cref="GetStartPageTokenRequest"/>.
        /// </summary>
        public class ListRequest : GoogleDriveRequest<Data.ChangeList>
        {
            /// <summary>
            /// The token for continuing a previous list request on the next page. This should
            /// be set to the value of 'nextPageToken' from the previous response or to the response
            /// from the getStartPageToken method.
            /// </summary>
            [QueryParameter] public string PageToken { get; private set; }
            /// <summary>
            /// Whether changes should include the file resource if the file is still accessible
            /// by the user at the time of the request, even when a file was removed from the
            /// list of changes and there will be no further change entries for this file.
            /// </summary>
            [QueryParameter] public bool? IncludeCorpusRemovals { get; set; }
            /// <summary>
            /// Whether to include changes indicating that items have been removed from the list
            /// of changes, for example by deletion or loss of access.
            /// </summary>
            [QueryParameter] public bool? IncludeRemoved { get; set; }
            /// <summary>
            /// Whether Team Drive files or changes should be included in results.
            /// </summary>
            [QueryParameter] public bool? IncludeTeamDriveItems { get; set; }
            /// <summary>
            /// The maximum number of changes to return per page.
            /// </summary>
            [QueryParameter] public int? PageSize { get; set; }
            /// <summary>
            /// Whether to restrict the results to changes inside the My Drive hierarchy. This
            /// omits changes to files such as those in the Application Data folder or shared
            /// files which have not been added to My Drive.
            /// </summary>
            [QueryParameter] public bool? RestrictToMyDrive { get; set; }
            /// <summary>
            /// A comma-separated list of spaces to query within the user corpus.
            /// Supported values are 'drive', 'appDataFolder' and 'photos'.
            /// </summary>
            [QueryParameter] public string Spaces { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// The Team Drive from which changes will be returned. If specified the change IDs
            /// will be reflective of the Team Drive; use the combined Team Drive ID and change
            /// ID as an identifier.
            /// </summary>
            [QueryParameter] public virtual string TeamDriveId { get; set; }

            public ListRequest (string pageToken)
                : base(@"https://www.googleapis.com/drive/v3/changes", UnityWebRequest.kHttpVerbGET)
            {
                PageToken = pageToken;
            }
        }

        /// <summary>
        /// Subscribes to changes for a user.
        /// </summary>
        public class WatchRequest : GoogleDriveUploadRequest<Data.Channel, Data.Channel>
        {
            /// <summary>
            /// The token for continuing a previous list request on the next page. This should
            /// be set to the value of 'nextPageToken' from the previous response or to the response
            /// from the getStartPageToken method.
            /// </summary>
            [QueryParameter] public string PageToken { get; private set; }
            /// <summary>
            /// Whether changes should include the file resource if the file is still accessible
            /// by the user at the time of the request, even when a file was removed from the
            /// list of changes and there will be no further change entries for this file.
            /// </summary>
            [QueryParameter] public bool? IncludeCorpusRemovals { get; set; }
            /// <summary>
            /// Whether to include changes indicating that items have been removed from the list
            /// of changes, for example by deletion or loss of access.
            /// </summary>
            [QueryParameter] public bool? IncludeRemoved { get; set; }
            /// <summary>
            /// Whether Team Drive files or changes should be included in results.
            /// </summary>
            [QueryParameter] public bool? IncludeTeamDriveItems { get; set; }
            /// <summary>
            /// The maximum number of changes to return per page.
            /// </summary>
            [QueryParameter] public int? PageSize { get; set; }
            /// <summary>
            /// Whether to restrict the results to changes inside the My Drive hierarchy. This
            /// omits changes to files such as those in the Application Data folder or shared
            /// files which have not been added to My Drive.
            /// </summary>
            [QueryParameter] public bool? RestrictToMyDrive { get; set; }
            /// <summary>
            /// A comma-separated list of spaces to query within the user corpus.
            /// Supported values are 'drive', 'appDataFolder' and 'photos'.
            /// </summary>
            [QueryParameter] public string Spaces { get; set; }
            /// <summary>
            /// Whether the requesting application supports Team Drives.
            /// </summary>
            [QueryParameter] public bool? SupportsTeamDrives { get; set; }
            /// <summary>
            /// The Team Drive from which changes will be returned. If specified the change IDs
            /// will be reflective of the Team Drive; use the combined Team Drive ID and change
            /// ID as an identifier.
            /// </summary>
            [QueryParameter] public virtual string TeamDriveId { get; set; }

            public WatchRequest (string pageToken, Data.Channel channel)
                : base(@"https://www.googleapis.com/drive/v3/changes/watch", UnityWebRequest.kHttpVerbPOST, channel)
            {
                PageToken = pageToken;
            }
        }

        /// <summary>
        /// Gets the starting pageToken for listing future changes.
        /// </summary>
        public static GetStartPageTokenRequest GetStartPageToken ()
        {
            return new GetStartPageTokenRequest();
        }

        /// <summary>
        /// Lists the changes for a user or Team Drive.
        /// Will list the changes, that happened after <see cref="GetStartPageToken"/>.
        /// </summary>
        /// <param name="pageToken">
        /// The token for continuing a previous list request on the next page. This should
        /// be set to the value of <see cref="Data.ChangeList.NextPageToken"/> from the previous
        /// response or to the response from the <see cref="GetStartPageToken"/> method.
        /// </param>
        public static ListRequest List (string pageToken)
        {
            return new ListRequest(pageToken);
        }

        /// <summary>
        /// Subscribes to changes for a user.
        /// </summary>
        public static WatchRequest Watch (string pageToken, Data.Channel channel)
        {
            return new WatchRequest(pageToken, channel);
        }
    }
}
