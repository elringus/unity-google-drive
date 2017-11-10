using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    private GoogleDriveAbout about;
    private GoogleDriveRequest<GoogleDriveAbout> request;
         
    private void Start ()
    {
        UpdateInfo();
    }

    private void OnGUI ()
    {
        GUILayout.Label("Google Drive Info\n==============");

        if (request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else if (about != null)
        {
            GUILayout.Label("User name: " + about.User.DisplayName);
            GUILayout.Label("User email: " + about.User.EmailAddress);
            GUILayout.Label(string.Format("Space used: {0:0}/{1:0} MB", about.StorageQuota.Usage * .000001f, about.StorageQuota.Limit * .000001f));
        }

        if (GUILayout.Button("Refresh"))
            UpdateInfo();
    }

    private void UpdateInfo ()
    {
        request = GoogleDriveAbout.Get();
        request.Send().OnDone += response => about = response;
    }
}
