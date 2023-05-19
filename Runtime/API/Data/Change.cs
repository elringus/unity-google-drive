using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// A change to a file or Team Drive.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Change : ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#change".
        /// </summary>
        public override string Kind => "drive#change";
        /// <summary>
        /// The ID of the file which has changed.
        /// </summary>
        public string FileId { get; private set; }
        /// <summary>
        /// Whether the file or Team Drive has been removed from this list of changes,
        /// for example by deletion or loss of access.
        /// </summary>
        public bool? Removed { get; private set; }
        /// <summary>
        /// The time of this change (RFC 3339 date-time).
        /// </summary>
        public DateTime? Time { get; private set; }
        /// <summary>
        /// The updated state of the file. Present if the type is file and the file
        /// has not been removed from this list of changes.
        /// </summary>
        public File File { get; private set; }
        /// <summary>
        /// The type of the change. Possible values are file and teamDrive.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// The ID of the Team Drive associated with this change.
        /// </summary>
        public string TeamDriveId { get; private set; }
        /// <summary>
        /// The updated state of the Team Drive. Present if the type is teamDrive,
        /// the user is still a member of the Team Drive, and the Team Drive has not been removed.
        /// </summary>
        public TeamDrive TeamDrive { get; private set; }
    }
}
