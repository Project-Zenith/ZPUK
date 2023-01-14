using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zenith.ThumbnailSelector
{
    public class Zenith_ThumbnailSelector : MonoBehaviour {
        private bool bAddScript = false;
        public Texture2D texture;
	
	    void Update () {
            if (false == bAddScript) {
                GameObject obj = GameObject.Find("VRCCam");
                if (null != obj)
                {
                    bAddScript = true;
                    obj.AddComponent<Zenith_ThumbnailOverlay>();
                    Zenith_ThumbnailOverlay script = obj.GetComponent<Zenith_ThumbnailOverlay>();
                    if (null == script) {
                        Debug.Log("Zenith_ThumbnailOverlay Script not Found");
                        return;
                    }
                    script.enabled = false;
                    script.SetTextuer(texture);
                }
            }
        }
    }
}
