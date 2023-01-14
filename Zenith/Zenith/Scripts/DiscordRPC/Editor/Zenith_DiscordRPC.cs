using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Zenith
{
    [InitializeOnLoad]
    public class Zenith_DiscordRPC
    {
        private static readonly DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();

        private static TimeSpan time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        private static long timestamp = (long)time.TotalSeconds;

        private static RpcState rpcState = RpcState.EDITMODE;
        private static string GameName = Application.productName;
        private static string SceneName = SceneManager.GetActiveScene().name;

        static Zenith_DiscordRPC()
        {
            if (EditorPrefs.GetBool("Zenith_discordRPC", true))
            {
                ZenithLog("Starting discord rpc");
                DiscordRpc.EventHandlers eventHandlers = default(DiscordRpc.EventHandlers);
                DiscordRpc.Initialize("1063572408893710459", ref eventHandlers, false, string.Empty);
                UpdateDRPC();
            }
        }

        public static void UpdateDRPC()
        {
            ZenithLog("Updating everything");
            SceneName = SceneManager.GetActiveScene().name;
            presence.details = string.Format("Project: {0} Scene: {1}", GameName, SceneName);
            presence.state = "State: " + rpcState.StateName();
            presence.startTimestamp = timestamp;
            presence.largeImageKey = "zenith";
            presence.largeImageText = "ZenithUtils";
            presence.smallImageKey = "trigon";
            presence.smallImageText = "From Trigon.Systems";
            DiscordRpc.UpdatePresence(presence);
        }

        public static void updateState(RpcState state)
        {
            ZenithLog("Updating state to '" + state.StateName() + "'");
            rpcState = state;
            presence.state = "State: " + state.StateName();
            DiscordRpc.UpdatePresence(presence);
        }

        public static void sceneChanged(Scene newScene)
        {
            ZenithLog("Updating scene name");
            SceneName = newScene.name;
            presence.details = string.Format("Project: {0} Scene: {1}", GameName, SceneName);
            DiscordRpc.UpdatePresence(presence);
        }

        public static void ResetTime()
        {
            ZenithLog("Reseting timer");
            time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            timestamp = (long)time.TotalSeconds;
            presence.startTimestamp = timestamp;

            DiscordRpc.UpdatePresence(presence);
        }

        private static void ZenithLog(string message)
        {
            Debug.Log("[Zenith] DiscordRPC: " + message);
        }
    }
}