using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// Information about the user, the user's Drive, and system capabilities.
    /// Prototype: https://developers.google.com/drive/v3/reference/about#resource-representations.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class About : ResourceData
    {
        /// <summary>
        /// The user's storage quota limits and usage. All fields are measured in bytes.
        /// </summary>
        public class StorageQuotaData
        {
            /// <summary>
            /// The usage limit, if applicable.
            /// This will not be present if the user has unlimited storage.
            /// </summary>
            public long? Limit { get; private set; }
            /// <summary>
            /// The total usage across all services.
            /// </summary>
            public long? Usage { get; private set; }
            /// <summary>
            /// The usage by all files in Google Drive.
            /// </summary>
            public long? UsageInDrive { get; private set; }
            /// <summary>
            /// The usage by trashed files in Google Drive.
            /// </summary>
            public long? UsageInDriveTrash { get; private set; }
        }

        public class TeamDriveThemesData
        {
            /// <summary>
            /// The ID of the theme.
            /// </summary>
            public string Id { get; private set; }
            /// <summary>
            /// A link to this Team Drive theme's background image.
            /// </summary>
            public string BackgroundImageLink { get; private set; }
            /// <summary>
            /// The color of this Team Drive theme as an RGB hex string.
            /// </summary>
            public string ColorRgb { get; private set; }
        }

        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#about".
        /// </summary>
        public override string Kind => "drive#about";
        /// <summary>
        /// Whether the user has installed the requesting app.
        /// </summary>
        public bool? AppInstalled { get; private set; }
        /// <summary>
        /// A map of source MIME type to possible targets for all supported exports.
        /// </summary>
        public Dictionary<string, List<string>> ExportFormats { get; private set; }
        /// <summary>
        /// The currently supported folder colors as RGB hex strings.
        /// </summary>
        public List<string> FolderColorPalette { get; private set; }
        /// <summary>
        /// A map of source MIME type to possible targets for all supported imports.
        /// </summary>
        public Dictionary<string, List<string>> ImportFormats { get; private set; }
        /// <summary>
        /// A map of maximum import sizes by MIME type, in bytes.
        /// </summary>
        public Dictionary<string, long> MaxImportSizes { get; private set; }
        /// <summary>
        /// The maximum upload size in bytes.
        /// </summary>
        public long? MaxUploadSize { get; private set; }
        /// <summary>
        /// The user's storage quota limits and usage. All fields are measured in bytes.
        /// </summary>
        public StorageQuotaData StorageQuota { get; private set; }
        /// <summary>
        /// A list of themes that are supported for Team Drives.
        /// </summary>
        public List<TeamDriveThemesData> TeamDriveThemes { get; private set; }
        /// <summary>
        /// The authenticated user.
        /// </summary>
        public User User { get; private set; }
    }
}
