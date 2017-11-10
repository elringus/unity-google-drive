
namespace Data
{
    /// <summary>
    /// Base class for a Google Drive resource data representation.
    /// </summary>
    [System.Serializable]
    public abstract class GoogleDriveData
    {
        /// <summary>
        /// Identifies what kind of resource this is.
        /// </summary>
        public abstract string Kind { get; }

    }
}
