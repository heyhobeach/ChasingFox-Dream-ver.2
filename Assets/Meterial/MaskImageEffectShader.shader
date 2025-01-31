Shader "Hidden/MaskImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CoverColor ("Cover Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }

        Stencil { Ref 1 Comp NotEqual Fail Replace Pass Replace }
        Blend SrcAlpha OneMinusSrcAlpha // 투명도 처리

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _CoverColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                color.rgb = _CoverColor.rgb;
                color.a = _CoverColor.a;
                return color;
            }
            ENDCG
        }
    }
}
