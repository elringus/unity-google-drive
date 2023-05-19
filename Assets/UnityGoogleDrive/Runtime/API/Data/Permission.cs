using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// A permission for a file. A permission grants a user, group, domain or the world access to a file or a folder hierarchy.
    /// Prototype: https://developers.google.com/drive/v3/reference/permissions#resource-representations.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Permission : ResourceData
    {
        public class TeamDrivePermissionDetailsData
        {
            /// <summary>
            /// Whether this permission is inherited. This field is always populated.
            /// This is an output-only field.
            /// </summary>
            public bool? Inherited { get; private set; }
            /// <summary>
            /// The ID of the item from which this permission is inherited.
            /// This is an output-only field and is only populated for members of the Team Drive.
            /// </summary>
            public string InheritedFrom { get; private set; }
            /// <summary>
            /// The primary role for this user. While new values may be added in the future,
            /// the following are currently possible:
            ///   - organizer
            ///   - writer
            ///   - commenter
            ///   - reader
            /// </summary>
            public string Role { get; private set; }
            /// <summary>
            /// The Team Drive permission type for this user.
            /// While new values may be added in future, the following are currently possible:
            ///   - file
            ///   - member
            /// </summary>
            public string TeamDrivePermissionType { get; private set; }
        }

        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#permission".
        /// </summary>
        public override string Kind => "drive#permission";
        /// <summary>
        /// The ID of this permission. This is a unique identifier for the grantee, and is
        /// published in User resources as permissionId.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// Whether the permission allows the file to be discovered through search.
        /// This is only applicable for permissions of type domain or anyone.
        /// </summary>
        public bool? AllowFileDiscovery { get; private set; }
        /// <summary>
        /// Whether the account associated with this permission has been deleted.
        /// This field only pertains to user and group permissions.
        /// </summary>
        public bool? Deleted { get; private set; }
        /// <summary>
        /// A displayable name for users, groups or domains.
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// The domain to which this permission refers.
        /// </summary>
        public string Domain { get; private set; }
        /// <summary>
        /// The email address of the user or group to which this permission refers.
        /// </summary>
        public string EmailAddress { get; private set; }
        /// <summary>
        /// The time at which this permission will expire (RFC 3339 date-time).
        /// Expiration times have the following restrictions:
        ///   - They can only be set on user and group permissions
        ///   - The time must be in the future
        ///   - The time cannot be more than a year in the future
        /// </summary>
        public DateTime? ExpirationTime { get; private set; }
        /// <summary>
        /// A link to the user's profile photo, if available.
        /// </summary>
        public string PhotoLink { get; private set; }
        /// <summary>
        /// The role granted by this permission.
        /// While new values may be supported in the future, the following are currently allowed:
        ///   - organizer
        ///   - owner
        ///   - writer
        ///   - commenter
        ///   - reader
        /// </summary>
        public string Role { get; private set; }
        /// <summary>
        /// Details of whether the permissions on this Team Drive item are inherited or directly
        /// on this item. This is an output-only field which is present only for Team Drive items.
        /// </summary>
        public List<TeamDrivePermissionDetailsData> TeamDrivePermissionDetails { get; private set; }
        /// <summary>
        /// The type of the grantee. Valid values are:
        ///   - user
        ///   - group
        ///   - domain
        ///   - anyone
        /// </summary>
        public string Type { get; private set; }
    }
}
