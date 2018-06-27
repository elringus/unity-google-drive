// Copyright 2012-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityCommon
{
    [InitializeOnLoad]
    public class PackageExporter : EditorWindow
    {
        public interface IProcessor
        {
            void OnPackagePreProcess ();
            void OnPackagePostProcess ();
        }

        protected static string PackageName { get { return PlayerPrefs.GetString(PREFS_PREFIX + "PackageName"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "PackageName", value); } }
        protected static string Copyright { get { return PlayerPrefs.GetString(PREFS_PREFIX + "Copyright"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "Copyright", value); } }
        protected static string AssetsPath { get { return "Assets/" + PackageName; } }
        protected static string OutputPath { get { return PlayerPrefs.GetString(PREFS_PREFIX + "OutputPath"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "OutputPath", value); } }
        protected static string OutputFileName { get { return PackageName; } }
        protected static string IgnoredAssetGUIds { get { return PlayerPrefs.GetString(PREFS_PREFIX + "IgnoredAssetGUIds"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "IgnoredAssetGUIds", value); } }
        private static bool IsAnyPathsIgnored { get { return !string.IsNullOrEmpty(IgnoredAssetGUIds); } }
        protected static bool IsReadyToExport { get { return !string.IsNullOrEmpty(OutputPath) && !string.IsNullOrEmpty(OutputFileName); } }

        private const string TEMP_FOLDER_NAME = "!TEMP_PACKAGE_EXPORTER";
        private const string PREFS_PREFIX = "PackageExporter.";
        private const string TAB_CHARS = "    ";

        private static Dictionary<string, string> modifiedScripts = new Dictionary<string, string>();
        private static List<UnityEngine.Object> ignoredAssets = new List<UnityEngine.Object>();

        public static void AddIgnoredAsset (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!IgnoredAssetGUIds.Contains(guid)) IgnoredAssetGUIds += "," + guid;
        }

        public static void RemoveIgnoredAsset (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!IgnoredAssetGUIds.Contains(guid)) IgnoredAssetGUIds = IgnoredAssetGUIds.Replace(guid, string.Empty);
        }

        private void Awake ()
        {
            if (string.IsNullOrEmpty(PackageName))
                PackageName = Application.productName;
        }

        private void OnEnable ()
        {
            DeserealizeIgnoredAssets();
        }

        [MenuItem("Edit/Project Settings/Package Exporter")]
        private static void OpenSettingsWindow ()
        {
            var window = GetWindow<PackageExporter>();
            window.Show();
        }

        [MenuItem("Assets/+ Export Package", priority = 20)]
        private static void ExportPackage ()
        {
            if (IsReadyToExport)
                ExportPackageImpl();
        }

        private void OnGUI ()
        {
            EditorGUILayout.LabelField("Package Exporter Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Settings are stored in editor's PlayerPrefs and won't be exposed in builds or project assets.", MessageType.Info);
            EditorGUILayout.Space();
            PackageName = EditorGUILayout.TextField("Package Name", PackageName);
            Copyright = EditorGUILayout.TextField("Copyright Notice", Copyright);
            using (new EditorGUILayout.HorizontalScope())
            {
                OutputPath = EditorGUILayout.TextField("Output Path", OutputPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    OutputPath = EditorUtility.OpenFolderPanel("Output Path", "", "");
            }
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Ignored assets: ");
            for (int i = 0; i < ignoredAssets.Count; i++)
                ignoredAssets[i] = EditorGUILayout.ObjectField(ignoredAssets[i], typeof(UnityEngine.Object), false);
            if (GUILayout.Button("+")) ignoredAssets.Add(null);
            if (EditorGUI.EndChangeCheck()) SerializeIgnoredAssets();
        }

        private static void SerializeIgnoredAssets ()
        {
            var ignoredAseetsGUIDs = new List<string>();
            foreach (var asset in ignoredAssets)
            {
                if (!asset) continue;
                var assetPath = AssetDatabase.GetAssetPath(asset);
                var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                ignoredAseetsGUIDs.Add(assetGUID);
            }
            IgnoredAssetGUIds = string.Join(",", ignoredAseetsGUIDs.ToArray());
        }

        private static void DeserealizeIgnoredAssets ()
        {
            ignoredAssets.Clear();
            var ignoredAseetsGUIDs = IgnoredAssetGUIds.Split(',');
            foreach (var guid in ignoredAseetsGUIDs)
            {
                if (string.IsNullOrEmpty(guid)) continue;
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset) ignoredAssets.Add(asset);
            }
        }

        private static bool IsAssetIgnored (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return IgnoredAssetGUIds.Contains(guid);
        }

        private static void ExportPackageImpl ()
        {
            DisplayProgressBar("Pre-processing assets...", 0f);
            var processors = GetProcessors();
            foreach (var proc in processors)
                proc.OnPackagePreProcess();

            // Temporary move-out ignored assets.
            DisplayProgressBar("Moving-out ignored assets...", 1f);
            var tmpFolderPath = string.Empty;
            if (IsAnyPathsIgnored)
            {
                var tmpFolderGuid = AssetDatabase.CreateFolder("Assets", TEMP_FOLDER_NAME);
                tmpFolderPath = AssetDatabase.GUIDToAssetPath(tmpFolderGuid);
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (!path.StartsWith(AssetsPath)) continue;
                    if (!IsAssetIgnored(path)) continue;

                    var movePath = path.Replace(AssetsPath, tmpFolderPath);
                    var moveDirectory = movePath.GetBeforeLast("/");
                    if (!Directory.Exists(moveDirectory))
                    {
                        Directory.CreateDirectory(moveDirectory);
                        AssetDatabase.Refresh();
                    }

                    AssetDatabase.MoveAsset(path, path.Replace(AssetsPath, tmpFolderPath));
                }
            }

            // Modify scripts (namespace and copyright).
            DisplayProgressBar("Modifying scripts...", .25f);
            modifiedScripts.Clear();
            var needToModify = !string.IsNullOrEmpty(Copyright);
            if (needToModify)
            {
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (!path.StartsWith(AssetsPath)) continue;
                    if (!path.EndsWith(".cs") && !path.EndsWith(".shader") && !path.EndsWith(".cginc")) continue;

                    var fullpath = Application.dataPath.Replace("Assets", "") + path;
                    var originalScriptText = File.ReadAllText(fullpath, Encoding.UTF8);

                    string scriptText = string.Empty;
                    var isImportedScript = path.Contains("ThirdParty");

                    var copyright = isImportedScript || string.IsNullOrEmpty(Copyright) ? string.Empty : "// " + Copyright;
                    if (!string.IsNullOrEmpty(copyright) && !isImportedScript)
                        scriptText += copyright + Environment.NewLine + Environment.NewLine;

                    scriptText += originalScriptText;

                    File.WriteAllText(fullpath, scriptText, Encoding.UTF8);

                    modifiedScripts.Add(fullpath, originalScriptText);
                }
            }

            // Export the package.
            DisplayProgressBar("Writing package file...", .5f);
            AssetDatabase.ExportPackage(AssetsPath, OutputPath + "/" + OutputFileName + ".unitypackage", ExportPackageOptions.Recurse);

            // Restore modified scripts.
            DisplayProgressBar("Restoring modified scripts...", .75f);
            if (needToModify)
            {
                foreach (var modifiedScript in modifiedScripts)
                    File.WriteAllText(modifiedScript.Key, modifiedScript.Value, Encoding.UTF8);
            }

            // Restore moved-out ignored assets.
            DisplayProgressBar("Restoring moved-out ignored assets...", .95f);
            if (IsAnyPathsIgnored)
            {
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (!path.StartsWith(tmpFolderPath)) continue;
                    AssetDatabase.MoveAsset(path, path.Replace(tmpFolderPath, AssetsPath));
                }

                AssetDatabase.DeleteAsset(tmpFolderPath);
            }

            DisplayProgressBar("Post-processing assets...", 1f);
            foreach (var proc in processors)
                proc.OnPackagePostProcess();

            EditorUtility.ClearProgressBar();
        }

        private static void DisplayProgressBar (string activity, float progress)
        {
            EditorUtility.DisplayProgressBar(string.Format("Exporting {0}", PackageName), activity, progress);
        }

        private static IEnumerable<IProcessor> GetProcessors ()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IProcessor).IsAssignableFrom(t) && t.IsClass)
                .Select(t => (IProcessor)Activator.CreateInstance(t));
        }
    }
}
