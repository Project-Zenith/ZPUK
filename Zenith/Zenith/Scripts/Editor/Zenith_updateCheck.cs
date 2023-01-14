using Zenith;
using System.IO;
using System.Net.Http;
using UnityEditor;
using UnityEngine;

public class Zenith_updateCheck : MonoBehaviour
{
    [InitializeOnLoad]
    public class Startup
    {
        public static string versionURL = "https://trigon.systems/ZPUK/updateZPUK/version.txt";
        public static string currentVersion = File.ReadAllText("Assets/Zenith/Zenithversion.txt");
        static Startup()
        {
            Check();
        }
        public async static void Check()
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync(versionURL);
            var strServerVersion = await result.Content.ReadAsStringAsync();
            var serverVersion = strServerVersion;

            var thisVersion = currentVersion;
            
            if (serverVersion != thisVersion)
            {
                Zenith_AutomaticUpdateAndInstall.AutomaticZPUKInstaller();
            }
        }
    }
}
