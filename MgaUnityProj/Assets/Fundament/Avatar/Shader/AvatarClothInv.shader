﻿Shader "MGA/Avatar/ClothInv" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_ClothColor ("Color", Color) = (1,1,1,1)
}

SubShader {
	//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Tags {"RenderType"="Opaque"}
	//LOD 100
	
	//ZWrite Off
	//Blend SrcAlpha OneMinusSrcAlpha 

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _ClothColor;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				col.rgb = col.rgb * col.a + _ClothColor.rgb * (1.0f - col.a);

				//col.rgb = saturate(col.rgb * 0.5f + _ClothColor.rgb * 0.5f - fixed3(0.5f, 0.5f, 0.5f)) * 0.5f + fixed3(0.3f, 0.3f, 0.3f);
				col.rgb = (col.rgb * 0.5f + _ClothColor.rgb * 0.5f - fixed3(0.5f, 0.5f, 0.5f)) * 0.2f + fixed3(0.2f, 0.2f, 0.2f);

				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}