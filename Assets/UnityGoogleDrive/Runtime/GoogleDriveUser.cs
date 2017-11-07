using System;
using UnityEngine;

/// <summary>
/// Information about a Google Drive user.
/// </summary>
[Serializable]
public class GoogleDriveUser
{
    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#user".
    /// </summary>
    public string Kind { get { return "drive#permission"; } }
    /// <summary>
    /// The user's ID as visible in Permission resources.
    /// </summary>
    public virtual string PermissionId { get { return permissionId; } }
    /// <summary>
    /// A plain text displayable name for this user.
    /// </summary>
    public virtual string DisplayName { get { return displayName; } }
    /// <summary>
    /// The email address of the user. This may not be present in certain contexts if
    /// the user has not made their email address visible to the requester.
    /// </summary>
    public virtual string EmailAddress { get { return emailAddress; } }
    /// <summary>
    /// Whether this user is the requesting user.
    /// </summary>
    public virtual bool? Me { get { return me; } }
    /// <summary>
    /// A link to the user's profile photo, if available.
    /// </summary>
    public virtual string PhotoLink { get { return photoLink; } }

    [SerializeField] private string permissionId = null;
    [SerializeField] private string displayName = null;
    [SerializeField] private string emailAddress = null;
    [SerializeField] private bool? me = null;
    [SerializeField] private string photoLink = null;
}
