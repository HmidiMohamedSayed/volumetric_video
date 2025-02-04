Shader "Point Cloud/Lit Point"
{
    Properties
    {
        _Ambient("Ambient", Float) = 0.5
        _Metallic("Metallic", Float) = 0
        _PointSize("PointSize", Float) = 1.0
        _Smoothness("Smoothness", Float) = 0
        [HideInInspector]_BUILTIN_QueueOffset("Float", Float) = 0
        [HideInInspector]_BUILTIN_QueueControl("Float", Float) = -1
    }
    SubShader
    {

        HLSLINCLUDE

        half _PointSize = 1.0f;

        struct GeomData
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : INTERP0;
            float3 normalWS : INTERP1;
            float4 tangentWS : INTERP2;
            float4 color : INTERP3;
            float3 viewDirectionWS : INTERP4;
            float2 lightmapUV : INTERP5;
            float3 sh : INTERP6;
            float4 fogFactorAndVertexLight : INTERP7;
            float4 shadowCoord : INTERP8;   
        };

        [maxvertexcount(6)]
        void geom(point GeomData i[1], inout TriangleStream<GeomData> OutputStream)
        {
            GeomData p = i[0];
            float s = _PointSize*0.01;
            s = s* 0.5f; 

            p.positionCS = i[0].positionCS + float4(s,s,0,0);
            OutputStream.Append(p);
            p.positionCS = i[0].positionCS + float4(s,-s,0,0);
            OutputStream.Append(p);
            p.positionCS = i[0].positionCS + float4(-s,s,0,0);
            OutputStream.Append(p);
            p.positionCS = i[0].positionCS + float4(-s,-s,0,0);
            OutputStream.Append(p);
            OutputStream.RestartStrip();
        }


        ENDHLSL



        Tags
        {
            // RenderPipeline: <None>
            "RenderType"="Opaque"
            "BuiltInMaterialType" = "Lit"
            "Queue"="Geometry"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="BuiltInLitSubTarget"
        }

        Pass
        {
            Name "BuiltIn Forward"
            Tags
            {
                "LightMode" = "ForwardBase"
            }
        
            // Render State
            Cull Off
            Blend One Zero
            ZTest LEqual
            ZWrite On
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma geometry geom
            #pragma vertex vert
            #pragma fragment frag
            
            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            // GraphKeywords: <None>
            
            // Defines
            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_COLOR
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            #define BUILTIN_TARGET_API 1
            #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
            #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
            #endif
            #ifdef _BUILTIN_ALPHATEST_ON
            #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
            #endif
            #ifdef _BUILTIN_AlphaClip
            #define _AlphaClip _BUILTIN_AlphaClip
            #endif
            #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
            #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
            #endif
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv1 : TEXCOORD1;
                float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 tangentWS;
                float4 color;
                #if defined(LIGHTMAP_ON)
                float2 lightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh;
                #endif
                float4 fogFactorAndVertexLight;
                float4 shadowCoord;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            struct SurfaceDescriptionInputs
            {
                float3 TangentSpaceNormal;
                float4 VertexColor;
            };
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if defined(LIGHTMAP_ON)
                float2 lightmapUV : INTERP0;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh : INTERP1;
                #endif
                float4 tangentWS : INTERP2;
                float4 color : INTERP3;
                float4 fogFactorAndVertexLight : INTERP4;
                float4 shadowCoord : INTERP5;
                float3 positionWS : INTERP6;
                float3 normalWS : INTERP7;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            PackedVaryings PackVaryings (Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                output.lightmapUV = input.lightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                output.tangentWS.xyzw = input.tangentWS;
                output.color.xyzw = input.color;
                output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
                output.shadowCoord.xyzw = input.shadowCoord;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            Varyings UnpackVaryings (PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                output.lightmapUV = input.lightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                output.tangentWS = input.tangentWS.xyzw;
                output.color = input.color.xyzw;
                output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
                output.shadowCoord = input.shadowCoord.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float _Ambient;
            float _Metallic;
            float _Smoothness;
            CBUFFER_END
            
            
            // Object and Global properties
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // Graph Functions
            
            void Unity_ColorspaceConversion_RGB_RGB_float(float3 In, out float3 Out)
            {
                Out = In;
            }
            
            void Unity_Clamp_float(float In, float Min, float Max, out float Out)
            {
                Out = clamp(In, Min, Max);
            }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 NormalTS;
                float3 Emission;
                float Metallic;
                float Smoothness;
                float Occlusion;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float3 _ColorspaceConversion_d571041f83164f938fad01043ac2d408_Out_1_Vector3;
                Unity_ColorspaceConversion_RGB_RGB_float((IN.VertexColor.xyz), _ColorspaceConversion_d571041f83164f938fad01043ac2d408_Out_1_Vector3);
                float _Property_671091e6d32243f99bcf27f603f5283c_Out_0_Float = _Metallic;
                float _Clamp_cff9303d2dd64dc790c7a873c2e46f0c_Out_3_Float;
                Unity_Clamp_float(_Property_671091e6d32243f99bcf27f603f5283c_Out_0_Float, 0, 1, _Clamp_cff9303d2dd64dc790c7a873c2e46f0c_Out_3_Float);
                float _Property_f5fe0d5bc7374afa8b68b762c1dd6a72_Out_0_Float = _Smoothness;
                float _Clamp_cdb4cdf5c20643d2b3347f206ffecf64_Out_3_Float;
                Unity_Clamp_float(_Property_f5fe0d5bc7374afa8b68b762c1dd6a72_Out_0_Float, 0, 1, _Clamp_cdb4cdf5c20643d2b3347f206ffecf64_Out_3_Float);
                float _Property_48416796d77440a19ef3a874a7df4567_Out_0_Float = _Ambient;
                float _Clamp_6c447c37bac54a4c976247272cd615c1_Out_3_Float;
                Unity_Clamp_float(_Property_48416796d77440a19ef3a874a7df4567_Out_0_Float, 0, 1, _Clamp_6c447c37bac54a4c976247272cd615c1_Out_3_Float);
                surface.BaseColor = _ColorspaceConversion_d571041f83164f938fad01043ac2d408_Out_1_Vector3;
                surface.NormalTS = IN.TangentSpaceNormal;
                surface.Emission = float3(0, 0, 0);
                surface.Metallic = _Clamp_cff9303d2dd64dc790c7a873c2e46f0c_Out_3_Float;
                surface.Smoothness = _Clamp_cdb4cdf5c20643d2b3347f206ffecf64_Out_3_Float;
                surface.Occlusion = _Clamp_6c447c37bac54a4c976247272cd615c1_Out_3_Float;
                return surface;
            }
            
            // --------------------------------------------------
            // Build Graph Inputs
            
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =                          input.normalOS;
                output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                output.ObjectSpacePosition =                        input.positionOS;
            
                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
                
            
            
            
                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
            
            
            
                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif
            
            
                output.VertexColor = input.color;
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                    return output;
            }
            
            void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
            {
                result.vertex     = float4(attributes.positionOS, 1);
                result.tangent    = attributes.tangentOS;
                result.normal     = attributes.normalOS;
                result.texcoord1  = attributes.uv1;
                result.color      = attributes.color;
                result.vertex     = float4(vertexDescription.Position, 1);
                result.normal     = vertexDescription.Normal;
                result.tangent    = float4(vertexDescription.Tangent, 0);
                #if UNITY_ANY_INSTANCING_ENABLED
                #endif
            }
            
            void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
            {
                result.pos = varyings.positionCS;
                result.worldPos = varyings.positionWS;
                result.worldNormal = varyings.normalWS;
                // World Tangent isn't an available input on v2f_surf
            
                result._ShadowCoord = varyings.shadowCoord;
            
                #if UNITY_ANY_INSTANCING_ENABLED
                #endif
                #if UNITY_SHOULD_SAMPLE_SH
                #if !defined(LIGHTMAP_ON)
                result.sh = varyings.sh;
                #endif
                #endif
                #if defined(LIGHTMAP_ON)
                result.lmap.xy = varyings.lightmapUV;
                #endif
                #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                    result.fogCoord = varyings.fogFactorAndVertexLight.x;
                    COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
                #endif
            
                DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
            }
            
            void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
            {
                result.positionCS = surfVertex.pos;
                result.positionWS = surfVertex.worldPos;
                result.normalWS = surfVertex.worldNormal;
                // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
                // World Tangent isn't an available input on v2f_surf
                result.shadowCoord = surfVertex._ShadowCoord;
            
                #if UNITY_ANY_INSTANCING_ENABLED
                #endif
                #if UNITY_SHOULD_SAMPLE_SH
                #if !defined(LIGHTMAP_ON)
                result.sh = surfVertex.sh;
                #endif
                #endif
                #if defined(LIGHTMAP_ON)
                result.lightmapUV = surfVertex.lmap.xy;
                #endif
                #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                    result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                    COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
                #endif
            
                DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
            }
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
            
            ENDHLSL
        }

    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.Rendering.BuiltIn.ShaderGraph.BuiltInLitGUI" ""
    FallBack "Hidden/Shader Graph/FallbackError"
}