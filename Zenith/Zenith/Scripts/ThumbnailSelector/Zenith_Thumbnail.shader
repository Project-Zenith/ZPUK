Shader "Zenith/ThumbnailCam" {
	Properties {
		_Overlay ("Color", 2D) = "grey" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _Overlay;
	half4 _UV_Transform = half4(1, 0, 0, 1);

	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = UnityObjectToClipPos( v.vertex );
		o.uv[0] = float2(
			dot( v.texcoord.xy, _UV_Transform.xy ),
			dot( v.texcoord.xy, _UV_Transform.zw )
		);
		o.uv[1] =  v.texcoord.xy;
		return o;
	}

	half4 fragOverride (v2f i) : SV_Target {
		return tex2D(_Overlay, i.uv[0]);
	}
	ENDCG 

	Subshader {
	ZTest Always Cull Off ZWrite Off
	Fog { Mode off }  
	ColorMask RGB

		Pass {
			  CGPROGRAM
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment fragOverride
			  ENDCG
		}
	}
Fallback off	
}