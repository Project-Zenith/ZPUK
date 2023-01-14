using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenith
{
    public class Zenith_Help
    {

        [MenuItem("Zenith/Help/Github", false, 1049)]
        public static void OpenDiscordLink()
        {
            Application.OpenURL("https://github.com/Zenith-Productions/Zenith-Unity-Kit");
        }
        
        [MenuItem("Zenith/Utilities/Update configs", false, 1000)]
        public static void ForceUpdateConfigs()
        {
            Zenith_ImportManager.updateConfig();
        }
        public static void UpdatesdkBtn()
        {

            Zenith_AutomaticUpdateAndInstall.AutomaticSDKInstaller();
        }


    }
}