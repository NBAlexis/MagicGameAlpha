
Shader "MGA/Scene/Rotate 2 Layer Additive" {
Properties {
	_MainTex ("1st layer (RGB)", 2D) = "white" {}
	_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
	_Speed1X ("1st speed X", Float) = 1.0
	_Offset1X ("1st offset X", Float) = 1.0
	_Speed1Y ("1st speed Y", Float) = 1.0
    _Offset1Y ("1st offset Y", Float) = 1.0
	_Speed2X ("2nd speed X", Float) = 1.0
	_Offset2X ("2nd offset X", Float) = 1.0
	_Speed2Y ("2nd speed Y", Float) = 1.0
    _Offset2Y ("2nd offset Y", Float) = 1.0

	_Color("Color", Color) = (1,1,1,1)
	_MMultiplier ("Layer Multiplier", Float) = 1.0
}

	
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend One One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	LOD 100
	
	
	
	CGINCLUDE
	#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	#pragma exclude_renderers molehill    
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _DetailTex;

	float4 _MainTex_ST;
	float4 _DetailTex_ST;
	
	float _Speed1X;
	float _Speed1Y;
	float _Offset1X;
	float _Offset1Y;
	float _Speed2X;
	float _Speed2Y;
	float _Offset2X;
	float _Offset2Y;

	float _MMultiplier;
	float4 _Color;

	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		fixed4 color : TEXCOORD1;
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv.x = v.texcoord.x + _Offset1X + _Speed1X * _Time;
		o.uv.y = v.texcoord.y + _Offset1Y + _Speed1Y * _Time;
		o.uv.z = v.texcoord.x + _Offset2X + _Speed2X * _Time;
		o.uv.w = v.texcoord.y + _Offset2Y + _Speed2Y * _Time;
		
		o.color = _MMultiplier.xxxx * _Color;
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		//#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
			
			o = (tex + tex2) * i.color;
						
			return o;
		}
		ENDCG 
	}	
}
}
