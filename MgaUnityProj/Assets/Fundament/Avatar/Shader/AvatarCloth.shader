Shader "MGA/Avatar/Cloth" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MainColor ("Color", Color) = (1,1,1,1)
	_SubColor ("Color", Color) = (1,1,1,1)
}

SubShader {
	Tags {"RenderType"="Opaque"}
	
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
			float4 _MainColor;
			float4 _SubColor;
			
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
				float rate = (col.r + col.g + col.b) * 0.333f;
				fixed3 cor = _SubColor * rate + _MainColor * (1.0f - rate);
				col.rgb = col.rgb * col.a + cor * (1.0f - col.a);

				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}
