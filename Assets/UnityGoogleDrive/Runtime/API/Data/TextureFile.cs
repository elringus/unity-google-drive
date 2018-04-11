using UnityEngine;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// The metadata for a texture file stored in Google Drive.
    /// Unity-specific data to use with <see cref="GoogleDriveFiles.DownloadTextureRequest"/>.
    /// </summary>
    public class TextureFile : File
    {
        [Newtonsoft.Json.JsonIgnore]
        public Texture2D Texture { get; set; }
    }
}
