
/// <summary>
/// Base class for a Google Drive resource data representation and associated methods.
/// </summary>
[System.Serializable]
public abstract class GoogleDriveResource
{
    /// <summary>
    /// Identifies what kind of resource this is.
    /// </summary>
    public abstract string Kind { get; }

}
