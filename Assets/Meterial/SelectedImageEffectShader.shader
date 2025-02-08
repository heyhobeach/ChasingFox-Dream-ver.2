Shader "Hidden/SelectedImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
        [HDR]_HoveredColor ("Hovered Color", Color) = (1, 0, 0, 1)
        [HDR]_SelectedColor ("Selected Color", Color) = (0, 1, 0, 1)
        [Toggle] _Selected ("Selected", float) = 0 
        _OutlineThickness ("Outline Thickness", Range(0, 5)) = 0.001
    }
    SubShader
    {
        Pass
        {
            Tags { "Queue"="Transparent" }
            Stencil { Ref 1 Comp Equal }
            ZWrite Off ZTest Always
    
            Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정
            Cull Off // 양면 렌더링 설정

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _OutlineThickness;

            fixed4 _HoveredColor;
            fixed4 _SelectedColor;

            sampler2D _MainTex;
            sampler2D _GlowTex;
            sampler2D _OutlineTex;

            float4 _MainTex_TexelSize;

            #include "UnityCG.cginc"

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

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Selected)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                fixed4 glowTexColor = tex2D(_MainTex, i.uv);

                fixed4 finalColor = mainTexColor;
                
                glowTexColor.rgb = _HoveredColor.rgb;
                finalColor = lerp(finalColor, glowTexColor, glowTexColor.a);
                finalColor.a *= _HoveredColor.a;

                if (UNITY_ACCESS_INSTANCED_PROP(Props, _Selected) > 0.5)
                {
                    fixed4 outlineTexColor = fixed4(0, 0, 0, 0);
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            float2 offset = float2(x, y) * _MainTex_TexelSize * _OutlineThickness;
                            outlineTexColor += tex2D(_MainTex, i.uv + offset);
                        }
                    }
                    outlineTexColor.a = clamp(outlineTexColor.a, 0, 1);
                    outlineTexColor.a *= _SelectedColor.a;
                    outlineTexColor.a -= tex2D(_MainTex, i.uv).a;

                    outlineTexColor.rgb = _SelectedColor.rgb;
                    finalColor = lerp(finalColor, outlineTexColor, outlineTexColor.a);
                }

                return finalColor;
            }
            ENDCG
        }


    }
}
