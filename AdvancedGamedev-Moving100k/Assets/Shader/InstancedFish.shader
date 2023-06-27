// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/InstancedFish"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling

            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"
            #include "UnityLightingCommon.cginc"
            #include "FishSwarm.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD0;
                float4 diff : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            StructuredBuffer<FishData> _FishSwarmData;
            StructuredBuffer<float4x4> _FishSwarmTransforms;


            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                float4 worldPos = 0;
#if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float4 objPos = v.vertex * _FishSwarmData[unity_InstanceID].scale;
                objPos.w = 1.0f;
                worldPos = mul(_FishSwarmTransforms[unity_InstanceID], objPos);
                o.color = _FishSwarmData[unity_InstanceID].color;
    
                float3 worldNormal = mul((float3x3)_FishSwarmTransforms[unity_InstanceID], v.normal);
                float nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
    
                o.diff.rgb += ShadeSH9(float4(worldNormal, 1));
    
#else
                o.color = float4(1, 1, 1, 1);
                o.diff = 0;
#endif
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
    

                UNITY_TRANSFER_FOG(o, o.vertex);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = i.color * i.diff;
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
