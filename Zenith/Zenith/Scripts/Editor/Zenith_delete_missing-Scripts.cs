using System;
using UnityEditor;
using UnityEngine;
using Zenith;

namespace Zenith
{
    public class Zenith_delete_missing_Scripts
    {
        [MenuItem("Zenith/Recovery/Delete Missing Scripts In GameObject")]
        private static void OnEnable()
        {
            try
            {
                var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
                int compCount = 0;
                int goCount = 0;
                foreach (var o in deepSelection)
                {
                    if (o is GameObject go)
                    {
                        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        if (count > 0)
                        {
                            Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                            compCount += count;
                            goCount++;
                        }
                    }
                }
                Debug.Log($"Found {compCount} missing Scripts from {goCount} Gameobjects - All of them got Deleted.");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Zenith", "delete_missing-Scripts.InGameObject: " + ex.Message, "Okay");
            }
        }
    }
}
