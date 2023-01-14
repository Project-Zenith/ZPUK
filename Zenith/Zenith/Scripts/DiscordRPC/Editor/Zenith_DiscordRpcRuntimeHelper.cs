using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Zenith
{
    [InitializeOnLoad]
    public static class Zenith_DiscordRpcRuntimeHelper
    {
        // register an event handler when the class is initialized
        static Zenith_DiscordRpcRuntimeHelper()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
            EditorSceneManager.activeSceneChanged += sceneChanged;
        }

        private static void sceneChanged(Scene old, Scene next)
        {
            Zenith_DiscordRPC.sceneChanged(next);
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredEditMode)
            {
                Zenith_DiscordRPC.updateState(RpcState.EDITMODE);
                Zenith_DiscordRPC.ResetTime();
            } else if(state == PlayModeStateChange.EnteredPlayMode)
            {
                Zenith_DiscordRPC.updateState(RpcState.PLAYMODE);
                Zenith_DiscordRPC.ResetTime();
            }
        }
    }
}