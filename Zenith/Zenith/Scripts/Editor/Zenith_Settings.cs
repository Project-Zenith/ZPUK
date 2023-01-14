using UnityEngine;
using UnityEditor;
using System.IO;
using System;
//using Amazon.S3.Model;
using UnityEngine.Serialization;

namespace Zenith
{

    [InitializeOnLoad]
    public class Zenith_Settings : EditorWindow
    {
        private const string Url = "https://github.com/Zenith-Productions/Zenith-Unity-Kit/";
        private const string Url1 = "https://trigon.systems/";
        private const string Link = "";
        private const string Link1 = "";

        public static string projectConfigPath = "Assets/Toolkit/Zenith/Configs/";
        private string backgroundConfig = "BackgroundVideo.txt";
        private static string projectDownloadPath = "Assets/Toolkit/Zenith/Assets/";
        private static GUIStyle ToolkitHeader;
        public Color ZPUKColor = Color.white;
        public static bool UITextRainbow { get; set; }
        //public Gradient ZPUKGRADIENT;

        [MenuItem("Zenith/Settings", false, 501)]
        public static void OpenSplashScreen()
        {
            GetWindow<Zenith_Settings>(true);
        }

        public static string getAssetPath()
        {
            if (EditorPrefs.GetBool("Zenith_onlyProject", false))
            {
                return projectDownloadPath;
            }

            var assetPath = EditorPrefs.GetString("Zenith_customAssetPath", "%appdata%/Zenith/")
                .Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                .Replace("/", "\\");

            if (!assetPath.EndsWith("\\"))
            {
                assetPath += "\\";
            }

            Directory.CreateDirectory(assetPath);
            return assetPath;
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("Zenith Settings");

            maxSize = new Vector2(400, 600);
            minSize = maxSize;

            ToolkitHeader = new GUIStyle
            {
                normal =
                {
                    background = Resources.Load("ZenithUtilsHeader") as Texture2D,
                    textColor = Color.white
                },
                fixedHeight = 200
            };
            
            if (!EditorPrefs.HasKey("Zenith_discordRPC"))
            {
                EditorPrefs.SetBool("Zenith_discordRPC", true);
            }

            if (!File.Exists(projectConfigPath + backgroundConfig) || !EditorPrefs.HasKey("Zenith_background"))
            {
                EditorPrefs.SetBool("Zenith_background", false);
                File.WriteAllText(projectConfigPath + backgroundConfig, "False");
            }
        }

        public void OnGUI()
        {
            GUILayout.Box("", ToolkitHeader);
            GUILayout.Space(4);
            GUI.backgroundColor = new Color(
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_R"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_G"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_B"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_A")
        );
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Zenith Productions"))
            {
                Application.OpenURL(Url);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Trigon.Systems"))
            {
                Application.OpenURL(Url1);
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = new Color(
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_R"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_G"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_B"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_A")
        );

            GUILayout.Space(4);
            EditorGUILayout.BeginVertical();
            GUI.backgroundColor = new Color(
           UnityEditor.EditorPrefs.GetFloat("ZPUKColor_R"),
           UnityEditor.EditorPrefs.GetFloat("ZPUKColor_G"),
           UnityEditor.EditorPrefs.GetFloat("ZPUKColor_B"),
           UnityEditor.EditorPrefs.GetFloat("ZPUKColor_A")
       );

            EditorGUILayout.LabelField("Zenith Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            //if (GUILayout.Button("Set Color"))
            //{
            //    UnityEditor.EditorPrefs.SetFloat("ZPUKColor_R", ZPUKColor.r);
            //    UnityEditor.EditorPrefs.SetFloat("ZPUKColor_G", ZPUKColor.g);
            //    UnityEditor.EditorPrefs.SetFloat("ZPUKColor_B", ZPUKColor.b);
            //    UnityEditor.EditorPrefs.SetFloat("ZPUKColor_A", ZPUKColor.a);
            //}

            EditorGUI.BeginChangeCheck();

            ZPUKColor = EditorGUI.ColorField(new Rect(3, 270, position.width - 6, 15), "Kit UI Color", ZPUKColor);
            //ZPUKGRADIENT = EditorGUI.GradientField(new Rect(3, 360, position.width - 6, 15), "ZPUK Gradient", ZPUKGRADIENT);

            if (EditorGUI.EndChangeCheck())
            {
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_R", ZPUKColor.r);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_G", ZPUKColor.g);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_B", ZPUKColor.b);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_A", ZPUKColor.a);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset Color"))
            {
                Color ZPUKColor = Color.gray;

                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_R", ZPUKColor.r);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_G", ZPUKColor.g);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_B", ZPUKColor.b);
                UnityEditor.EditorPrefs.SetFloat("ZPUKColor_A", ZPUKColor.a);
            }

