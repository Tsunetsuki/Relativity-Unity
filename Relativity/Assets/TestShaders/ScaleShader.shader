﻿Shader "Unlit/ScaleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScaleBy("Scale By", float) = 1
        _TranslateXBy("Translate X By", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ScaleBy;
            float _TranslateXBy;

            v2f vert (appdata v)
            {
                v2f o;
                
                float4x4 scaleMat = float4x4(_ScaleBy, 0, 0, 0, 0, _ScaleBy, 0, 0, 0, 0, _ScaleBy, 0, 0, 0, 0, _ScaleBy);

                float4 transformedPos = mul(scaleMat, v.vertex) + float4(_TranslateXBy, 0, 0, 1);

                o.vertex = UnityObjectToClipPos(transformedPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
