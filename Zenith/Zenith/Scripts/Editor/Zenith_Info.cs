using UnityEngine;
using UnityEditor;

namespace Zenith
{
    [InitializeOnLoad]
    public class Zenith_Info : EditorWindow
    {
        private const string Url = "https://github.com/Zenith-Productions/Zenith-Unity-Kit/";
        private const string Url1 = "https://trigon.systems/";
        private const string Link = "";
        private const string Link1 = "https://trigonstatus.statuspage.io";
        static Zenith_Info()
        {
            EditorApplication.update -= DoSplashScreen;
            EditorApplication.update += DoSplashScreen;
        }

        private static void DoSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            if (!EditorPrefs.HasKey("Zenith_ShowInfoPanel"))
            {
                EditorPrefs.SetBool("Zenith_ShowInfoPanel", true);
            }
            if (EditorPrefs.GetBool("Zenith_ShowInfoPanel"))
                OpenSplashScreen();
        }

        private static Vector2 changeLogScroll;
        private static GUIStyle ToolkitHeader;
        private static GUIStyle ZenithBottomHeader;
        private static GUIStyle ZenithHeaderLearnMoreButton;
        private static GUIStyle ZenithBottomHeaderLearnMoreButton;
        [MenuItem("Zenith/Info", false, 500)]
        public static void OpenSplashScreen()
        {
            GetWindow<Zenith_Info>(true);
        }
        
        public static void Open()
        {
            OpenSplashScreen();
        }

        public void OnEnable()
        {
            titleContent = new GUIContent("Zenith Info");
            
            minSize = new Vector2(400, 700);;
            ZenithBottomHeader = new GUIStyle();
            ToolkitHeader = new GUIStyle
            {
                normal =
                    {
                       background = Resources.Load("ZenithUtilsHeader") as Texture2D,
                       textColor = Color.white
                    },
                fixedHeight = 200
            };
        }

        public void OnGUI()
        {
            GUILayout.Box("", ToolkitHeader);
            ZenithHeaderLearnMoreButton = EditorStyles.miniButton;
            ZenithHeaderLearnMoreButton.normal.textColor = Color.black;
            ZenithHeaderLearnMoreButton.fontSize = 12;
            ZenithHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            Texture2D texture = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/UISprite.psd");
            ZenithHeaderLearnMoreButton.normal.background = texture;
            ZenithHeaderLearnMoreButton.active.background = texture;
            GUILayout.Space(4);
            GUI.backgroundColor = new Color(
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_R"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_G"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_B"),
            UnityEditor.EditorPrefs.GetFloat("ZPUKColor_A")
            );
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Zenith Productions Unity Kit"))
            {
                Application.OpenURL(Url);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Trigon.Systems"))
            {
                Application.OpenURL(Url1 + Link);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            //Update assets
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Status"))
            {
                Application.OpenURL(Link1);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Version 1.0.1"))
            {
                Application.OpenURL("https://github.com/Zenith-Productions/Zenith-Unity-Kit/blob/main/CHANGELOG.md");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Zenith Version 1.0.1");
            GUILayout.Space(2);
            GUILayout.Label("ZPUK imported correctly if you are seeing this");
            changeLogScroll = GUILayout.BeginScrollView(changeLogScroll, GUILayout.Width(390));

            GUILayout.Label(
@"
== Zentih Productions Unity Kit ==

This Unity Kit is hopefully providing everything you need

------------------------------------------------------------
∞∞∞∞∞∞∞∞∞∞∞∞Information∞∞∞∞∞∞∞∞∞∞∞∞
This unity kit provides tools and scripts for you
I am not responsible for misuse of these tools and scripts
The goal is to become the main package anyone needs
If you have issues visit the github repository issues tab
Updates can be done from within unity itself (or manually)
Bugs/Issues can be reported via github issues
There is not a discord for Zenith Productions

------------------------------------------------------------
∞∞∞∞∞∞∞Contributors to Zenith Unity Kit∞∞∞∞∞∞∞
> Developer: PhoenixAceVFX
- Contributor : WTFBlaze
============================================
");
            GUILayout.EndScrollView();
            GUILayout.Space(4);

            GUILayout.Box("", ZenithBottomHeader);
            ZenithBottomHeaderLearnMoreButton = EditorStyles.miniButton;
            ZenithBottomHeaderLearnMoreButton.normal.textColor = Color.black;
            ZenithBottomHeaderLearnMoreButton.fontSize = 10;
            ZenithBottomHeaderLearnMoreButton.border = new RectOffset(10, 10, 10, 10);
            ZenithBottomHeaderLearnMoreButton.normal.background = texture;
            ZenithBottomHeaderLearnMoreButton.active.background = texture;

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            EditorPrefs.SetBool("Zenith_ShowInfoPanel", GUILayout.Toggle(EditorPrefs.GetBool("Zenith_ShowInfoPanel"), "Show at startup"));
            GUILayout.EndHorizontal();
        }

    }
}