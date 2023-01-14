using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zenith.ThumbnailSelector
{
    public class Zenith_ThumbnailOverlay : MonoBehaviour
    {
        private Texture2D texture;
        private Material material;

        public void SetTextuer(Texture2D texture)
        {
            if (null == texture)
            {
                Debug.Log("No texture found");
                return;
            }
            this.texture = texture;

            if (null == material)
            {
                Shader overlayShader = Shader.Find("Zenith/ThumbnailCam");
                if (null == overlayShader)
                {
                    Debug.Log("ThumbnailCam Not Found");
                    return;
                }

                material = new Material(overlayShader);
                if (null == material)
                {
                    Debug.Log("Material Create Faild");
                    return;
                }
            }
            material.SetTexture("_Overlay", texture);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (null == material)
            {
                return;
            }

            material.SetVector("_UV_Transform", new Vector4(1, 0, 0, 1));
            material.SetTexture("_Overlay", texture);
            Graphics.Blit(src, dest, material, 0);
        }
    }
}