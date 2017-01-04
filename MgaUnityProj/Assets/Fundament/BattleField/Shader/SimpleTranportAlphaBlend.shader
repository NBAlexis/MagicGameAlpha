Shader "MGA/Scene/SimpleTransparentAlphaBlend"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TintColor   ("TintColor", Color) = (1,1,1,1)
		_SpeedX   ("SpeedX", Float) = 1
		_SpeedY   ("SpeedY", Float) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
		
	uniform fixed4 _MainTex_ST;
	uniform sampler2D _MainTex;
	uniform fixed4 _TintColor;
	uniform fixed _SpeedX;
	uniform fixed _SpeedY;
	
	struct v2f
	{
		fixed4 pos : POSITION;
		fixed2 uv : TEXCOORD0;
	};

	v2f vert (appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);	
		o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.uv.x = o.uv.x + _Time * _SpeedX;
		o.uv.y = o.uv.y + _Time * _SpeedY;
		return o; 
	}

	fixed4 frag (v2f i) : COLOR 
	{
	    fixed4 retcolor = tex2D(_MainTex, i.uv.xy).xyzw;
		return retcolor * retcolor * _TintColor * 2;
	}
	
	ENDCG	
	Category {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			//Blend OneMinusDstColor One    
			Blend SrcAlpha One
			Cull Off Lighting Off ZWrite Off Fog { Mode Off }
			
//			BindChannels {
//				Bind "TintColor", color
//				Bind "Vertex", vertex
//				Bind "TexCoord", texcoord
//			}

	SubShader 
	{
		Pass 
		{

		     

			
			CGPROGRAM	
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
				
			ENDCG	
	
		}
		
	}
    }
	Fallback "Mobile/VertexLit"
}


