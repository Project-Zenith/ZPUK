//https://docs.unity3d.com/ScriptReference/EditorApplication.SaveScene.html

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace AutoSaver
{
  [InitializeOnLoad]
    public class ZenithSplashScreen : EditorWindow
    {

        static ZenithSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            EditorApplication.update += DoSplashScreen;
        }

        private static void DoSplashScreen()
        {
            EditorApplication.update -= DoSplashScreen;
            if (!EditorPrefs.HasKey("ShowSplashScreen"))
            {
                EditorPrefs.SetBool("ShowSplashScreen", true);
            }
            if (EditorPrefs.GetBool("ShowSplashScreen"))
                OpenSplashScreen();
        }

        private static GUIStyle Header;
        private static Vector2 changeLogScroll;
		private float Timer = 60f;
		float TimeLeft = 0f;

        [MenuItem("Zenith/Scene AutoSave", false, 500)]
        private static void Init()
		{
            ZenithSplashScreen window = EditorWindow.GetWindowWithRect<ZenithSplashScreen>(new Rect(0, 0, 400,180), true);
			window.Show();
		}

        public static void OpenSplashScreen()
        {
            GetWindowWithRect<ZenithSplashScreen>(new Rect(0, 0, 400,180), true);
        }

        public static void Open()
        {
            OpenSplashScreen();
        }

        public void OnEnable()
        {

            titleContent = new GUIContent("Auto Save");
            maxSize = new Vector2(300, 100);

        }

        public void OnGUI()
        {

			EditorGUILayout.LabelField("Time:", Timer + " Secs");
			int timeToSave = (int)(TimeLeft - EditorApplication.timeSinceStartup);
			EditorGUILayout.LabelField("time left:", timeToSave.ToString() + " Sec");
			this.Repaint();
			if (EditorApplication.timeSinceStartup > TimeLeft)
			{
				string[] path = EditorApplication.currentScene.Split(char.Parse("/"));
				path[path.Length - 1] = "AutoSave_" + path[path.Length - 1];
				EditorApplication.SaveScene(string.Join("/", path), true);
				TimeLeft = (int)(EditorApplication.timeSinceStartup + Timer);
			}


			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.cyan;

			if (GUILayout.Button("Trigon"))
			{
				Application.OpenURL("https://trigon.systems/");
			}

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Github"))
			{
				Application.OpenURL("https://github.com/Zenith-Productions/ZPUK");
			}

			GUI.backgroundColor = Color.yellow;
			if (GUILayout.Button("PaleRa1n"))
			{
				Application.OpenURL("https://palera1n.cf/");
			}

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Support"))
			{
				Application.OpenURL("https://github.com/Zenith-Productions/ZPUK/issues");
			}

			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			GUILayout.Space(0);



			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.gray;

			if (GUILayout.Button("Status"))
			{
				Application.OpenURL("https://trigonstatus.statuspage.io/");
			}





			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();




			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			GUILayout.Space(0);

			changeLogScroll = GUILayout.BeginScrollView(changeLogScroll);
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            EditorPrefs.SetBool("ShowSplashScreen", GUILayout.Toggle(EditorPrefs.GetBool("ShowSplashScreen"), "Toggle AutoSave"));
            GUILayout.EndHorizontal();
        }

    }
}
#endif