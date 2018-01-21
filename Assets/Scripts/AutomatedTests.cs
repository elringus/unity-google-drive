using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;

public class AutomatedTests
{
    private const string TEST_RESOURCE_PATH = "TestImage";

    private string createdFileId;
    private string copiedFileId;

    [UnityTest]
    public IEnumerator Test001_AboutGet ()
    {
        var request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user" };
        yield return request.Send();
        Assert.IsFalse(request.IsError);
    }

    [UnityTest]
    public IEnumerator Test002_FilesList ()
    {
        var request = GoogleDriveFiles.List();
        yield return request.Send();
        Assert.IsFalse(request.IsError);
    }

    [UnityTest]
    public IEnumerator Test003_FilesCreate ()
    {
        var content = Resources.Load<Texture2D>(TEST_RESOURCE_PATH).EncodeToPNG();
        var file = new UnityGoogleDrive.Data.File() { Name = "AutoTestUpload", Content = content };
        var request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id" };
        yield return request.Send();
        Assert.IsFalse(request.IsError);
        Assert.NotNull(request.ResponseData.Id);
        createdFileId = request.ResponseData.Id;
    }

    [UnityTest]
    public IEnumerator Test004_FilesGet ()
    {
        var request = GoogleDriveFiles.Get(createdFileId);
        yield return request.Send();
        Assert.IsFalse(request.IsError);
    }

    [UnityTest]
    public IEnumerator Test005_FilesDownload ()
    {
        var request = GoogleDriveFiles.Download(createdFileId);
        yield return request.Send();
        Assert.IsFalse(request.IsError);
        Assert.NotNull(request.ResponseData.Content);
        Assert.Greater(request.ResponseData.Content.Length, 0);
    }

    [UnityTest]
    public IEnumerator Test006_FilesCopy ()
    {
        var file = new UnityGoogleDrive.Data.File() { Id = createdFileId };
        var request = GoogleDriveFiles.Copy(file);
        request.Fields = new List<string> { "id" };
        yield return request.Send();
        Assert.IsFalse(request.IsError);
        Assert.NotNull(request.ResponseData.Id);
        copiedFileId = request.ResponseData.Id;
    }

    [UnityTest]
    public IEnumerator Test007_FilesEmptyTrash ()
    {
        var request = GoogleDriveFiles.EmptyTrash();
        yield return request.Send();
        Assert.IsFalse(request.IsError);
    }

    [UnityTest]
    public IEnumerator Test008_FilesGenerateIds ()
    {
        const int COUNT = 3;
        var request = GoogleDriveFiles.GenerateIds(COUNT);
        yield return request.Send();
        Assert.IsFalse(request.IsError);
        Assert.AreEqual(request.ResponseData.Ids.Count, COUNT);
    }

    [UnityTest]
    public IEnumerator Test009_FilesUpdate ()
    {
        const string UPDATED_NAME = "UpdatedName";
        var file = new UnityGoogleDrive.Data.File() { Name = UPDATED_NAME };
        var request = GoogleDriveFiles.Update(copiedFileId, file);
        yield return request.Send();
        Assert.IsFalse(request.IsError);
        Assert.AreEqual(request.ResponseData.Name, UPDATED_NAME);
    }

    [UnityTest]
    public IEnumerator Test999_FilesDelete ()
    {
        var request1 = GoogleDriveFiles.Delete(createdFileId);
        yield return request1.Send();
        Assert.IsFalse(request1.IsError);
        var request2 = GoogleDriveFiles.Delete(copiedFileId);
        yield return request2.Send();
        Assert.IsFalse(request2.IsError);
    }
}
