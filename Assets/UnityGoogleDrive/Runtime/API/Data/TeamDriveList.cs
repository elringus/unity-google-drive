using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// A list of Team Drives.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class TeamDriveList : ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#teamDriveList".
        /// </summary>
        public override string Kind => "drive#teamDriveList";
        /// <summary>
        /// The page token for the next page of Team Drives. This will be absent if the end
        /// of the Team Drives list has been reached. If the token is rejected for any reason,
        /// it should be discarded, and pagination should be restarted from the first page of results.
        /// </summary>
        public string NextPageToken { get; private set; }
        /// <summary>
        /// The list of Team Drives. If nextPageToken is populated, then this list may be
        /// incomplete and an additional page of results should be fetched.
        /// </summary>
        public List<TeamDrive> TeamDrives { get; private set; }
    }
}
