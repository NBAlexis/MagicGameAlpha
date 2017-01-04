
Shader "MGA/Scene/Very Simple Under Water" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
    _SubTex ("Ground", 2D) = "white" {}

	_ScrollX ("Base layer Scroll speed X", Float) = 1.0
	_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
	_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
	_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
	_SineAmplX ("Base layer sine amplitude X",Float) = 0.5 
	_SineAmplY ("Base layer sine amplitude Y",Float) = 0.5
	_SineFreqX ("Base layer sine freq X",Float) = 10 
	_SineFreqY ("Base layer sine freq Y",Float) = 10
	_SineAmplX2 ("2nd layer sine amplitude X",Float) = 0.5 
	_SineAmplY2 ("2nd layer sine amplitude Y",Float) = 0.5
	_SineFreqX2 ("2nd layer sine freq X",Float) = 10 
	_SineFreqY2 ("2nd layer sine freq Y",Float) = 10

    _Color("Color", Color) = (1,1,1,1)
    _Alpha ("Alpha", Float) = 0.6
}

	
SubShader {
	Tags { "RenderType"="Opaque" }
	
	LOD 100

	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#include "UnityCG.cginc"
	sampler2D _MainTex;
    sampler2D _SubTex;
    
	float4 _MainTex_ST;
	float4 _SubTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _Scroll2X;
	float _Scroll2Y;
	
	float _SineAmplX;
	float _SineAmplY;
	float _SineFreqX;
	float _SineFreqY;

	float _SineAmplX2;
	float _SineAmplY2;
	float _SineFreqX2;
	float _SineFreqY2;

    float4 _Color;
    float _Alpha;

	struct Vertex
	{
		float4 vertex : POSITION;
		float4 uv : TEXCOORD0;
		float4 uv2 : TEXCOORD1;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float2 uv2 : TEXCOORD2;
		float2 lmuv : TEXCOORD3;
		UNITY_FOG_COORDS(1)
	};

	v2f vert (Vertex v)
	{
		v2f o;
        float2 multp = float2(1.1, 1.2);
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
        o.uv.zw = TRANSFORM_TEX(v.uv.xy * multp, _MainTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
		o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
		o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
		o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
		o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
        
        o.uv2 = TRANSFORM_TEX(v.uv.xy, _SubTex);
		o.lmuv = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;

		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}
	ENDCG

	Pass
	{
		Tags { "LightMode" = "VertexLM" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		fixed4 frag (v2f i) : SV_Target
		{
			fixed4 o;
            fixed3 tex3 = tex2D (_MainTex, i.uv.yx).rgb;
            fixed3 tex4 = tex2D (_MainTex, i.uv.wz * float2(1.1, 0.9)).rgb * tex3;
			tex4 = tex4 * tex4;
            fixed4 groundcolor = tex2D (_SubTex, i.uv2);
			o.rgb = groundcolor * _Color.rgb * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv) * 4.0f + tex4 * _Alpha * _Color.rgb * 2.0f;
			UNITY_APPLY_FOG(i.fogCoord, o);
			UNITY_OPAQUE_ALPHA(o.a);
			return o;
		}
		ENDCG
	}
	Pass
	{
		Tags { "LightMode" = "VertexLMRGBM" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		fixed4 frag (v2f i) : SV_Target
		{
			fixed4 o;
            fixed3 tex3 = tex2D (_MainTex, i.uv.yx).rgb;
            fixed3 tex4 = tex2D (_MainTex, i.uv.wz * float2(1.1, 0.9)).rgb * tex3;
			tex4 = tex4 * tex4;
            fixed4 groundcolor = tex2D (_SubTex, i.uv2);
			half4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmuv);
			o.rgb = groundcolor * _Color.rgb * lm * lm.a * 16.0f + tex4 * _Alpha * _Color.rgb * 2.0f;
			UNITY_APPLY_FOG(i.fogCoord, o);
			UNITY_OPAQUE_ALPHA(o.a);
			return o;
		}
		ENDCG
	}		
}
}
