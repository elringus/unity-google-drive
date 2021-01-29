using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// Information about a Google Drive user.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class User : ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#user".
        /// </summary>
        public override string Kind => "drive#user";
        /// <summary>
        /// The user's ID as visible in Permission resources.
        /// </summary>
        public string PermissionId { get; private set; }
        /// <summary>
        /// A plain text displayable name for this user.
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// The email address of the user. This may not be present in certain contexts if
        /// the user has not made their email address visible to the requester.
        /// </summary>
        public string EmailAddress { get; private set; }
        /// <summary>
        /// Whether this user is the requesting user.
        /// </summary>
        public bool? Me { get; private set; }
        /// <summary>
        /// A link to the user's profile photo, if available.
        /// </summary>
        public string PhotoLink { get; private set; }
    }
}
