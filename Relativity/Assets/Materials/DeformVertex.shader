Shader "Unlit/DeformVertex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            float4x4 _LorentzMatrix;
            float4x4 _LorentzMatrixInverse;
            float4 _ObserverPos;
            float4 _ObserverFramePos;

            v2f vert (appdata v)
            {
                v2f o;
                
                //Lorentz Transformation
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                float4 relToObjectPos = worldPos - objectPos;

                float4 untransformedObjectPos = mul(_LorentzMatrixInverse, objectPos - _ObserverFramePos);
                float4 transformedPos = _ObserverFramePos + mul(_LorentzMatrix, untransformedObjectPos + relToObjectPos);

                o.vertex = UnityWorldToClipPos(transformedPos);
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
