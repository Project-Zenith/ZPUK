using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
    public class ESR : EditorWindow
    {


        private static int _goCount;
        private static int _componentsCount;
        private static int _missingCount;

        private static bool _bHaveRun;

        [MenuItem("Zenith/Recovery/Missing Scripts Remover")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ESR));
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Remove Empty Scripts"))
            {
                FindInSelected();
            }

            if (!_bHaveRun) return;

            EditorGUILayout.TextField($"{_goCount} GameObjects Selected");
            if (_goCount > 0) EditorGUILayout.TextField($"{_componentsCount} Components");
            if (_goCount > 0) EditorGUILayout.TextField($"{_missingCount} Deleted");
        }

        private static void FindInSelected()
        {
            var go = Selection.gameObjects;
            _goCount = 0;
            _componentsCount = 0;
            _missingCount = 0;
            foreach (var g in go)
            {
                FindInGo(g);
            }

            _bHaveRun = true;
            Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");

            AssetDatabase.SaveAssets();
        }

        private static void FindInGo(GameObject g)
        {
            _goCount++;
            var components = g.GetComponents<Component>();

            var r = 0;

            for (var i = 0; i < components.Length; i++)
            {
                _componentsCount++;
                if (components[i] != null) continue;
                _missingCount++;
                var s = g.name;
                var t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }

                Debug.Log($"{s} has a missing script at {i}", g);

                var serializedObject = new SerializedObject(g);

                var prop = serializedObject.FindProperty("m_Component");

                prop.DeleteArrayElementAtIndex(i - r);
                r++;

                serializedObject.ApplyModifiedProperties();
            }

            foreach (Transform childT in g.transform)
            {
                FindInGo(childT.gameObject);
            }
        }

    }
#endif
