Shader "Custom/Choice"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FilterStrength ("Filter Strength", Range(0, 1)) = 1
        _PrevFilterType ("Prev Filter Type", Float) = 0
        _NextFilterType ("Next Filter Type", Float) = 0
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _FilterStrength;
            float _PrevFilterType;
            float _NextFilterType;
            float _BlendAmount;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            fixed3 ApplyEmotionFilter(float type, fixed3 rgb)
            {
                float gray = dot(rgb, float3(0.3, 0.59, 0.11));
                fixed3 result = rgb;

                if (type == 1) // 슬픔
                {
                    result = lerp(rgb, float3(gray * 0.7, gray * 0.8, gray + 0.2), 0.8);
                }
                else if (type == 2) // 분노
                {
                    result = float3(rgb.r * 1.5, rgb.g * 0.5, rgb.b * 0.5);
                }
                else if (type == 3) // 공포
                {
                    result = float3(rgb.r * 0.6, rgb.g * 0.8, rgb.b * 1.2) * 0.7;
                }
                else if (type == 4) // 억눌린 분노
                {
                    float g = dot(rgb, float3(0.299, 0.587, 0.114));
                    result = float3(g + 0.2, g, g);
                }
                else if (type == 5) // 사랑
                {
                    result = rgb + float3(0.2, 0.05, 0.1);
                }
                else if (type == 6) // 당황
                {
                    result = rgb + float3(0.3, 0.15, 0.05);
                }
                else if (type == 7) // 자부심
                {
                    result = saturate(rgb * 1.3 + float3(0.2, 0.15, 0.05));
                }
                else if (type == 8) // 무기력
                {
                    result = rgb * 0.6;
                }
                else if (type == 9) // 공허
                {
                    result = lerp(rgb, float3(gray, gray, gray), 0.85);
                }
                else if (type == 10) // 평온
                {
                    result = rgb + float3(0.1, 0.1, -0.05);
                }

                return saturate(result);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                fixed3 prevFiltered = ApplyEmotionFilter(_PrevFilterType, col.rgb);
                fixed3 nextFiltered = ApplyEmotionFilter(_NextFilterType, col.rgb);
                fixed3 blended = lerp(prevFiltered, nextFiltered, _BlendAmount);
                col.rgb = lerp(col.rgb, blended, _FilterStrength);

                return col;
            }
            ENDCG
        }
    }
}
