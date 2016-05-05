Shader "MGA/Avatar/Cloth" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_ColorTex ("Color (RGB)", 2D) = "white" {}
	_ShadowTex ("Shadow (RGB)", 2D) = "white" {}
	_MainColor ("Main Color", Color) = (1,1,1,1)
	_SubColor ("Sub Color", Color) = (1,1,1,1)
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
			sampler2D _ColorTex;
			float4 _ColorTex_ST;
			sampler2D _ShadowTex;
			float4 _ShadowTex_ST;

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
				fixed4 ratecol = tex2D(_ColorTex, i.texcoord);
				fixed shadowcol = saturate((tex2D(_ShadowTex, i.texcoord).r + 0.2f) * (1.0f + ratecol.b * ratecol.r));
				fixed rate = ratecol.b;
				fixed3 cor = _SubColor * ratecol.g + _MainColor * ratecol.r;
				col.rgb = col.rgb * (1.0f - rate) + cor * rate;
				col.rgb = 1.5f * shadowcol * col.rgb;

				fixed3 invc = (col.rgb - fixed3(0.5f, 0.5f, 0.5f)) * 0.25f + 0.15f * _MainColor.rgb + fixed3(0.2f, 0.2f, 0.2f);
				col.rgb = col.rgb * _MainColor.a + invc * (1.0f - _MainColor.a);

				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}
