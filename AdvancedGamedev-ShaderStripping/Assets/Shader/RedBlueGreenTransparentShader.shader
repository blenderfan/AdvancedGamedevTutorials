Shader "ShaderStripping/RedBlueGreenTransparentShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode"="ForwardBase" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_fragment RED BLUE GREEN YELLOW
            #pragma shader_feature_fragment OPAQUE SEMI_TRANSPARENT 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            struct v2f
            {
                float4 pos : SV_POSITION;
                SHADOW_COORDS(1)
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
    
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal, 1));

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed3 lighting = i.diff * shadow + i.ambient;

                fixed4 red = fixed4(1.0f, 0.1f, 0.1f, 1.0f);
                fixed4 blue = fixed4(0.1f, 0.1f, 1.0f, 1.0f);
                fixed4 green = fixed4(0.1f, 1.0f, 0.1f, 1.0f);
                fixed4 yellow = fixed4(1.0f, 1.0f, 0.1f, 1.0f);
    
            #if RED
                fixed4 col = red;
            #elif BLUE
                fixed4 col = blue;
            #elif GREEN
                fixed4 col = green;
            #else
                fixed4 col = yellow;
            #endif

            #if SEMI_TRANSPARENT
                col.a = 0.5f;
            #elif OPAQUE
                col.a = 1.0f;
            #endif

                return fixed4(col.rgb * lighting, col.a);
            }
            ENDCG
        }
    }
}
