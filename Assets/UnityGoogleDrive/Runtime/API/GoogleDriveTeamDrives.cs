using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// The <see cref="Data.TeamDrive"/> collection of methods.
    /// Prototype: https://developers.google.com/drive/v3/reference/teamdrives.
    /// </summary>
    public static class GoogleDriveTeamDrives
    {
        /// <summary>
        /// Creates a new Team Drive.
        /// </summary>
        public class CreateRequest : GoogleDriveUploadRequest<Data.TeamDrive, Data.TeamDrive>
        {
            /// <summary>
            /// An ID, such as a random UUID, which uniquely identifies this user's request for
            /// idempotent creation of a Team Drive. A repeated request by the same user and
            /// with the same request ID will avoid creating duplicates by attempting to create
            /// the same Team Drive. If the Team Drive already exists a 409 error will be returned.
            /// </summary>
            [QueryParameter] public string RequestId { get; private set; }

            public CreateRequest (Data.TeamDrive teamDrive, string requestId)
                : base(@"https://www.googleapis.com/drive/v3/teamdrives", UnityWebRequest.kHttpVerbPOST, teamDrive)
            {
                RequestId = requestId;
            }
        }

        /// <summary>
        /// Permanently deletes a Team Drive for which the user is an organizer.
        /// The Team Drive cannot contain any un-trashed items.
        /// </summary>
        public class DeleteRequest : GoogleDriveRequest<string>
        {
            public DeleteRequest (string teamDriveId)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/teamdrives/", teamDriveId), UnityWebRequest.kHttpVerbDELETE) { }
        }

        /// <summary>
        /// Gets a Team Drive's metadata by ID.
        /// </summary>
        public class GetRequest : GoogleDriveRequest<Data.TeamDrive>
        {
            /// <summary>
            /// Whether the request should be treated as if it was issued by a domain administrator;
            /// if set to true, then the requester will be granted access if they are an administrator
            /// of the domain to which the Team Drive belongs.
            /// </summary>
            [QueryParameter] public bool? UseDomainAdminAccess { get; set; }

            public GetRequest (string teamDriveId)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/teamdrives/", teamDriveId), UnityWebRequest.kHttpVerbGET) { }
        }

        /// <summary>
        /// Lists the user's Team Drives.
        /// </summary>
        public class ListRequest : GoogleDriveRequest<Data.TeamDriveList>
        {
            /// <summary>
            /// Maximum number of Team Drives to return.
            /// </summary>
            [QueryParameter] public int? PageSize { get; set; }
            /// <summary>
            /// Page token for Team Drives.
            /// This should be set to the value of 'nextPageToken' from the previous response.
            /// </summary>
            [QueryParameter] public string PageToken { get; set; }
            /// <summary>
            /// Query string for searching Team Drives.
            /// See <see href="https://developers.google.com/drive/v3/web/search-parameters"/> for the supported syntax.
            /// </summary>
            [QueryParameter] public string Q { get; set; }
            /// <summary>
            /// Whether the request should be treated as if it was issued by a domain administrator;
            /// if set to true, then the requester will be granted access if they are an administrator
            /// of the domain to which the Team Drive belongs.
            /// </summary>
            [QueryParameter] public bool? UseDomainAdminAccess { get; set; }

            public ListRequest ()
                : base(@"https://www.googleapis.com/drive/v3/teamdrives", UnityWebRequest.kHttpVerbGET) { }
        }

        /// <summary>
        /// Updates a Team Drive's metadata.
        /// </summary>
        public class UpdateRequest : GoogleDriveUploadRequest<Data.TeamDrive, Data.TeamDrive>
        {
            public UpdateRequest (string teamDriveId, Data.TeamDrive teamDrive)
                : base(string.Concat(@"https://www.googleapis.com/drive/v3/teamdrives/", teamDriveId), "PATCH", teamDrive) { }
        }

        /// <summary>
        /// Creates a new Team Drive.
        /// </summary>
        /// <param name="teamDrive">The metadata of the Team Drive to create.</param>
        /// <param name="requestId">
        /// An ID, such as a random UUID, which uniquely identifies this user's request for
        /// idempotent creation of a Team Drive. A repeated request by the same user and
        /// with the same request ID will avoid creating duplicates by attempting to create
        /// the same Team Drive. If the Team Drive already exists a 409 error will be returned.
        /// </param>
        public static CreateRequest Create (Data.TeamDrive teamDrive, string requestId)
        {
            return new CreateRequest(teamDrive, requestId);
        }

        /// <summary>
        /// Permanently deletes a Team Drive for which the user is an organizer.
        /// The Team Drive cannot contain any untrashed items.
        /// </summary>
        /// <param name="teamDriveId">The ID of the Team Drive to delete.</param>
        public static DeleteRequest Delete (string teamDriveId)
        {
            return new DeleteRequest(teamDriveId);
        }

        /// <summary>
        /// Gets a Team Drive's metadata by ID.
        /// </summary>
        /// <param name="teamDriveId">The ID of the Team Drive.</param>
        public static GetRequest Get (string teamDriveId)
        {
            return new GetRequest(teamDriveId);
        }

        /// <summary>
        /// Lists the user's Team Drives.
        /// </summary>
        public static ListRequest List ()
        {
            return new ListRequest();
        }

        /// <summary>
        /// Updates a Team Drive's metadata.
        /// </summary>
        /// <param name="teamDriveId">ID of the Team Drive to update.</param>
        /// <param name="teamDrive">Updated metadata of the Team Drive.</param>
        public static UpdateRequest Update (string teamDriveId, Data.TeamDrive teamDrive)
        {
            return new UpdateRequest(teamDriveId, teamDrive);
        }
    }
}
