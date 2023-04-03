#ifndef HELLSING_CUSTOM_DEFINE
#define HELLSING_CUSTOM_DEFINE

CBUFFER_START(UnityPerMaterial)
    half4 _MainTex_ST;
    half4 _NormalMap_ST;
    half4 _MainColor;
    
    half _NormalScale;
    half _CutOff;
CBUFFER_END

TEXTURE2D(_MainTex);
TEXTURE2D(_NormalMap);
TEXTURE2D(_OccusionMap);
TEXTURE2D(_MetallicMap);

SAMPLER(sampler_MainTex);
SAMPLER(sampler_NormalMap);
SAMPLER(sampler_OccusionMap);
SAMPLER(sampler_MetallicMap);

struct VertexInput{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
};

struct VertexOutput{
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float4 TtoW0 : TEXCOORD1;
    float4 TtoW1 : TEXCOORD2;
    float4 TtoW2 : TEXCOORD3;
    float2 shadowCoords : TEXCOORD4;
};

#endif