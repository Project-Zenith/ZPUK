using UnityEngine;
using System.IO;
using System.Net;
using System;
using System.ComponentModel;
using Zenith;
using UnityEditor;

namespace Zenith
{
    public class Zenith_ImportManager_Avatars
    {
        private const string V = "https://zenith.trigon.systems/ZPUK/assets/avatars/";
        public static string configName = "importConfig_avatars.json";
        public static string serverUrl = V;
        public static string internalServerUrl = V;

        public static void downloadAndImportAssetFromServer(string assetName)
        {
            if (File.Exists(Zenith_Settings.getAssetPath() + assetName))
            {
                ZenithLog(assetName + " exists. Importing it..");
                importDownloadedAsset(assetName);
            }
            else
            {
                ZenithLog(assetName + " does not exist. Starting download..");
                downloadFile(assetName);
            }
        }

        private static void downloadFile(string assetName)
        {
            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.QueryString.Add("assetName", assetName);
            w.DownloadFileCompleted += fileDownloadCompleted;
            w.DownloadProgressChanged += fileDownloadProgress;
            string url = serverUrl + assetName;
            w.DownloadFileAsync(new Uri(url), Zenith_Settings.getAssetPath() + assetName);
        }

        public static void deleteAsset(string assetName)
        {
            File.Delete(Zenith_Settings.getAssetPath() + assetName);
        }

        public static void updateConfig()
        {
            WebClient w = new WebClient();
            w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
            w.DownloadFileCompleted += configDownloadCompleted;
            w.DownloadProgressChanged += fileDownloadProgress;
            string url = internalServerUrl + configName;
            w.DownloadFileAsync(new Uri(url), Zenith_Settings.projectConfigPath + "update_" + configName);
        }

        private static void configDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //var updateFile = File.ReadAllText(Zenith_Settings.projectConfigPath + "update_" + configName);
                File.Delete(Zenith_Settings.projectConfigPath + configName);
                File.Move(Zenith_Settings.projectConfigPath + "update_" + configName,
                    Zenith_Settings.projectConfigPath + configName);
                Zenith_ImportPanel.LoadJson();

                EditorPrefs.SetInt("Zenith_configImportLastUpdated", (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                ZenithLog("Import Config has been updated!");
            }
            else
            {
                ZenithLog("Import Config could not be updated!");
            }
        }

        private static void fileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string assetName = ((WebClient) sender).QueryString["assetName"];
            if (e.Error == null)
            {
                ZenithLog("Download of file " + assetName + " completed!");
            }
            else
            {
                deleteAsset(assetName);
                ZenithLog("Download of file " + assetName + " failed!");
            }
        }

        private static void fileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = e.ProgressPercentage;
            var assetName = ((WebClient) sender).QueryString["assetName"];
            if (progress < 0) return;
            if (progress >= 100)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Download of " + assetName,
                    "Downloading " + assetName + ". Currently at: " + progress + "%",
                    (progress / 100F));
            }
        }

        public static void checkForConfigUpdate()
        {
            if (EditorPrefs.HasKey("Zenith_configImportLastUpdated"))
            {
                var lastUpdated = EditorPrefs.GetInt("Zenith_configImportLastUpdated");
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (currentTime - lastUpdated < 3600)
                {
                    Debug.Log("Not updating config: " + (currentTime - lastUpdated));
                    return;
                }
            }
            ZenithLog("Updating import config");
            updateConfig();
        }

        private static void ZenithLog(string message)
        {
            Debug.Log("[Zenith] AssetDownloadManager: " + message);
        }

        public static void importDownloadedAsset(string assetName)
        {
            AssetDatabase.ImportPackage(Zenith_Settings.getAssetPath() + assetName, true);
        }
    }
}