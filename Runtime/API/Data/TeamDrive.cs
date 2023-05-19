using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// Representation of a Team Drive.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class TeamDrive : ResourceData
    {
        /// <summary>
        /// An image file and cropping parameters from which a background image for this
        /// Team Drive is set. This is a write only field; it can only be set on drive.teamdrives.update
        /// requests that don't set themeId. When specified, all fields of the backgroundImageFile must be set.
        /// </summary>
        public class BackgroundImageFileData
        {
            /// <summary>
            /// The ID of an image file in Drive to use for the background image.
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// The width of the cropped image in the closed range of 0 to 1. This value represents
            /// the width of the cropped image divided by the width of the entire image. The
            /// eight is computed by applying a width to height aspect ratio of 80 to 9. The
            /// resulting image must be at least 1280 pixels wide and 144 pixels high.
            /// </summary>
            public float? Width { get; set; }
            /// <summary>
            /// The X coordinate of the upper left corner of the cropping area in the background
            /// image. This is a value in the closed range of 0 to 1. This value represents the
            /// horizontal distance from the left side of the entire image to the left side of
            /// the cropping area divided by the width of the entire image.
            /// </summary>
            public float? XCoordinate { get; set; }
            /// <summary>
            /// The Y coordinate of the upper left corner of the cropping area in the background
            /// image. This is a value in the closed range of 0 to 1. This value represents the
            /// vertical distance from the top side of the entire image to the top side of the
            /// cropping area divided by the height of the entire image.
            /// </summary>
            public float? YCoordinate { get; set; }
        }

        /// <summary>
        /// Capabilities the current user has on this Team Drive.
        /// </summary>
        public class CapabilitiesData
        {
            /// <summary>
            /// Whether the current user can add children to folders in this Team Drive.
            /// </summary>
            public bool? CanAddChildren { get; private set; }
            /// <summary>
            /// Whether the current user can change the background of this Team Drive.
            /// </summary>
            public bool? CanChangeTeamDriveBackground { get; private set; }
            /// <summary>
            /// Whether the current user can comment on files in this Team Drive.
            /// </summary>
            public bool? CanComment { get; private set; }
            /// <summary>
            /// Whether the current user can copy files in this Team Drive.
            /// </summary>
            public bool? CanCopy { get; private set; }
            /// <summary>
            /// Whether the current user can delete this Team Drive. Attempting to delete the
            /// Team Drive may still fail if there are untrashed items inside the Team Drive.
            /// </summary>
            public bool? CanDeleteTeamDrive { get; private set; }
            /// <summary>
            /// Whether the current user can download files in this Team Drive.
            /// </summary>
            public bool? CanDownload { get; private set; }
            /// <summary>
            /// Whether the current user can edit files in this Team Drive.
            /// </summary>
            public bool? CanEdit { get; private set; }
            /// <summary>
            /// Whether the current user can list the children of folders in this Team Drive.
            /// </summary>
            public bool? CanListChildren { get; private set; }
            /// <summary>
            /// Whether the current user can add members to this Team Drive or remove them or
            /// change their role.
            /// </summary>
            public bool? CanManageMembers { get; private set; }
            /// <summary>
            /// Whether the current user can read the revisions resource of files in this Team Drive.
            /// </summary>
            public bool? CanReadRevisions { get; private set; }
            /// <summary>
            /// Whether the current user can remove children from folders in this Team Drive.
            /// </summary>
            public bool? CanRemoveChildren { get; private set; }
            /// <summary>
            /// Whether the current user can rename files or folders in this Team Drive.
            /// </summary>
            public bool? CanRename { get; private set; }
            /// <summary>
            /// Whether the current user can rename this Team Drive.
            /// </summary>
            public bool? CanRenameTeamDrive { get; private set; }
            /// <summary>
            /// Whether the current user can share files or folders in this Team Drive.
            /// </summary>
            public bool? CanShare { get; private set; }
        }

        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#teamDrive".
        /// </summary>
        public override string Kind => "drive#teamDrive";
        /// <summary>
        /// An image file and cropping parameters from which a background image for this
        /// Team Drive is set. This is a write only field; it can only be set on drive.teamdrives.update
        /// requests that don't set themeId. When specified, all fields of the backgroundImageFile must be set.
        /// </summary>
        public BackgroundImageFileData BackgroundImageFile { get; set; }
        /// <summary>
        /// A short-lived link to this Team Drive's background image.
        /// </summary>
        public string BackgroundImageLink { get; private set; }
        /// <summary>
        /// Capabilities the current user has on this Team Drive.
        /// </summary>
        public CapabilitiesData Capabilities { get; private set; }
        /// <summary>
        /// The color of this Team Drive as an RGB hex string. It can only be set on a drive.teamdrives.update
        /// request that does not set themeId.
        /// </summary>
        public string ColorRgb { get; set; }
        /// <summary>
        /// The time at which the Team Drive was created.
        /// </summary>
        public DateTime? CreatedTime { get; private set; }
        /// <summary>
        /// The ID of this Team Drive which is also the ID of the top level folder for this Team Drive.
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The name of this Team Drive.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The ID of the theme from which the background image and color will be set. The
        /// set of possible teamDriveThemes can be retrieved from a drive.about.get response.
        /// When not specified on a drive.teamdrives.create request, a random theme is chosen
        /// from which the background image and color are set. This is a write-only field;
        /// it can only be set on requests that don't set colorRgb or backgroundImageFile.
        /// </summary>
        public string ThemeId { get; set; }
    }
}
