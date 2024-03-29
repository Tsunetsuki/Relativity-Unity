﻿Shader "Unlit/ConstantVelocityLorentz"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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

            //Set by observer
            float4x4 _LorentzMatrix;
            float4x4 _LorentzMatrixInverse;
            float4 _ObserverPos;//unncessary, can use _ObserverFrame instead?
            float4 _ObserverVel;
            float4 _ObservedFramePos;

            //Set by observees individually
            float4 _OriginalPos; //unnecessary?
            float4 _OriginalVel;
            
            float4 _IndivColor;

            float3 LinePlaneIntersect(float3 point_on_line, float3 line_direction, float3 point_on_plane, float3 plane_normal) {
                float a = dot(point_on_plane - point_on_line, plane_normal);
                float b = dot(line_direction, plane_normal);

                if (b == 0) {
                    return float3(0, 0, 0);
                }
                else
                {
                    float x = a / b;
                    return x * line_direction + point_on_line;
                }
            }


            v2f vert(appdata v)
            {
                v2f o;

                //Lorentz Transformation: First undoes L-Tf on object (origin) position, then redoes it with the specific vertex
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz; // object center
                float3 relToObjectPos = worldPos - objectPos; // vertex rel to center

                float3 untransformedVertexPos = _OriginalPos.xyz + relToObjectPos;
                float3 projectedVertexPos = LinePlaneIntersect(untransformedVertexPos, _OriginalVel, _ObserverPos, _ObserverVel);
                float3 transformedProjectedVertexPos = mul(_LorentzMatrix, projectedVertexPos - _ObserverPos);

                float3 transformedPos = _ObservedFramePos + transformedProjectedVertexPos;

                float3 finalPos = transformedPos;

                o.vertex = UnityWorldToClipPos(finalPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);//_IndivColor
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
