Shader "MGA/Scene/Lightmapped Color Unlit"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0,0,1,1)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
	float4 _Color;

    struct appdata_t
    {
        float4 position : POSITION;
        float2 texcoord0 : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord0 : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        UNITY_FOG_COORDS(2)
    };

    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = mul(UNITY_MATRIX_MVP, v.position);
        o.texcoord0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
        o.texcoord1 = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
        UNITY_TRANSFER_FOG(o, o.position);
        return o;
    }

    fixed4 frag_ldr(v2f i) : SV_Target
    {
        fixed4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.texcoord1);
        fixed4 col = tex2D(_MainTex, i.texcoord0) * lm * 4.0f * _Color;
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
    }

    fixed4 frag_rgbm(v2f i) : SV_Target
    {
        half4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.texcoord1);
        fixed4 col = tex2D(_MainTex, i.texcoord0) * lm * lm.a * 16.0f * _Color;
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode" = "VertexLM" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_ldr
            #pragma multi_compile_fog
            ENDCG
        }
        Pass
        {
            Tags { "LightMode" = "VertexLMRGBM" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_rgbm
            #pragma multi_compile_fog
            ENDCG
        }
    }
}