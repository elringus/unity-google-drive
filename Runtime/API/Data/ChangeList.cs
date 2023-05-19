using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// A list of changes for a user.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class ChangeList : ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#changeList".
        /// </summary>
        public override string Kind => "drive#changeList";
        /// <summary>
        /// The list of changes. If nextPageToken is populated, then this list may be incomplete
        /// and an additional page of results should be fetched.
        /// </summary>
        public List<Change> Changes { get; private set; }
        /// <summary>
        /// The starting page token for future changes. This will be present only if the
        /// end of the current changes list has been reached.
        /// </summary>
        public string NewStartPageToken { get; private set; }
        /// <summary>
        /// The page token for the next page of changes. This will be absent if the end of
        /// the changes list has been reached. If the token is rejected for any reason, it
        /// should be discarded, and pagination should be restarted from the first page of
        /// results.
        /// </summary>
        public string NextPageToken { get; private set; }
    }
}
