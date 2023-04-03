#ifndef HELLSING_SHADOW_FUNCTION_INCLUDE
#define HELLSING_SHADOW_FUNCTION_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "urp_hellsing_define.hlsl"

float3 _LightDirection;
// float4 _ShadowBias;             //x : depth bias,  y : normal bias
// half4 _MainLightShadowParams;   //x : shadow strength y: 1.0 if soft shadow 0.0 otherwise


// float3 ApplyShadowBias(float3 posWs, float3 normalWs, float3 lightDirection){
//     half invNdotL = 1 - max(0, dot(normalWs, lightDirection));
//     half scale = inNdotL * _ShadowBias.y;
//     posWs = lightDirection * _ShadowBias.xxx + posWs;
//     posWs = normalWs * scale.xxx + posWs;
//     return posWs;
// }

VertexOutput_Shadow vert(VertexInput_Shadow i){
    VertexOutput_Shadow o;
    half3 posWs = mul(unity_ObjectToWorld, i.posOs).xyz;
    half3 normalWs = TransformObjectToWorldNormal(i.normal);
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(posWs, normalWs, _LightDirection));

    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif

    o.posCs = positionCS;
    o.uv = TRANSFORM_TEX(i.uv, _MainTex);

    return o;
}

half4 frag(VertexOutput_Shadow i) : SV_TARGET{
    float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
    Alpha(albedo.a, _MainColor, _Cutoff);
    return 0;
}

#endif