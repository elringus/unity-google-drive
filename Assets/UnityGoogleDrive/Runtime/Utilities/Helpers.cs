using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityGoogleDrive
{
    public static class Helpers
    {
        /// <summary>
        /// The MIME type of a <see cref="Data.File"/> representing a folder in Google Drive.
        /// </summary>
        /// <remarks>
        /// A folder in Google Drive is actually a file with the special MIME type. 
        /// More info: https://developers.google.com/drive/api/v3/folder.
        /// </remarks>
        public const string FOLDER_MIME_TYPE = "application/vnd.google-apps.folder";
        /// <summary>
        /// ID alias of the drive's root folder.
        /// </summary>
        public const string ROOT_ALIAS = "root";
        /// <summary>
        /// ID alias of the app data folder.
        /// </summary>
        public const string APPDATA_ALIAS = "appDataFolder";
        /// <summary>
        /// Main user's space, starting at the root.
        /// </summary>
        public const string DRIVE_SPACE = "drive";
        /// <summary>
        /// A special hidden space that your app can use to store application data.
        /// </summary>
        public const string APPDATA_SPACE = "appDataFolder";

        #if NET_4_6 || NET_STANDARD_2_0
        /// <summary>
        /// Looks for the files located at the provided path.
        /// </summary>
        /// <param name="path">File's path in Google Drive. Add front slash to find all files in a folder.</param>
        /// <param name="appData">Whether to look inside the AppData folder instead of the drive root.</param>
        /// <param name="fields">File fields to request.</param>
        /// <param name="mimeType">File's MIME type.</param>
        /// <param name="trashed">Whether to include trashed files.</param>
        public static async System.Threading.Tasks.Task<List<Data.File>> FindFilesByPathAsync (string path, bool appData = false, List<string> fields = null, string mime = null, bool trashed = false)
        {
            var result = new List<Data.File>();

            var fileName = Path.GetFileName(path);

            var parentIds = await ValidatePath(path, appData);
            if (parentIds == null) return result;

            var parentQueries = parentIds.Select(parentId => $"'{parentId}' in parents");
            var query = $"({string.Join(" or ", parentQueries)}) and trashed = {trashed}";
            if (!string.IsNullOrWhiteSpace(fileName)) query += $" and name = '{fileName}' ";
            if (!string.IsNullOrWhiteSpace(mime)) query += $" and mimeType = '{mime}'";

            if (fields == null) fields = new List<string>();
            fields.Add("nextPageToken, files(id)");

            string pageToken = null;
            do
            {
                var listRequest = GoogleDriveFiles.List(query, fields, appData ? APPDATA_SPACE : DRIVE_SPACE, pageToken);
                var fileList = await listRequest.Send();
                if (fileList?.Files?.Count > 0) result.AddRange(fileList.Files);
                pageToken = fileList?.NextPageToken;
                listRequest.Dispose();

            } while (pageToken != null);

            return result;
        }

        /// <summary>
        /// Traverses the entire path and returns ID of the top-level folder(s) (if found), null otherwise.
        /// </summary>
        /// <param name="path">Path to validate.</param>
        /// <returns>ID of the final folders in the path; null if the path is not valid.</returns>
        public static async System.Threading.Tasks.Task<HashSet<string>> ValidatePath (string path, bool appData, string parentId = null)
        {
            if (parentId == null) parentId = appData ? APPDATA_ALIAS : ROOT_ALIAS;
            if (string.IsNullOrWhiteSpace(path)) return new HashSet<string> { parentId };
            path = Path.GetDirectoryName(path).Replace('\\', '/');
            if (string.IsNullOrWhiteSpace(path) || path.Trim() == "/" || path.Trim() == "\\") return new HashSet<string> { parentId };
            var parentNames = path.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var result = new HashSet<string>();

            for (int i = 0; i < parentNames.Length; i++)
            {
                using (var listRequest = new GoogleDriveFiles.ListRequest())
                {
                    listRequest.Fields = new List<string> { "files(id, name)" };
                    listRequest.Spaces = appData ? APPDATA_SPACE : DRIVE_SPACE;
                    listRequest.PageSize = 1000; // Assume we can't have more than 1K folders with equal names at the same level and skip the pagination.
                    listRequest.Q = $"'{parentId}' in parents and name = '{parentNames[i]}' and mimeType = '{FOLDER_MIME_TYPE}' and trashed = false";

                    await listRequest.Send();

                    if (listRequest == null || listRequest.IsError || listRequest.ResponseData.Files == null || listRequest.ResponseData.Files.Count == 0)
                        return null;

                    // When at the top level, add all the folder's IDs.
                    if (i + 1 == parentNames.Length) { result.UnionWith(listRequest.ResponseData.Files.Select(f => f.Id)); break; }

                    // Multiple folders with equal names found at the same level (but not top), travers them recursively.
                    if (listRequest.ResponseData.Files.Count > 1)
                    {
                        var nextPath = string.Join("/", parentNames.Skip(i + 1)) + "/";
                        foreach (var folder in listRequest.ResponseData.Files)
                        {
                            var topFolderIds = await ValidatePath(nextPath, appData, folder.Id);
                            if (topFolderIds != null) result.UnionWith(topFolderIds);
                        }
                        break;
                    } // Only one folder found, use it's ID to travers higher.
                    else parentId = listRequest.ResponseData.Files[0].Id;
                }
            }

            return result.Count == 0 ? null : result;
        }
        #endif
    }
}
