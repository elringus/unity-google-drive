using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    /// <summary>
    /// The metadata for a file stored in Google Drive.
    /// Prototype: https://developers.google.com/drive/v3/reference/files.
    /// </summary>
    [Serializable]
    public class File : GoogleDriveData
    {
        /// <summary>
        /// Additional information about the content of the file. 
        /// These fields are never populated in responses.
        /// </summary>
        [Serializable]
        public class ContentHintsData
        {
            /// <summary>
            /// A thumbnail for the file. 
            /// This will only be used if Drive cannot generate a standard thumbnail.
            /// </summary>
            [Serializable]
            public class ThumbnailData
            {
                /// <summary>
                /// The thumbnail data encoded with URL-safe Base64 (RFC 4648 section 5).
                /// </summary>
                public string Image { get { return image; } set { image = value; } }
                /// <summary>
                /// The MIME type of the thumbnail.
                /// </summary>
                public string MimeType { get { return mimeType; } set { mimeType = value; } }

                [SerializeField] private string image = null;
                [SerializeField] private string mimeType = null;
            }

            /// <summary>
            /// Text to be indexed for the file to improve fullText queries. 
            /// This is limited to 128KB in length and may contain HTML elements.
            /// </summary>
            public string IndexableText { get { return indexableText; } set { indexableText = value; } }
            /// <summary>
            /// A thumbnail for the file. 
            /// This will only be used if Drive cannot generate a standard thumbnail.
            /// </summary>
            public ThumbnailData Thumbnail { get { return thumbnail; } }

            [SerializeField] private string indexableText = null;
            [SerializeField] private ThumbnailData thumbnail = null;
        }

        /// <summary>
        /// Additional metadata about image media, if available.
        /// </summary>
        [Serializable]
        public class ImageMediaMetadataData
        {
            /// <summary>
            /// Geographic location information stored in the image.
            /// </summary>
            [Serializable]
            public class LocationData
            {
                /// <summary>
                /// The altitude stored in the image.
                /// </summary>
                public double Altitude { get { return altitude; } }
                /// <summary>
                /// The latitude stored in the image.
                /// </summary>
                public double Latitude { get { return latitude; } }
                /// <summary>
                /// The longitude stored in the image.
                /// </summary>
                public double Longitude { get { return longitude; } }

                [SerializeField] private double altitude = -1;
                [SerializeField] private double latitude = -1;
                [SerializeField] private double longitude = -1;
            }

            /// <summary>
            /// The white balance mode used to create the photo.
            /// </summary>
            public string WhiteBalance { get { return whiteBalance; } }
            /// <summary>
            /// The date and time the photo was taken (EXIF DateTime).
            /// </summary>
            public string Time { get { return time; } }
            /// <summary>
            /// The distance to the subject of the photo, in meters.
            /// </summary>
            public int SubjectDistance { get { return subjectDistance; } }
            /// <summary>
            /// The type of sensor used to create the photo.
            /// </summary>
            public string Sensor { get { return sensor; } }
            /// <summary>
            /// The rotation in clockwise degrees from the image's original orientation.
            /// </summary>
            public int Rotation { get { return rotation; } }
            /// <summary>
            /// The metering mode used to create the photo.
            /// </summary>
            public string MeteringMode { get { return meteringMode; } }
            /// <summary>
            /// The smallest f-number of the lens at the focal length used to create the photo (APEX value).
            /// </summary>
            public float MaxApertureValue { get { return maxApertureValue; } }
            /// <summary>
            /// Geographic location information stored in the image.
            /// </summary>
            public LocationData Location { get { return location; } }
            /// <summary>
            /// The lens used to create the photo.
            /// </summary>
            public string Lens { get { return lens; } }
            /// <summary>
            /// The width of the image in pixels.
            /// </summary>
            public int Width { get { return width; } }
            /// <summary>
            /// The ISO speed used to create the photo.
            /// </summary>
            public int IsoSpeed { get { return isoSpeed; } }
            /// <summary>
            /// The focal length used to create the photo, in millimeters.
            /// </summary>
            public float FocalLength { get { return focalLength; } }
            /// <summary>
            /// Whether a flash was used to create the photo.
            /// </summary>
            public bool FlashUsed { get { return flashUsed; } }
            /// <summary>
            /// The length of the exposure, in seconds.
            /// </summary>
            public float ExposureTime { get { return exposureTime; } }
            /// <summary>
            /// The exposure mode used to create the photo.
            /// </summary>
            public string ExposureMode { get { return exposureMode; } }
            /// <summary>
            /// The exposure bias of the photo (APEX value).
            /// </summary>
            public float ExposureBias { get { return exposureBias; } }
            /// <summary>
            /// The color space of the photo.
            /// </summary>
            public string ColorSpace { get { return colorSpace; } }
            /// <summary>
            /// The model of the camera used to create the photo.
            /// </summary>
            public string CameraModel { get { return cameraModel; } }
            /// <summary>
            /// The make of the camera used to create the photo.
            /// </summary>
            public string CameraMake { get { return cameraMake; } }
            /// <summary>
            /// The aperture used to create the photo (f-number).
            /// </summary>
            public float Aperture { get { return aperture; } }
            /// <summary>
            /// The height of the image in pixels.
            /// </summary>
            public int Height { get { return height; } }

            [SerializeField] private string whiteBalance = null;
            [SerializeField] private string time = null;
            [SerializeField] private int subjectDistance = -1;
            [SerializeField] private string sensor = null;
            [SerializeField] private int rotation = -1;
            [SerializeField] private string meteringMode = null;
            [SerializeField] private float maxApertureValue = -1;
            [SerializeField] private LocationData location = null;
            [SerializeField] private string lens = null;
            [SerializeField] private int width = -1;
            [SerializeField] private int isoSpeed = -1;
            [SerializeField] private float focalLength = -1;
            [SerializeField] private bool flashUsed = false;
            [SerializeField] private float exposureTime = -1;
            [SerializeField] private string exposureMode = null;
            [SerializeField] private float exposureBias = -1;
            [SerializeField] private string colorSpace = null;
            [SerializeField] private string cameraModel = null;
            [SerializeField] private string cameraMake = null;
            [SerializeField] private float aperture = -1;
            [SerializeField] private int height = -1;
        }

        /// <summary>
        /// Capabilities the current user has on this file. Each capability corresponds to
        /// a fine-grained action that a user may take.
        /// </summary>
        [Serializable]
        public class CapabilitiesData
        {
            /// <summary>
            /// Whether the current user can modify the sharing settings for this file.
            /// </summary>
            public bool CanShare { get { return canShare; } }
            /// <summary>
            /// Whether the current user can rename this file.
            /// </summary>
            public bool CanRename { get { return canRename; } }
            /// <summary>
            /// Whether the current user can remove children from this folder. 
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool CanRemoveChildren { get { return canRemoveChildren; } }
            /// <summary>
            /// Whether the current user can read the Team Drive to which this file belongs.
            /// Only populated for Team Drive files.
            /// </summary>
            public bool CanReadTeamDrive { get { return canReadTeamDrive; } }
            /// <summary>
            /// Whether the current user can read the revisions resource of this file. 
            /// For a Team Drive item, whether revisions of non-folder descendants of this item, 
            /// or this item itself if it is not a folder, can be read.
            /// </summary>
            public bool CanReadRevisions { get { return canReadRevisions; } }
            /// <summary>
            /// Whether the current user can move this Team Drive item by changing its parent.
            /// Note that a request to change the parent for this item may still fail depending
            /// on the new parent that is being added. Only populated for Team Drive files.
            /// </summary>
            public bool CanMoveTeamDriveItem { get { return canMoveTeamDriveItem; } }
            /// <summary>
            /// Whether the current user can move this item into a Team Drive. If the item is
            /// in a Team Drive, this field is equivalent to canMoveTeamDriveItem.
            /// </summary>
            public bool CanMoveItemIntoTeamDrive { get { return canMoveItemIntoTeamDrive; } }
            /// <summary>
            /// Whether the current user can list the children of this folder. 
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool CanListChildren { get { return canListChildren; } }
            /// <summary>
            /// Whether the current user can edit this file.
            /// </summary>
            public bool CanEdit { get { return canEdit; } }
            /// <summary>
            /// Whether the current user can download this file.
            /// </summary>
            public bool CanDownload { get { return canDownload; } }
            /// <summary>
            /// Whether the current user can delete this file.
            /// </summary>
            public bool CanDelete { get { return canDelete; } }
            /// <summary>
            /// Whether the current user can copy this file. For a Team Drive item, whether the 
            /// current user can copy non-folder descendants of this item, or this item itself
            /// if it is not a folder.
            /// </summary>
            public bool CanCopy { get { return canCopy; } }
            /// <summary>
            /// Whether the current user can comment on this file.
            /// </summary>
            public bool CanComment { get { return canComment; } }
            /// <summary>
            /// Whether the current user can change whether viewers can copy the contents of this file.
            /// </summary>
            public bool CanChangeViewersCanCopyContent { get { return canChangeViewersCanCopyContent; } }
            /// <summary>
            /// Whether the current user can add children to this folder. 
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool CanAddChildren { get { return canAddChildren; } }
            /// <summary>
            /// Whether the current user can move this file to trash.
            /// </summary>
            public bool CanTrash { get { return canTrash; } }
            /// <summary>
            /// Whether the current user can restore this file from trash.
            /// </summary>
            public bool CanUntrash { get { return canUntrash; } }

            [SerializeField] private bool canShare = false;
            [SerializeField] private bool canRename = false;
            [SerializeField] private bool canRemoveChildren = false;
            [SerializeField] private bool canReadTeamDrive = false;
            [SerializeField] private bool canReadRevisions = false;
            [SerializeField] private bool canMoveTeamDriveItem = false;
            [SerializeField] private bool canMoveItemIntoTeamDrive = false;
            [SerializeField] private bool canListChildren = false;
            [SerializeField] private bool canEdit = false;
            [SerializeField] private bool canDownload = false;
            [SerializeField] private bool canDelete = false;
            [SerializeField] private bool canCopy = false;
            [SerializeField] private bool canComment = false;
            [SerializeField] private bool canChangeViewersCanCopyContent = false;
            [SerializeField] private bool canAddChildren = false;
            [SerializeField] private bool canTrash = false;
            [SerializeField] private bool canUntrash = false;
        }

        /// <summary>
        /// Additional metadata about video media. 
        /// This may not be available immediately upon upload.
        /// </summary>
        [Serializable]
        public class VideoMediaMetadataData
        {
            /// <summary>
            /// The duration of the video in milliseconds.
            /// </summary>
            public long DurationMillis { get { return durationMillis; } }
            /// <summary>
            /// The height of the video in pixels.
            /// </summary>
            public int Height { get { return height; } }
            /// <summary>
            /// The width of the video in pixels.
            /// </summary>
            public int Width { get { return width; } }

            [SerializeField] private long durationMillis = -1;
            [SerializeField] private int height = -1;
            [SerializeField] private int width = -1;
        }

        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#file".
        /// </summary>
        public override string Kind { get { return "drive#file"; } }
        /// <summary>
        /// The ID of the file.
        /// </summary>
        public string Id { get { return id; } set { id = value; } }
        /// <summary>
        /// The name of the file. This is not necessarily unique within a folder. Note that
        /// for immutable items such as the top level folders of Team Drives, My Drive root
        /// folder, and Application Data folder the name is constant.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// The original filename of the uploaded content if available, or else the original
        /// value of the name field. This is only available for files with binary content in Drive.
        /// </summary>
        public string OriginalFilename { get { return originalFilename; } set { originalFilename = value; } }
        /// <summary>
        /// A short description of the file.
        /// </summary>
        public string Description { get { return description; } set { description = value; } }
        /// <summary>
        /// The MIME type of the file. Drive will attempt to automatically detect an appropriate
        /// value from uploaded content if no value is provided. The value cannot be changed
        /// unless a new revision is uploaded. If a file is created with a Google Doc MIME
        /// type, the uploaded content will be imported if possible. The supported import
        /// formats are published in the About resource.
        /// </summary>
        public string MimeType { get { return mimeType; } set { mimeType = value; } }
        /// <summary>
        /// The full file extension extracted from the name field. May contain multiple concatenated
        /// extensions, such as "tar.gz". This is only available for files with binary content
        /// in Drive. This is automatically updated when the name field changes, however
        /// it is not cleared if the new name does not contain a valid extension.
        /// </summary>
        public string FullFileExtension { get { return fullFileExtension; } }
        /// <summary>
        /// The final component of fullFileExtension. This is only available for files with
        /// binary content in Drive.
        /// </summary>
        public string FileExtension { get { return fileExtension; } }
        /// <summary>
        /// The size of the file's content in bytes. This is only applicable to files with
        /// binary content in Drive.
        /// </summary>
        public long Size { get { return size; } }
        /// <summary>
        /// The MD5 checksum for the content of the file. This is only applicable to files 
        /// with binary content in Drive.
        /// </summary>
        public string Md5Checksum { get { return md5Checksum; } }
        /// <summary>
        /// A monotonically increasing version number for the file. This reflects every change 
        /// made to the file on the server, even those not visible to the user.
        /// </summary>
        public long Version { get { return version; } }
        /// <summary>
        /// The time at which the file was created (RFC 3339 date-time).
        /// </summary>
        public string CreatedTimeRaw { get { return createdTime; } set { createdTime = value; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.CreatedTimeRaw.
        /// </summary>
        public DateTime CreatedTime { get { return XmlConvert.ToDateTime(CreatedTimeRaw, XmlDateTimeSerializationMode.Utc); } set { XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// The last time the file was modified by anyone (RFC 3339 date-time). Note that
        /// setting modifiedTime will also update modifiedByMeTime for the user.
        /// </summary>
        public string ModifiedTimeRaw { get { return modifiedTime; } set { modifiedTime = value; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.ModifiedTimeRaw.
        /// </summary>
        public DateTime ModifiedTime { get { return XmlConvert.ToDateTime(ModifiedTimeRaw, XmlDateTimeSerializationMode.Utc); } set { XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// The IDs of the parent folders which contain the file. If not specified as part
        /// of a create request, the file will be placed directly in the My Drive folder.
        /// Update requests must use the addParents and removeParents parameters to modify the values.
        /// </summary>
        public List<string> Parents { get { return parents; } set { parents = value; } }
        /// <summary>
        /// The full list of permissions for the file. This is only available if the requesting
        /// user can share the file. Not populated for Team Drive files.
        /// </summary>
        public List<Permission> Permissions { get { return permissions; } set { permissions = value; } }
        /// <summary>
        /// A collection of arbitrary key-value pairs which are visible to all apps. Entries
        /// with null values are cleared in update and copy requests.
        /// </summary>
        public IDictionary<string, string> Properties { get { return properties; } set { properties = value; } }
        /// <summary>
        /// The number of storage quota bytes used by the file. This includes the head revision
        /// as well as previous revisions with keepForever enabled.
        /// </summary>
        public long QuotaBytesUsed { get { return quotaBytesUsed; } }
        /// <summary>
        /// Whether the file has been shared. Not populated for Team Drive files.
        /// </summary>
        public bool Shared { get { return shared; } }
        /// <summary>
        /// The time at which the file was shared with the user, if applicable (RFC 3339 date-time).
        /// </summary>
        public string SharedWithMeTimeRaw { get { return sharedWithMeTime; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.SharedWithMeTimeRaw.
        /// </summary>
        public DateTime SharedWithMeTime { get { return XmlConvert.ToDateTime(SharedWithMeTimeRaw, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// The user who shared the file with the requesting user, if applicable.
        /// </summary>
        public User SharingUser { get { return sharingUser; } }
        /// <summary>
        /// The list of spaces which contain the file. The currently supported values are
        /// 'drive', 'appDataFolder' and 'photos'.
        /// </summary>
        public List<string> Spaces { get { return spaces; } }
        /// <summary>
        /// Whether the user has starred the file.
        /// </summary>
        public bool Starred { get { return starred; } set { starred = value; } }
        /// <summary>
        /// ID of the Team Drive the file resides in.
        /// </summary>
        public string TeamDriveId { get { return teamDriveId; } }
        /// <summary>
        /// A short-lived link to the file's thumbnail, if available. Typically lasts on 
        /// the order of hours. Only populated when the requesting app can access the file's content.
        /// </summary>
        public string ThumbnailLink { get { return thumbnailLink; } }
        /// <summary>
        /// The thumbnail version for use in thumbnail cache invalidation.
        /// </summary>
        public long ThumbnailVersion { get { return thumbnailVersion; } }
        /// <summary>
        /// Whether the file has been trashed, either explicitly or from a trashed parent
        /// folder. Only the owner may trash a file, and other users cannot see files in the owner's trash.
        /// </summary>
        public bool Trashed { get { return trashed; } set { trashed = value; } }
        /// <summary>
        /// The time that the item was trashed (RFC 3339 date-time). Only populated for Team Drive files.
        /// </summary>
        public string TrashedTimeRaw { get { return trashedTime; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.TrashedTimeRaw.
        /// </summary>
        public DateTime TrashedTime { get { return XmlConvert.ToDateTime(TrashedTimeRaw, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// If the file has been explicitly trashed, the user who trashed it. Only populated for Team Drive files.
        /// </summary>
        public User TrashingUser { get { return trashingUser; } }
        /// <summary>
        /// Additional metadata about video media. This may not be available immediately upon upload.
        /// </summary>
        public VideoMediaMetadataData VideoMediaMetadata { get { return videoMediaMetadata; } set { videoMediaMetadata = value; } }
        /// <summary>
        /// Whether the file has been viewed by this user.
        /// </summary>
        public bool ViewedByMe { get { return viewedByMe; } set { viewedByMe = value; } }
        /// <summary>
        /// The last time the file was viewed by the user (RFC 3339 date-time).
        /// </summary>
        public string ViewedByMeTimeRaw { get { return viewedByMeTime; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.ViewedByMeTimeRaw.
        /// </summary>
        public DateTime ViewedByMeTime { get { return XmlConvert.ToDateTime(ViewedByMeTimeRaw, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// Whether users with only reader or commenter permission can copy the file's content.
        /// This affects copy, download, and print operations.
        /// </summary>
        public bool ViewersCanCopyContent { get { return viewersCanCopyContent; } set { viewersCanCopyContent = value; } }
        /// <summary>
        /// A link for downloading the content of the file in a browser. This is only available
        /// for files with binary content in Drive.
        /// </summary>
        public string WebContentLink { get { return webContentLink; } }
        /// <summary>
        /// A link for opening the file in a relevant Google editor or viewer in a browser.
        /// </summary>
        public string WebViewLink { get { return webViewLink; } }
        /// <summary>
        /// Whether users with only writer permission can modify the file's permissions.
        /// Not populated for Team Drive files.
        /// </summary>
        public bool WritersCanShare { get { return writersCanShare; } set { writersCanShare = value; } }
        /// <summary>
        /// List of permission IDs for users with access to this file.
        /// </summary>
        public List<string> PermissionIds { get { return permissionIds; } }
        /// <summary>
        /// The owners of the file. Currently, only certain legacy files may have more than
        /// one owner. Not populated for Team Drive files.
        /// </summary>
        public List<User> Owners { get { return owners; } }
        /// <summary>
        /// The ID of the file's head revision. This is currently only available for files
        /// with binary content in Drive.
        /// </summary>
        public string HeadRevisionId { get { return headRevisionId; } }
        /// <summary>
        /// Capabilities the current user has on this file. Each capability corresponds to
        /// a fine-grained action that a user may take.
        /// </summary>
        public CapabilitiesData Capabilities { get { return capabilities; } }
        /// <summary>
        /// Additional information about the content of the file. These fields are never
        /// populated in responses.
        /// </summary>
        public ContentHintsData ContentHints { get { return contentHints; } }
        /// <summary>
        /// Whether the file has been explicitly trashed, as opposed to recursively trashed
        /// from a parent folder.
        /// </summary>
        public bool ExplicitlyTrashed { get { return explicitlyTrashed; } set { explicitlyTrashed = value; } }
        /// <summary>
        /// The color for a folder as an RGB hex string. The supported colors are published
        /// in the folderColorPalette field of the About resource. If an unsupported color
        /// is specified, the closest color in the palette will be used instead.
        /// </summary>
        public string FolderColorRgb { get { return folderColorRgb; } set { folderColorRgb = value; } }
        /// <summary>
        /// Whether any users are granted file access directly on this file. This field is
        /// only populated for Team Drive files.
        /// </summary>
        public bool HasAugmentedPermissions { get { return hasAugmentedPermissions; } }
        /// <summary>
        /// Whether this file has a thumbnail. This does not indicate whether the requesting
        /// app has access to the thumbnail. To check access, look for the presence of the
        /// thumbnailLink field.
        /// </summary>
        public bool HasThumbnail { get { return hasThumbnail; } }
        /// <summary>
        /// Whether the user owns the file. Not populated for Team Drive files.
        /// </summary>
        public bool OwnedByMe { get { return ownedByMe; } }
        /// <summary>
        /// A static, unauthenticated link to the file's icon.
        /// </summary>
        public string IconLink { get { return iconLink; } }
        /// <summary>
        /// Additional metadata about image media, if available.
        /// </summary>
        public ImageMediaMetadataData ImageMediaMetadata { get { return imageMediaMetadata; } }
        /// <summary>
        /// Whether the file was created or opened by the requesting app.
        /// </summary>
        public bool IsAppAuthorized { get { return isAppAuthorized; } }
        /// <summary>
        /// The last user to modify the file.
        /// </summary>
        public User LastModifyingUser { get { return lastModifyingUser; } }
        /// <summary>
        /// Whether the file has been modified by this user.
        /// </summary>
        public bool ModifiedByMe { get { return modifiedByMe; } }
        /// <summary>
        /// The last time the file was modified by the user (RFC 3339 date-time).
        /// </summary>
        public string ModifiedByMeTimeRaw { get { return modifiedByMeTime; } }
        /// <summary>
        /// System.DateTime representation of Google.Apis.Drive.v3.Data.File.ModifiedByMeTimeRaw.
        /// </summary>
        public DateTime ModifiedByMeTime { get { return XmlConvert.ToDateTime(ModifiedByMeTimeRaw, XmlDateTimeSerializationMode.Utc); } }
        /// <summary>
        /// A collection of arbitrary key-value pairs which are private to the requesting app. 
        /// Entries with null values are cleared in update and copy requests.
        /// </summary>
        public IDictionary<string, string> AppProperties { get { return appProperties; } }

        [SerializeField] private string id = null;
        [SerializeField] private string name = null;
        [SerializeField] private string originalFilename = null;
        [SerializeField] private string description = null;
        [SerializeField] private string mimeType = null;
        [SerializeField] private string fullFileExtension = null;
        [SerializeField] private string fileExtension = null;
        [SerializeField] private long size = -1;
        [SerializeField] private string md5Checksum = null;
        [SerializeField] private long version = -1;
        [SerializeField] private string createdTime = null;
        [SerializeField] private string modifiedTime = null;
        [SerializeField] private List<string> parents = null;
        [SerializeField] private List<Permission> permissions = null;
        [SerializeField] private IDictionary<string, string> properties = null;
        [SerializeField] private long quotaBytesUsed = -1;
        [SerializeField] private bool shared = false;
        [SerializeField] private string sharedWithMeTime = null;
        [SerializeField] private User sharingUser = null;
        [SerializeField] private List<string> spaces = null;
        [SerializeField] private bool starred = false;
        [SerializeField] private string teamDriveId = null;
        [SerializeField] private string thumbnailLink = null;
        [SerializeField] private long thumbnailVersion = -1;
        [SerializeField] private bool trashed = false;
        [SerializeField] private string trashedTime = null;
        [SerializeField] private User trashingUser = null;
        [SerializeField] private VideoMediaMetadataData videoMediaMetadata = null;
        [SerializeField] private bool viewedByMe = false;
        [SerializeField] private string viewedByMeTime = null;
        [SerializeField] private bool viewersCanCopyContent = false;
        [SerializeField] private string webContentLink = null;
        [SerializeField] private string webViewLink = null;
        [SerializeField] private bool writersCanShare = false;
        [SerializeField] private List<string> permissionIds = null;
        [SerializeField] private List<User> owners = null;
        [SerializeField] private string headRevisionId = null;
        [SerializeField] private CapabilitiesData capabilities = null;
        [SerializeField] private ContentHintsData contentHints = null;
        [SerializeField] private bool explicitlyTrashed = false;
        [SerializeField] private string folderColorRgb = null;
        [SerializeField] private bool hasAugmentedPermissions = false;
        [SerializeField] private bool hasThumbnail = false;
        [SerializeField] private bool ownedByMe = false;
        [SerializeField] private string iconLink = null;
        [SerializeField] private ImageMediaMetadataData imageMediaMetadata = null;
        [SerializeField] private bool isAppAuthorized = false;
        [SerializeField] private User lastModifyingUser = null;
        [SerializeField] private bool modifiedByMe = false;
        [SerializeField] private string modifiedByMeTime = null;
        [SerializeField] private IDictionary<string, string> appProperties = null;
    }
}
