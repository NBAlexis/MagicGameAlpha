Shader "MGA/Scene/WaterPool" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Normal("Normal", 2D) = "bump" {}
	_ReflectionTex("_ReflectionTex", 2D) = "black" {}
	_DirectionUv("Wet scroll direction (2 samples)", Vector) = (1.0,1.0, -0.2,-0.2)
	_TexAtlasTiling("Tex atlas tiling", Vector) = (8.0,8.0, 4.0,4.0)
	_Color("Color", Color) = (1,1,1,1)
}

CGINCLUDE		

struct v2f_full
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half4 normalScrollUv : TEXCOORD1;	
	half4 screen : TEXCOORD2;
	half2 fakeRefl : TEXCOORD3;
};
	
#include "AngryInclude.cginc"		

half4 _DirectionUv;
half4 _TexAtlasTiling;

sampler2D _MainTex;
sampler2D _Normal;		
sampler2D _ReflectionTex;
sampler2D _FakeReflect;

float4 _Color;
			
ENDCG 

SubShader {
	Tags { "RenderType"="Transparent" }

	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

	Pass {
		CGPROGRAM
		
		float4 _MainTex_ST;
		
		v2f_full vert (appdata_full v) 
		{
			v2f_full o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			
			o.normalScrollUv.xyzw = v.texcoord.xyxy * _TexAtlasTiling + _Time.xxxx * _DirectionUv;
						
			o.fakeRefl = EthansFakeReflection(v.vertex);
			o.screen = ComputeScreenPos(o.pos);
				
			return o; 
		}
				
		fixed4 frag (v2f_full i) : COLOR0 
		{
			fixed4 nrml1 = tex2D(_Normal, i.normalScrollUv.xy);
			fixed4 nrml2 = tex2D(_Normal, i.normalScrollUv.zw);
			fixed4 nrml = (nrml1 - 0.5) * 0.02 +(nrml2 - 0.5) * 0.02;
			fixed4 rtRefl = tex2D (_ReflectionTex, (i.screen.xy / i.screen.w) + nrml.xy);
			fixed4 tex = tex2D (_MainTex, i.uv);
			tex = tex + tex.a * tex.a * rtRefl * (nrml1.z + nrml2.z - 1.0);

			return tex * _Color;	
		}	
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	}
} 

}
