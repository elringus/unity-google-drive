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
        protected static string PackageName { get { return PlayerPrefs.GetString(PREFS_PREFIX + "PackageName"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "PackageName", value); } }
        protected static string Copyright { get { return PlayerPrefs.GetString(PREFS_PREFIX + "Copyright"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "Copyright", value); } }
        protected static string AssetsPath { get { return "Assets/" + PackageName; } }
        protected static string OutputPath { get { return PlayerPrefs.GetString(PREFS_PREFIX + "OutputPath"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "OutputPath", value); } }
        protected static string OutputFileName { get { return PackageName; } }
        protected static string IgnoredPaths { get { return PlayerPrefs.GetString(PREFS_PREFIX + "IgnoredPaths"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "IgnoredPaths", value); } }
        private static bool IsAnyPathsIgnored { get { return !string.IsNullOrEmpty(IgnoredPaths); } }
        protected static bool IsReadyToExport { get { return !string.IsNullOrEmpty(OutputPath) && !string.IsNullOrEmpty(OutputFileName); } }

        private const string TEMP_FOLDER_NAME = "!TEMP_PACKAGE_EXPORTER";
        private const string PREFS_PREFIX = "PackageExporter.";
        private const string TAB_CHARS = "    ";

        private static Dictionary<string, string> modifiedScripts = new Dictionary<string, string>();

        private void Awake ()
        {
            if (string.IsNullOrEmpty(PackageName))
                PackageName = Application.productName;
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
            EditorGUILayout.LabelField("Ignored paths (split with new line, start with 'Assets/...'): ");
            IgnoredPaths = EditorGUILayout.TextArea(IgnoredPaths);
        }

        private static void ExportPackageImpl ()
        {
            // Temporary move-out ignored assets.
            DisplayProgressBar("Moving-out ignored assets...", 0f);
            var tmpFolderPath = string.Empty;
            if (IsAnyPathsIgnored)
            {
                var tmpFolderGuid = AssetDatabase.CreateFolder("Assets", TEMP_FOLDER_NAME);
                tmpFolderPath = AssetDatabase.GUIDToAssetPath(tmpFolderGuid);
                var ignoredPaths = IgnoredPaths.SplitByNewLine().ToList();
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (!path.StartsWith(AssetsPath)) continue;
                    if (!ignoredPaths.Exists(p => path.StartsWith(p))) continue;

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

            EditorUtility.ClearProgressBar();
        }

        private static void DisplayProgressBar (string activity, float progress)
        {
            EditorUtility.DisplayProgressBar(string.Format("Exporting {0}", PackageName), activity, progress);
        }
    }
}
