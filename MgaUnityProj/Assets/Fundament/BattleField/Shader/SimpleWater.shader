
Shader "MGA/Scene/Very Simple Water" {
Properties {
	_MainTex ("Base layer (RGB)", 2D) = "white" {}
    _Reflection ("Reflection", Cube) = "white" {}

	_ScrollX ("Base layer Scroll speed X", Float) = -0.51
	_ScrollY ("Base layer Scroll speed Y", Float) = 0.025
	_Scroll2X ("2nd layer Scroll speed X", Float) = 0.42
	_Scroll2Y ("2nd layer Scroll speed Y", Float) = -0.031
	_SineAmplX ("Base layer sine amplitude X",Float) = 1
	_SineAmplY ("Base layer sine amplitude Y",Float) = 1
	_SineFreqX ("Base layer sine freq X",Float) = 1
	_SineFreqY ("Base layer sine freq Y",Float) = 1
	_SineAmplX2 ("2nd layer sine amplitude X",Float) = 0
	_SineAmplY2 ("2nd layer sine amplitude Y",Float) = 0
	_SineFreqX2 ("2nd layer sine freq X",Float) = 0
	_SineFreqY2 ("2nd layer sine freq Y",Float) = 0

    _Color("Color", Color) = (0,0,1,1)
	_MMultiplier ("Wave Multiplier", Float) = 0.2
    _RMultiplier ("Reflect Multiplier", Float) = 0.2
    _Alpha ("Alpha", Float) = 0.6
}

	
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
    Lighting Off ZWrite Off
	
	LOD 100

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
    samplerCUBE _Reflection;
    
	float4 _MainTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _Scroll2X;
	float _Scroll2Y;
	float _MMultiplier;
    float _RMultiplier;
	
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

	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float3 uv2 : TEXCOORD2;
	};

	v2f vert (appdata_img v)
	{
		v2f o;
        float2 multp = float2(1.1, 1.2);
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
        o.uv.zw = TRANSFORM_TEX(v.texcoord.xy * multp,_MainTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		
		o.uv.x += sin(_Time * _SineFreqX) * _SineAmplX;
		o.uv.y += sin(_Time * _SineFreqY) * _SineAmplY;
		
		o.uv.z += sin(_Time * _SineFreqX2) * _SineAmplX2;
		o.uv.w += sin(_Time * _SineFreqY2) * _SineAmplY2;
        
        o.uv2 = o.pos.xyz;
		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_particles
		#pragma multi_compile_fog

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy).aaaa;
            tex.a = 1.0;
            fixed4 tex2 = tex2D (_MainTex, i.uv.zw).aaaa;
            tex2.a = 1.0;

            fixed3 tex3 = tex2D (_MainTex, i.uv.yx).aaa;
            fixed3 tex4 = tex2D (_MainTex, i.uv.wz).aaa * tex3;
            float factor1 = 1.0 - _Color.a;
            float factor2 = _Color.a;
            fixed4 reflectcolor = texCUBE (_Reflection, i.uv2);
            float4 reflectfactor = tex4.r + tex4.g + tex4.b;
            reflectfactor = reflectfactor * reflectfactor * 2.0;
            
            float3 wave_color = tex * tex2;
            float wavefactor = (tex.r + tex.g + tex.b) * (tex.r + tex.g + tex.b) * 0.3;
			o.rgb = 
			wave_color * wavefactor * _MMultiplier * _Color.rgb 
			+ _Color.rgb * factor2 + reflectcolor * reflectfactor * _RMultiplier;
            o.a = _Alpha;
						
			UNITY_APPLY_FOG(i.fogCoord, o);
			return o;
		}
		ENDCG 
	}	
}
}
