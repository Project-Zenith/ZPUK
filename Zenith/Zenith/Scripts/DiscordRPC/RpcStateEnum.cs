namespace Zenith
{
    public static class RpcStateInfo
    {
        public static string StateName(this RpcState state)
        {
            switch (state)
            {
                case RpcState.EDITMODE: return "Edit mode";
                case RpcState.PLAYMODE: return "Play mode";
                case RpcState.UPLOADPANEL: return "Uploading Something";
                default: return "Error";
            }
        }
    }

        public enum RpcState
    {
        EDITMODE = 0,
        PLAYMODE = 1,
        UPLOADPANEL = 2
    }
}