            //ZPUKGRADIENT = EditorGUI.GradientField(new Rect(3, 290, position.width - 6, 15), "ZPUK Gradient", ZPUKGRADIENT);

            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
            GUILayout.Label("Overall:");
            GUILayout.BeginHorizontal();
            var isDiscordEnabled = EditorPrefs.GetBool("Zenith_discordRPC", true);
            var enableDiscord = EditorGUILayout.ToggleLeft("Discord RPC", isDiscordEnabled);
            if (enableDiscord != isDiscordEnabled)
            {
                EditorPrefs.SetBool("Zenith_discordRPC", enableDiscord);
            }

            GUILayout.EndHorizontal();
            //Hide Console logs
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            var isHiddenConsole = EditorPrefs.GetBool("Zenith_HideConsole");
            var enableConsoleHide = EditorGUILayout.ToggleLeft("Hide Console Errors", isHiddenConsole);
            if (enableConsoleHide == true)
            {
                EditorPrefs.SetBool("Zenith_HideConsole", true);
                Debug.ClearDeveloperConsole();
                Debug.unityLogger.logEnabled = false;
            }
            else if (enableConsoleHide == false)
            {
                EditorPrefs.SetBool("Zenith_HideConsole", false);
                Debug.ClearDeveloperConsole();
                Debug.unityLogger.logEnabled = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            var isUITextRainbowEnabled = EditorPrefs.GetBool("Zenith_UITextRainbow", false);
            var enableUITextRainbow = EditorGUILayout.ToggleLeft("Rainbow Text", isUITextRainbowEnabled);
            if (enableUITextRainbow != isUITextRainbowEnabled)
            {
                EditorPrefs.SetBool("Zenith_UITextRainbow", enableUITextRainbow);
                UITextRainbow = true;
            }
            else
            {
                UITextRainbow = false;
            }


            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.Label("Upload panel:");
            GUILayout.BeginHorizontal();
            var isBackgroundEnabled = EditorPrefs.GetBool("Zenith_background", false);
            var enableBackground = EditorGUILayout.ToggleLeft("Custom background", isBackgroundEnabled);
            if (enableBackground != isBackgroundEnabled)
            {
                EditorPrefs.SetBool("Zenith_background", enableBackground);
                File.WriteAllText(projectConfigPath + backgroundConfig, enableBackground.ToString());
            }

            GUILayout.EndHorizontal();


            GUILayout.Space(4);
            GUILayout.Label("Import panel:");
            GUILayout.BeginHorizontal();
            var isOnlyProjectEnabled = EditorPrefs.GetBool("Zenith_onlyProject", false);
            var enableOnlyProject = EditorGUILayout.ToggleLeft("Save files only in project", isOnlyProjectEnabled);
            if (enableOnlyProject != isOnlyProjectEnabled)
            {
                EditorPrefs.SetBool("Zenith_onlyProject", enableOnlyProject);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            GUI.backgroundColor = new Color(
             UnityEditor.EditorPrefs.GetFloat("ZPUKColor_R"),
             UnityEditor.EditorPrefs.GetFloat("ZPUKColor_G"),
             UnityEditor.EditorPrefs.GetFloat("ZPUKColor_B"),
             UnityEditor.EditorPrefs.GetFloat("ZPUKColor_A")
         );
            GUILayout.Label("Asset path:");
            GUILayout.BeginHorizontal();
            var customAssetPath = EditorGUILayout.TextField("",
                EditorPrefs.GetString("Zenith_customAssetPath", "%appdata%/Zenith/"));
            if (GUILayout.Button("Choose", GUILayout.Width(60)))
            {
                var path = EditorUtility.OpenFolderPanel("Asset download folder",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zenith");
                if (path != "")
                {
                    Debug.Log(path);
                    customAssetPath = path;
                }
            }

            if (GUILayout.Button("Reset", GUILayout.Width(50)))
            {
                customAssetPath = "%appdata%/Zenith/";
            }

            if (EditorPrefs.GetString("Zenith_customAssetPath", "%appdata%/Zenith/") != customAssetPath)
            {
                EditorPrefs.SetString("Zenith_customAssetPath", customAssetPath);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorPrefs.SetBool("Zenith_ShowInfoPanel", GUILayout.Toggle(EditorPrefs.GetBool("Zenith_ShowInfoPanel"), "Show at startup"));
            GUILayout.EndHorizontal();
        }
    }
}
// Soph waz 'ere