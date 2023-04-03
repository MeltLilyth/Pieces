#ifndef HELLSING_UNIFORM_DEFINED
#define HELLSING_UNIFORM_DEFINED

CBUFFER_START(UnityPerMaterial)
    half4 _MainTex_ST;
    half4 _MainColor;
    half4 _NormalMap_ST;
    half _NormalScale;
    half _Cutoff;
CBUFFER_END

TEXTURE2D(_MainTex);
TEXTURE2D(_NormalMap);
TEXTURE2D(_MetallicMap);
TEXTURE2D(_OccusionMap);

SAMPLER(sampler_MainTex);
SAMPLER(sampler_NormalMap);
SAMPLER(sampler_MetallicMap);
SAMPLER(sampler_OccusionMap);

struct VertexInput{
    float4 posOs : POSITION;
    float4 normal : NORMAL;
    float4 tangent : TANGENT;
    float4 uv : TEXCOORD0;
    float2 lightMapUV : TEXCOORD1;
};

struct VertexOutput{
    float4 posCs : SV_POSITION;
    float4 uv : TEXCOORD0;
    float4 TtoW0 : TEXCOORD1;
    float4 TtoW1 : TEXCOORD2;
    float4 TtoW2 : TEXCOORD3;
    float4 shadowCoords : TEXCOORD4;
    DECLARE_LIGHTMAP_OR_SH(lightMapUV, vertexSH, 5);   
    /*

    //关于unity场景的光照信息
    GI -- Global illumibation, 全局光照, 可以在unity中的Lighting面板中进行设置

    Precomputed Realtime GI: 预计算实时光照, 针对实时静态模型之间的光照信息
    Baked GI: 烘焙全局光照, 能够得到更精确的模型之间的反射光信息, 但是无法在游戏中实时的变动光源信息
    Gerneral GI: 常规照明设置

    DECLARE_LIGHTMAP_OR_SH: lighting.hlsl中的宏定义,等价于 float2 lmName : TEXCOORD4 
    如果LIGHTMAP_ON被定义的时候会采样lightMap的方式计算GI[#define OUTPUT_LIGHTMAP_UV(lightmapUV, lightmapScaleOffset, OUT) OUT.xy = lightmapUV.xy * lightmapScaleOffset.xy + lightmapScaleOffset.zw],
    否则就会采用球谐的方式去计算GI[具体实现见Lighting.hlsl中的SampleSHVertex(half3 normalWS), #define OUTPUT_SH(normalWS, OUT) OUT.xyz = SampleSHVertex(normalWS)]
    
    */
};

struct VertexInput_Shadow{
    float4 posOs : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct VertexOutput_Shadow{
    float4 posCs : SV_POSITION;
    float2 uv : TEXCOORD0;
};

#endif