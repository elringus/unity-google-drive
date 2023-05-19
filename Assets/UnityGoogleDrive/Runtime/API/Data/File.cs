using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// The metadata for a file stored in Google Drive.
    /// Prototype: https://developers.google.com/drive/v3/reference/files#resource-representations.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class File : ResourceData
    {
        /// <summary>
        /// Additional information about the content of the file.
        /// These fields are never populated in responses.
        /// </summary>
        public class ContentHintsData
        {
            /// <summary>
            /// A thumbnail for the file.
            /// This will only be used if Drive cannot generate a standard thumbnail.
            /// </summary>
            public class ThumbnailData
            {
                /// <summary>
                /// The thumbnail data encoded with URL-safe Base64 (RFC 4648 section 5).
                /// </summary>
                public string Image { get; set; }
                /// <summary>
                /// The MIME type of the thumbnail.
                /// </summary>
                public string MimeType { get; set; }
            }

            /// <summary>
            /// Text to be indexed for the file to improve fullText queries.
            /// This is limited to 128KB in length and may contain HTML elements.
            /// </summary>
            public string IndexableText { get; set; }
            /// <summary>
            /// A thumbnail for the file.
            /// This will only be used if Drive cannot generate a standard thumbnail.
            /// </summary>
            public ThumbnailData Thumbnail { get; private set; }
        }

        /// <summary>
        /// Additional metadata about image media, if available.
        /// </summary>
        public class ImageMediaMetadataData
        {
            /// <summary>
            /// Geographic location information stored in the image.
            /// </summary>
            public class LocationData
            {
                /// <summary>
                /// The altitude stored in the image.
                /// </summary>
                public double? Altitude { get; private set; }
                /// <summary>
                /// The latitude stored in the image.
                /// </summary>
                public double? Latitude { get; private set; }
                /// <summary>
                /// The longitude stored in the image.
                /// </summary>
                public double? Longitude { get; private set; }
            }

            /// <summary>
            /// The white balance mode used to create the photo.
            /// </summary>
            public string WhiteBalance { get; private set; }
            /// <summary>
            /// The date and time the photo was taken (EXIF DateTime?).
            /// </summary>
            public string Time { get; private set; }
            /// <summary>
            /// The distance to the subject of the photo, in meters.
            /// </summary>
            public int? SubjectDistance { get; private set; }
            /// <summary>
            /// The type of sensor used to create the photo.
            /// </summary>
            public string Sensor { get; private set; }
            /// <summary>
            /// The rotation in clockwise degrees from the image's original orientation.
            /// </summary>
            public int? Rotation { get; private set; }
            /// <summary>
            /// The metering mode used to create the photo.
            /// </summary>
            public string MeteringMode { get; private set; }
            /// <summary>
            /// The smallest f-number of the lens at the focal length used to create the photo (APEX value).
            /// </summary>
            public float? MaxApertureValue { get; private set; }
            /// <summary>
            /// Geographic location information stored in the image.
            /// </summary>
            public LocationData Location { get; private set; }
            /// <summary>
            /// The lens used to create the photo.
            /// </summary>
            public string Lens { get; private set; }
            /// <summary>
            /// The width of the image in pixels.
            /// </summary>
            public int? Width { get; private set; }
            /// <summary>
            /// The ISO speed used to create the photo.
            /// </summary>
            public int? IsoSpeed { get; private set; }
            /// <summary>
            /// The focal length used to create the photo, in millimeters.
            /// </summary>
            public float? FocalLength { get; private set; }
            /// <summary>
            /// Whether a flash was used to create the photo.
            /// </summary>
            public bool? FlashUsed { get; private set; }
            /// <summary>
            /// The length of the exposure, in seconds.
            /// </summary>
            public float? ExposureTime { get; private set; }
            /// <summary>
            /// The exposure mode used to create the photo.
            /// </summary>
            public string ExposureMode { get; private set; }
            /// <summary>
            /// The exposure bias of the photo (APEX value).
            /// </summary>
            public float? ExposureBias { get; private set; }
            /// <summary>
            /// The color space of the photo.
            /// </summary>
            public string ColorSpace { get; private set; }
            /// <summary>
            /// The model of the camera used to create the photo.
            /// </summary>
            public string CameraModel { get; private set; }
            /// <summary>
            /// The make of the camera used to create the photo.
            /// </summary>
            public string CameraMake { get; private set; }
            /// <summary>
            /// The aperture used to create the photo (f-number).
            /// </summary>
            public float? Aperture { get; private set; }
            /// <summary>
            /// The height of the image in pixels.
            /// </summary>
            public int? Height { get; private set; }
        }

        /// <summary>
        /// Capabilities the current user has on this file. Each capability corresponds to
        /// a fine-grained action that a user may take.
        /// </summary>
        public class CapabilitiesData
        {
            /// <summary>
            /// Whether the current user can modify the sharing settings for this file.
            /// </summary>
            public bool? CanShare { get; private set; }
            /// <summary>
            /// Whether the current user can rename this file.
            /// </summary>
            public bool? CanRename { get; private set; }
            /// <summary>
            /// Whether the current user can remove children from this folder.
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool? CanRemoveChildren { get; private set; }
            /// <summary>
            /// Whether the current user can read the Team Drive to which this file belongs.
            /// Only populated for Team Drive files.
            /// </summary>
            public bool? CanReadTeamDrive { get; private set; }
            /// <summary>
            /// Whether the current user can read the revisions resource of this file.
            /// For a Team Drive item, whether revisions of non-folder descendants of this item,
            /// or this item itself if it is not a folder, can be read.
            /// </summary>
            public bool? CanReadRevisions { get; private set; }
            /// <summary>
            /// Whether the current user can move this Team Drive item by changing its parent.
            /// Note that a request to change the parent for this item may still fail depending
            /// on the new parent that is being added. Only populated for Team Drive files.
            /// </summary>
            public bool? CanMoveTeamDriveItem { get; private set; }
            /// <summary>
            /// Whether the current user can move this item into a Team Drive. If the item is
            /// in a Team Drive, this field is equivalent to canMoveTeamDriveItem.
            /// </summary>
            public bool? CanMoveItemIntoTeamDrive { get; private set; }
            /// <summary>
            /// Whether the current user can list the children of this folder.
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool? CanListChildren { get; private set; }
            /// <summary>
            /// Whether the current user can edit this file.
            /// </summary>
            public bool? CanEdit { get; private set; }
            /// <summary>
            /// Whether the current user can download this file.
            /// </summary>
            public bool? CanDownload { get; private set; }
            /// <summary>
            /// Whether the current user can delete this file.
            /// </summary>
            public bool? CanDelete { get; private set; }
            /// <summary>
            /// Whether the current user can copy this file. For a Team Drive item, whether the
            /// current user can copy non-folder descendants of this item, or this item itself
            /// if it is not a folder.
            /// </summary>
            public bool? CanCopy { get; private set; }
            /// <summary>
            /// Whether the current user can comment on this file.
            /// </summary>
            public bool? CanComment { get; private set; }
            /// <summary>
            /// Whether the current user can change whether viewers can copy the contents of this file.
            /// </summary>
            public bool? CanChangeViewersCanCopyContent { get; private set; }
            /// <summary>
            /// Whether the current user can add children to this folder.
            /// This is always false when the item is not a folder.
            /// </summary>
            public bool? CanAddChildren { get; private set; }
            /// <summary>
            /// Whether the current user can move this file to trash.
            /// </summary>
            public bool? CanTrash { get; private set; }
            /// <summary>
            /// Whether the current user can restore this file from trash.
            /// </summary>
            public bool? CanUntrash { get; private set; }
        }

        /// <summary>
        /// Additional metadata about video media.
        /// This may not be available immediately upon upload.
        /// </summary>
        public class VideoMediaMetadataData
        {
            /// <summary>
            /// The duration of the video in milliseconds.
            /// </summary>
            public long? DurationMillis { get; private set; }
            /// <summary>
            /// The height of the video in pixels.
            /// </summary>
            public int? Height { get; private set; }
            /// <summary>
            /// The width of the video in pixels.
            /// </summary>
            public int? Width { get; private set; }
        }

        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#file".
        /// </summary>
        public override string Kind => "drive#file";
        /// <summary>
        /// The ID of the file.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The raw content of the file.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public byte[] Content { get; set; }
        /// <summary>
        /// The name of the file. This is not necessarily unique within a folder. Note that
        /// for immutable items such as the top level folders of Team Drives, My Drive root
        /// folder, and Application Data folder the name is constant.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The original filename of the uploaded content if available, or else the original
        /// value of the name field. This is only available for files with binary content in Drive.
        /// </summary>
        public string OriginalFilename { get; set; }
        /// <summary>
        /// A short description of the file.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The MIME type of the file. Drive will attempt to automatically detect an appropriate
        /// value from uploaded content if no value is provided. The value cannot be changed
        /// unless a new revision is uploaded. If a file is created with a Google Doc MIME
        /// type, the uploaded content will be imported if possible. The supported import
        /// formats are published in the About resource.
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// The full file extension extracted from the name field. May contain multiple concatenated
        /// extensions, such as "tar.gz". This is only available for files with binary content
        /// in Drive. This is automatically updated when the name field changes, however
        /// it is not cleared if the new name does not contain a valid extension.
        /// </summary>
        public string FullFileExtension { get; private set; }
        /// <summary>
        /// The final component of fullFileExtension. This is only available for files with
        /// binary content in Drive.
        /// </summary>
        public string FileExtension { get; private set; }
        /// <summary>
        /// The size of the file's content in bytes. This is only applicable to files with
        /// binary content in Drive.
        /// </summary>
        public long? Size { get; private set; }
        /// <summary>
        /// The MD5 checksum for the content of the file. This is only applicable to files
        /// with binary content in Drive.
        /// </summary>
        public string Md5Checksum { get; private set; }
        /// <summary>
        /// A monotonically increasing version number for the file. This reflects every change
        /// made to the file on the server, even those not visible to the user.
        /// </summary>
        public long? Version { get; private set; }
        /// <summary>
        /// The time at which the file was created.
        /// </summary>
        public DateTime? CreatedTime { get; set; }
        /// <summary>
        /// The last time the file was modified by anyone (RFC 3339 date-time). Note that
        /// setting modifiedTime will also update modifiedByMeTime for the user.
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
        /// <summary>
        /// The IDs of the parent folders which contain the file. If not specified as part
        /// of a create request, the file will be placed directly in the My Drive folder.
        /// Update requests must use the addParents and removeParents parameters to modify the values.
        /// </summary>
        public List<string> Parents { get; set; }
        /// <summary>
        /// The full list of permissions for the file. This is only available if the requesting
        /// user can share the file. Not populated for Team Drive files.
        /// </summary>
        public List<Permission> Permissions { get; set; }
        /// <summary>
        /// A collection of arbitrary key-value pairs which are visible to all apps. Entries
        /// with null values are cleared in update and copy requests.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
        /// <summary>
        /// The number of storage quota bytes used by the file. This includes the head revision
        /// as well as previous revisions with keepForever enabled.
        /// </summary>
        public long? QuotaBytesUsed { get; private set; }
        /// <summary>
        /// Whether the file has been shared. Not populated for Team Drive files.
        /// </summary>
        public bool? Shared { get; private set; }
        /// <summary>
        /// The time at which the file was shared with the user, if applicable (RFC 3339 date-time).
        /// </summary>
        public DateTime? SharedWithMeTime { get; private set; }
        /// <summary>
        /// The user who shared the file with the requesting user, if applicable.
        /// </summary>
        public User SharingUser { get; private set; }
        /// <summary>
        /// The list of spaces which contain the file. The currently supported values are
        /// 'drive', 'appDataFolder' and 'photos'.
        /// </summary>
        public List<string> Spaces { get; private set; }
        /// <summary>
        /// Whether the user has starred the file.
        /// </summary>
        public bool? Starred { get; set; }
        /// <summary>
        /// ID of the Team Drive the file resides in.
        /// </summary>
        public string TeamDriveId { get; private set; }
        /// <summary>
        /// A short-lived link to the file's thumbnail, if available. Typically lasts on
        /// the order of hours. Only populated when the requesting app can access the file's content.
        /// </summary>
        public string ThumbnailLink { get; private set; }
        /// <summary>
        /// The thumbnail version for use in thumbnail cache invalidation.
        /// </summary>
        public long? ThumbnailVersion { get; private set; }
        /// <summary>
        /// Whether the file has been trashed, either explicitly or from a trashed parent
        /// folder. Only the owner may trash a file, and other users cannot see files in the owner's trash.
        /// </summary>
        public bool? Trashed { get; set; }
        /// <summary>
        /// The time that the item was trashed (RFC 3339 date-time). Only populated for Team Drive files.
        /// </summary>
        public DateTime? TrashedTime { get; private set; }
        /// <summary>
        /// If the file has been explicitly trashed, the user who trashed it. Only populated for Team Drive files.
        /// </summary>
        public User TrashingUser { get; private set; }
        /// <summary>
        /// Additional metadata about video media. This may not be available immediately upon upload.
        /// </summary>
        public VideoMediaMetadataData VideoMediaMetadata { get; set; }
        /// <summary>
        /// Whether the file has been viewed by this user.
        /// </summary>
        public bool? ViewedByMe { get; set; }
        /// <summary>
        /// The last time the file was viewed by the user (RFC 3339 date-time).
        /// </summary>
        public DateTime? ViewedByMeTime { get; private set; }
        /// <summary>
        /// Whether users with only reader or commenter permission can copy the file's content.
        /// This affects copy, download, and print operations.
        /// </summary>
        public bool? ViewersCanCopyContent { get; set; }
        /// <summary>
        /// A link for downloading the content of the file in a browser. This is only available
        /// for files with binary content in Drive.
        /// </summary>
        public string WebContentLink { get; private set; }
        /// <summary>
        /// A link for opening the file in a relevant Google editor or viewer in a browser.
        /// </summary>
        public string WebViewLink { get; private set; }
        /// <summary>
        /// Whether users with only writer permission can modify the file's permissions.
        /// Not populated for Team Drive files.
        /// </summary>
        public bool? WritersCanShare { get; set; }
        /// <summary>
        /// List of permission IDs for users with access to this file.
        /// </summary>
        public List<string> PermissionIds { get; private set; }
        /// <summary>
        /// The owners of the file. Currently, only certain legacy files may have more than
        /// one owner. Not populated for Team Drive files.
        /// </summary>
        public List<User> Owners { get; private set; }
        /// <summary>
        /// The ID of the file's head revision. This is currently only available for files
        /// with binary content in Drive.
        /// </summary>
        public string HeadRevisionId { get; private set; }
        /// <summary>
        /// Capabilities the current user has on this file. Each capability corresponds to
        /// a fine-grained action that a user may take.
        /// </summary>
        public CapabilitiesData Capabilities { get; private set; }
        /// <summary>
        /// Additional information about the content of the file. These fields are never
        /// populated in responses.
        /// </summary>
        public ContentHintsData ContentHints { get; private set; }
        /// <summary>
        /// Whether the file has been explicitly trashed, as opposed to recursively trashed
        /// from a parent folder.
        /// </summary>
        public bool? ExplicitlyTrashed { get; set; }
        /// <summary>
        /// The color for a folder as an RGB hex string. The supported colors are published
        /// in the folderColorPalette field of the About resource. If an unsupported color
        /// is specified, the closest color in the palette will be used instead.
        /// </summary>
        public string FolderColorRgb { get; set; }
        /// <summary>
        /// Whether any users are granted file access directly on this file. This field is
        /// only populated for Team Drive files.
        /// </summary>
        public bool? HasAugmentedPermissions { get; private set; }
        /// <summary>
        /// Whether this file has a thumbnail. This does not indicate whether the requesting
        /// app has access to the thumbnail. To check access, look for the presence of the
        /// thumbnailLink field.
        /// </summary>
        public bool? HasThumbnail { get; private set; }
        /// <summary>
        /// Whether the user owns the file. Not populated for Team Drive files.
        /// </summary>
        public bool? OwnedByMe { get; private set; }
        /// <summary>
        /// A static, unauthenticated link to the file's icon.
        /// </summary>
        public string IconLink { get; private set; }
        /// <summary>
        /// Additional metadata about image media, if available.
        /// </summary>
        public ImageMediaMetadataData ImageMediaMetadata { get; private set; }
        /// <summary>
        /// Whether the file was created or opened by the requesting app.
        /// </summary>
        public bool? IsAppAuthorized { get; private set; }
        /// <summary>
        /// The last user to modify the file.
        /// </summary>
        public User LastModifyingUser { get; private set; }
        /// <summary>
        /// Whether the file has been modified by this user.
        /// </summary>
        public bool? ModifiedByMe { get; private set; }
        /// <summary>
        /// The last time the file was modified by the user (RFC 3339 date-time).
        /// </summary>
        public DateTime? ModifiedByMeTime { get; private set; }
        /// <summary>
        /// A collection of arbitrary key-value pairs which are private to the requesting app.
        /// Entries with null values are cleared in update and copy requests.
        /// </summary>
        public Dictionary<string, string> AppProperties { get; private set; }
    }
}
