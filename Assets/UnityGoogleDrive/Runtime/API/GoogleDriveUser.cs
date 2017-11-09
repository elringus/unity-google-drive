using UnityEngine;

/// <summary>
/// Information about a Google Drive user.
/// </summary>
public class GoogleDriveUser : GoogleDriveResource
{
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#user".
    /// </summary>
    public override string Kind { get { return "drive#user"; } }
    /// <summary>
    /// The user's ID as visible in Permission resources.
    /// </summary>
    public string PermissionId { get { return permissionId; } }
    /// <summary>
    /// A plain text displayable name for this user.
    /// </summary>
    public string DisplayName { get { return displayName; } }
    /// <summary>
    /// The email address of the user. This may not be present in certain contexts if
    /// the user has not made their email address visible to the requester.
    /// </summary>
    public string EmailAddress { get { return emailAddress; } }
    /// <summary>
    /// Whether this user is the requesting user.
    /// </summary>
    public bool? Me { get { return me; } }
    /// <summary>
    /// A link to the user's profile photo, if available.
    /// </summary>
    public string PhotoLink { get { return photoLink; } }

    [SerializeField] private string permissionId = null;
    [SerializeField] private string displayName = null;
    [SerializeField] private string emailAddress = null;
    [SerializeField] private bool? me = null;
    [SerializeField] private string photoLink = null;
}
