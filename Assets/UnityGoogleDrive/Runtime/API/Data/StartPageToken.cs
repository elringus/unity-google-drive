using System.Diagnostics.CodeAnalysis;
using UnityGoogleDrive.Newtonsoft.Json;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// The starting page token for listing changes.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class StartPageToken : ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#startPageToken".
        /// </summary>
        public override string Kind => "drive#startPageToken";
        /// <summary>
        /// The value of the starting page token for listing changes.
        /// </summary>
        [JsonProperty("startPageToken")]
        public string StartPageTokenValue { get; private set; }
    }
}
