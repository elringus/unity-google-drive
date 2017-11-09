using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Information about the user, the user's Drive, and system capabilities.
/// Prototype: https://developers.google.com/drive/v3/reference/about.
/// </summary>
public class GoogleDriveAbout : GoogleDriveResource
{
    /// <summary>
    /// The user's storage quota limits and usage. All fields are measured in bytes.
    /// </summary>
    [System.Serializable]
    public class StorageQuotaData
    {
        /// <summary>
        /// The usage limit, if applicable. 
        /// This will not be present if the user has unlimited storage.
        /// </summary>
        public long Limit { get { return limit; } }
        /// <summary>
        /// The total usage across all services.
        /// </summary>
        public long Usage { get { return usage; } }
        /// <summary>
        /// The usage by all files in Google Drive.
        /// </summary>
        public long UsageInDrive { get { return usageInDrive; } }
        /// <summary>
        /// The usage by trashed files in Google Drive.
        /// </summary>
        public long UsageInDriveTrash { get { return usageInDriveTrash; } }

        [SerializeField] private long limit = -1;
        [SerializeField] private long usage = -1;
        [SerializeField] private long usageInDrive = -1;
        [SerializeField] private long usageInDriveTrash = -1;
    }

    [System.Serializable]
    public class TeamDriveThemesData
    {
        /// <summary>
        /// The ID of the theme.
        /// </summary>
        public string Id { get { return id; } }
        /// <summary>
        /// A link to this Team Drive theme's background image.
        /// </summary>
        public string BackgroundImageLink { get { return backgroundImageLink; } }
        /// <summary>
        /// The color of this Team Drive theme as an RGB hex string.
        /// </summary>
        public string ColorRgb { get { return colorRgb; } }

        [SerializeField] private string id = null;
        [SerializeField] private string backgroundImageLink = null;
        [SerializeField] private string colorRgb = null;
    }

    /// <summary>
    /// Identifies what kind of resource this is. Value: the fixed string "drive#about".
    /// </summary>
    public override string Kind { get { return "drive#about"; } }
    /// <summary>
    /// Whether the user has installed the requesting app.
    /// </summary>
    public bool AppInstalled { get { return appInstalled; } }
    /// <summary>
    /// A map of source MIME type to possible targets for all supported exports.
    /// </summary>
    public IDictionary<string, IList<string>> ExportFormats { get { return exportFormats; } }
    /// <summary>
    /// The currently supported folder colors as RGB hex strings.
    /// </summary>
    public List<string> FolderColorPalette { get { return folderColorPalette; } }
    /// <summary>
    /// A map of source MIME type to possible targets for all supported imports.
    /// </summary>
    public IDictionary<string, IList<string>> ImportFormats { get { return importFormats; } }
    /// <summary>
    /// A map of maximum import sizes by MIME type, in bytes.
    /// </summary>
    public IDictionary<string, long> MaxImportSizes { get { return maxImportSizes; } }
    /// <summary>
    /// The maximum upload size in bytes.
    /// </summary>
    public long MaxUploadSize { get { return maxUploadSize; } }
    /// <summary>
    /// The user's storage quota limits and usage. All fields are measured in bytes.
    /// </summary>
    public StorageQuotaData StorageQuota { get { return storageQuota; } }
    /// <summary>
    /// A list of themes that are supported for Team Drives.
    /// </summary>
    public List<TeamDriveThemesData> TeamDriveThemes { get { return teamDriveThemes; } }
    /// <summary>
    /// The authenticated user.
    /// </summary>
    public GoogleDriveUser User { get { return user; } }

    [SerializeField] private bool appInstalled = false;
    [SerializeField] private IDictionary<string, IList<string>> exportFormats = null;
    [SerializeField] private List<string> folderColorPalette = null;
    [SerializeField] private IDictionary<string, IList<string>> importFormats = null;
    [SerializeField] private IDictionary<string, long> maxImportSizes = null;
    [SerializeField] private long maxUploadSize = -1;
    [SerializeField] private StorageQuotaData storageQuota = null;
    [SerializeField] private List<TeamDriveThemesData> teamDriveThemes = null;
    [SerializeField] private GoogleDriveUser user = null;

    /// <summary>
    /// Gets information about the user, the user's Drive, and system capabilities.
    /// </summary>
    public static GoogleDriveRequest<GoogleDriveAbout> Get ()
    {
        return new GoogleDriveRequest<GoogleDriveAbout>(@"https://www.googleapis.com/drive/v3/about", UnityWebRequest.kHttpVerbGET);
    }
}
