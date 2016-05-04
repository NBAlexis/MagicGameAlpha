Shader "MGA/Avatar/Cloth" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_ClothColor ("Color", Color) = (1,1,1,1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	//Tags {"RenderType"="Opaque"}
	//LOD 100
	
	//ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

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
				fixed aofrgb = sign(col.a + col.r + col.g + col.b - 0.2f) * 0.5f + 0.5f;
				col.rgb = col.rgb * aofrgb + _ClothColor.rgb * (1.0f - aofrgb);
				col.a = col.a * aofrgb + (1.0f - aofrgb);
				col.a = col.a * _ClothColor.a;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}
