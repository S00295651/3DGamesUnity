Shader "Custom/Terrain" {

    Properties {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }

    SubShader {
        Tags { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(testTexture);
            SAMPLER(sampler_testTexture);
            TEXTURE2D_ARRAY(baseTextures);
            SAMPLER(sampler_baseTextures);

            // Arrays CANNOT go in CBUFFER — leave them outside
            const static int maxLayerCount = 8;
            const static float epsilon = 1E-4;
            int layerCount;
            float3 baseColours[maxLayerCount];
            float baseStartHeights[maxLayerCount];
            float baseBlends[maxLayerCount];
            float baseColourStrength[maxLayerCount];
            float baseTextureScales[maxLayerCount];

            // Scalars MUST go inside CBUFFER for SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float minHeight;
                float maxHeight;
                float testScale;
            CBUFFER_END

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            float inverseLerp(float a, float b, float value) {
                return saturate((value - a) / (b - a));
            }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
                float3 scaledWorldPos = worldPos / scale;

                float3 xProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures,
                    float2(scaledWorldPos.y, scaledWorldPos.z), textureIndex).rgb * blendAxes.x;

                float3 yProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures,
                    float2(scaledWorldPos.x, scaledWorldPos.z), textureIndex).rgb * blendAxes.y;

                float3 zProjection = SAMPLE_TEXTURE2D_ARRAY(baseTextures, sampler_baseTextures,
                    float2(scaledWorldPos.x, scaledWorldPos.y), textureIndex).rgb * blendAxes.z;

                return xProjection + yProjection + zProjection;
            }

            half4 frag(Varyings IN) : SV_Target {
                float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);

                float3 blendAxes = abs(IN.worldNormal);
                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

                float3 colour = float3(0, 0, 0);

                for (int i = 0; i < layerCount; i++) {
                    float drawStrength = inverseLerp(
                        -baseBlends[i] / 2 - epsilon,
                         baseBlends[i] / 2,
                        heightPercent - baseStartHeights[i]
                    );

                    float3 baseColour    = baseColours[i] * baseColourStrength[i];
                    float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i)
                                          * (1 - baseColourStrength[i]);

                    colour = colour * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
                }
                //return half4(heightPercent, heightPercent, heightPercent, 1.0);
                //return half4(layerCount / 9.0, 0, 0, 1);
                //return half4(heightPercent, heightPercent, heightPercent, 1);
                return half4(colour, 1.0);
            }

            ENDHLSL
        }
    }
}