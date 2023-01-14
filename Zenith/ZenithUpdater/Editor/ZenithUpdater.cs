using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Net.Http;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;


namespace Zenith
{



    public class Zenith_AutomaticUpdateAndInstall : MonoBehaviour
    {

        //get server version
        public static string versionURL = "https://trigon.systems/zenithversion.txt";
        //get download url
        public static string unitypackageUrl = "https://trigon.systems/all-sdk/updatesdk/newest/Zenith"; //sdk

        //GetVersion
        public static string currentVersion = File.ReadAllText("Assets/Zenith/Zenithversion.txt");


        //select where to be imported (sdk)
        public static string assetPath = "Assets\\";
        //Custom name for downloaded unitypackage
        public static string assetName = "latest Zenith.unitypackage";
        //gets Toolkit Directory Path
        public static string ToolkitPath = "Assets\\Zenith\\";
        public async static void AutomaticSDKInstaller()
        {
            //Starting Browser
            HttpClient httpClient = new HttpClient();
            //Reading Version data
            var result = await httpClient.GetAsync(versionURL);
            var strServerVersion = await result.Content.ReadAsStringAsync();
            var serverVersion = strServerVersion;

            var thisVersion = currentVersion;

            try
            {
                //Checking if Uptodate or not
                if (serverVersion == thisVersion)
                {
                    //up to date
                    ZenithLog("you are using the newest version of Zenith!");
                    EditorUtility.DisplayDialog("You are up to date",
                        "Current Zenith version: " + currentVersion,
                        "Okay"
                        );
                }
                else
                {
                    //not up to date
                    ZenithLog("There is an Update Available");
                    //start download
                    await DownloadZenith();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[Zenith] AssetDownloadManager:" + ex.Message);
            }
        }

        public static async Task DownloadZenith()
        {
            ZenithLog("Asking for Approval..");
            if (EditorUtility.DisplayDialog("Zenith Updater", "Your Version (V" + currentVersion.ToString() + ") is Outdated!" + " do you want to Download and Import the Newest Version?", "Yes", "No"))
            {
                //starting deletion of old sdk
                await DeleteAndDownloadAsync();
            }
            else
            {
                //canceling the whole process
                ZenithLog("You pressed no.");
            }
        }

        public static async Task DeleteAndDownloadAsync()
        {
            try
            {
                if (EditorUtility.DisplayDialog("Zenith_Automatic_DownloadAndInstall", "The Old devkit will Be Deleted and the New devkit Will be imported!", "Okay"))
                {
                    try
                    {
                        //gets every file in Toolkit folder
                        string[] ToolkitDir = Directory.GetFiles(ToolkitPath, "*.*");

                        try
                        {
                            //Deletes All Files in Toolkit folder
                            await Task.Run(() =>
                            {
                                foreach (string f in ToolkitDir)
                                {
                                    ZenithLog($"{f} - Deleted");
                                    File.Delete(f);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            EditorUtility.DisplayDialog("Error Deleting devkit", ex.Message, "Okay");
                        }
                    }
                    catch //catch nothing
                    {
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                EditorUtility.DisplayDialog("Error Deleting Files", "Error wihle trying to find Toolkit Folder.", "Ignore");
            }
            //Checks if Directory still exists
            //if (Directory.Exists(ToolkitPath))
            //{
            //    ZenithLog($"{ToolkitPath} - Deleted");
            //    //Delete Folder
            //    Directory.Delete(ToolkitPath, true);
            //}
            //Refresh
            AssetDatabase.Refresh();


            if (EditorUtility.DisplayDialog("Zenith_Automatic_DownloadAndInstall", "The New devkit Will be imported now!", "Nice!"))
            {
                //Creates WebClient to Download latest .unitypackage
                WebClient w = new WebClient();
                w.Headers.Set(HttpRequestHeader.UserAgent, "Webkit Gecko wHTTPS (Keep Alive 55)");
                w.DownloadFileCompleted += new AsyncCompletedEventHandler(fileDownloadComplete);
                w.DownloadProgressChanged += fileDownloadProgress;
                string url = unitypackageUrl;
                w.DownloadFileAsync(new Uri(url), assetName);
            }
        }

        private static void fileDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            //Creates A ProgressBar
            var progress = e.ProgressPercentage;
            if (progress < 0) return;
            if (progress >= 100)
            {
                EditorUtility.ClearProgressBar();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Download of " + assetName,
                    "Downloading " + assetName + " " + progress + "%",
                    (progress / 100F));
            }
        }

        private static void fileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            //Checks if Download is complete
            if (e.Error == null)
            {
                ZenithLog("Download completed!");
                //Opens .unitypackage
                Process.Start(assetName);
            }
            else
            {
                //Asks to open Download Page Manually
                ZenithLog("Download failed!");
                if (EditorUtility.DisplayDialog("Zenith_Automatic_DownloadAndInstall", "Zenith Failed to Download to latest Version", "Open URL instead", "Cancel"))
                {
                    Application.OpenURL(unitypackageUrl);
                }
            }
        }

        private static void ZenithLog(string message)
        {
            //Our Logger
            Debug.Log("[Zenith] AssetDownloadManager: " + message);
        }
    }
}
