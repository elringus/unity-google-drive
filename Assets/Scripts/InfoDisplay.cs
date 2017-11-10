using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    public Rect WindowRect = new Rect(0, 0, 250, 0);

    private GoogleDriveAbout.GetRequest request;
         
    private void Start ()
    {
        UpdateInfo();
    }

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Info");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else if (request.Response != null)
        {
            GUILayout.Label(string.Format("User name: {0}\nUser email: {1}\nSpace used: {2:0}/{3:0} MB", 
                request.Response.User.DisplayName,
                request.Response.User.EmailAddress,
                request.Response.StorageQuota.Usage * .000001f,
                request.Response.StorageQuota.Limit * .000001f));
        }

        if (GUILayout.Button("Refresh"))
            UpdateInfo();
    }

    private void UpdateInfo ()
    {
        request = GoogleDriveAbout.Get();
        request.Send();
    }
}
