using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesDownloadAudio : AdaptiveWindowGUI
{
    public AudioSource AudioSource;

    private GoogleDriveFiles.GetRequest getRequest;
    private GoogleDriveFiles.DownloadAudioRequest downloadRequest;
    private string audioFileId = string.Empty;

    protected override void OnWindowGUI (int windowId)
    {
        if (IsRunning())
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", downloadRequest != null ? downloadRequest.Progress : getRequest.Progress));
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Audio file ID:", GUILayout.Width(85));
            audioFileId = GUILayout.TextField(audioFileId);
            if (GUILayout.Button("Download", GUILayout.Width(100))) GetInfoAndDownloadAudio();
            GUILayout.EndHorizontal();
        }
    }

    private bool IsRunning ()
    {
        return (getRequest != null && getRequest.IsRunning) || (downloadRequest != null && downloadRequest.IsRunning);
    }

    private void GetInfoAndDownloadAudio ()
    {
        // First, we should find out what the encoding format of the audio is.
        // In case you know that beforehand, you can use GoogleDriveFiles.DownloadAudio(fileId, audioType) right away.
        getRequest = GoogleDriveFiles.Get(audioFileId);
        getRequest.Fields = new List<string> { "id, mimeType" };
        getRequest.Send().OnDone += DownloadAudio;
    }

    private void DownloadAudio (UnityGoogleDrive.Data.File file)
    {
        downloadRequest = GoogleDriveFiles.DownloadAudio(file.Id, AudioType.UNKNOWN);
        downloadRequest.Send().OnDone += PlayAudio;
    }

    private void PlayAudio (UnityGoogleDrive.Data.AudioFile audioFile)
    {
        AudioSource.clip = audioFile.AudioClip;
        AudioSource.Play();
    }
}
