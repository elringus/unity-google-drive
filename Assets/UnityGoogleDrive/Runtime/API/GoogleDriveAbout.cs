using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// The <see cref="Data.About"/> resource collection of methods.
/// Prototype: https://developers.google.com/drive/v3/reference/about.
/// </summary>
public static class GoogleDriveAbout
{
    /// <summary>
    /// Gets information about the user, the user's Drive, and system capabilities.
    /// </summary>
    public class GetRequest : GoogleDriveRequest<GoogleDriveQueryParameters, Data.About>
    {
        public GetRequest (GoogleDriveQueryParameters queryParameters)
            : base(@"https://www.googleapis.com/drive/v3/about", UnityWebRequest.kHttpVerbGET, queryParameters) { }
    }

    /// <summary>
    /// Gets information about the user, the user's Drive, and system capabilities.
    /// </summary>
    /// <param name="fields">
    /// Specify fields to return. Only <see cref="Data.About.User"/> and 
    /// <see cref="Data.About.StorageQuota"/> fields are returned by default.
    /// </param>
    public static GetRequest Get (List<string> fields = null)
    {
        var queryParams = new GoogleDriveQueryParameters();
        if (fields != null) queryParams.Fields = fields;
        else queryParams.Fields = new List<string> { "user", "storageQuota" };
        return new GetRequest(queryParams);
    }

}
