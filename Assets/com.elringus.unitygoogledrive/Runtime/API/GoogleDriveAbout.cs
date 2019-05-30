using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// The <see cref="Data.About"/> resource collection of methods.
    /// Prototype: https://developers.google.com/drive/v3/reference/about.
    /// </summary>
    public static class GoogleDriveAbout
    {
        /// <summary>
        /// Gets information about the user, the user's Drive, and system capabilities.
        /// </summary>
        public class GetRequest : GoogleDriveRequest<Data.About>
        {
            public GetRequest ()
                : base(@"https://www.googleapis.com/drive/v3/about", UnityWebRequest.kHttpVerbGET) { }
        }

        /// <summary>
        /// Gets information about the user, the user's Drive, and system capabilities.
        /// </summary>
        public static GetRequest Get ()
        {
            return new GetRequest();
        }
    }
}
