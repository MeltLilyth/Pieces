#ifndef HELLSING_FUNCTION_INCLUDE
#define HELLSING_FUNCTION_INCLUDE

struct HellsingInputData{
    half metallic;
    half smoothness;
    half roughness;
    half ao;

    half3 posWs;
    half3 normalWs;
};

struct BRDFSectionInfo{
    float3 BRDFSection;
    float3 Fresnel;
};

HellsingInputData InitliazeInputData(VertexOutput i){
    HellsingInputData o;
    //杂项
    float4 metallicMap = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, i.uv.xy);
    float4 occusionMap = SAMPLE_TEXTURE2D(_OccusionMap, sampler_OccusionMap, i.uv.xy);
    o.metallic = metallicMap.r;
    o.smoothness = metallicMap.a;
    o.ao = occusionMap.g;
    half tempRoughness = 1.0 - o.smoothness;
    o.roughness = pow(tempRoughness, 2);

    o.posWs = half3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
    o.normalWs = i.TtoW2.xyz;

    return o;
}

//直接光照
float D_Function(float NdotH, float roughness){
    float a2 = roughness * roughness;
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = NdotH2 * (a2 - 1) + 1;
    denom = denom * denom * PI;
    return nom / denom;
}

float G_Session(float dot, float k){
    float nom = dot;
    float denom = lerp(dot, 1, k);
    return nom / denom;
}

float G_Function(float NdotL, float NdotV, float roughness){
    /*
        直接光系数: k = pow((1+roughness), 2)
        间接光系数: k = pow(roughness, 2)
    */
    float k = pow((1 + roughness), 2) / 8;  
    float Gnl = G_Session(max(0, NdotL), k);
    float Gnv = G_Session(max(0, NdotV), k);
    
    return Gnl * Gnv;
}

/*核心项 -- F 菲涅尔系数 --> 基础公式: F = lerp((1-(NdotV))^5, 1, F0)
    但是PBR的实际计算中,因为法线分布函数的影响,部分法线数据会被均化,所以计算中需要经过法线分布函数计算剔除后的法线数据
    于是菲涅尔函数修改如下:
        F = F0 + (1 - F0)((1 - HdotV) ^ 5)
    后续经过unity进行优化,最终使用的计算公式为:
        F = F0 + (1 - F0)((1 - LdotH) ^ 5) ----> 由原来的dot(halfDir, viewDir)优化为dot(lightDir, halfDir)
        F0 = lerp(0.04, albedo, metallic) --> metallic[金属度,可以存储在贴图中或者直接暴露于层级面板]    

    UE4中对于菲涅尔项的计算用了如下公式: F = F0 + (1-F0) * 2 ^ (-5.55473 * HdotV - 6.98316) * HdotV
    原理在于数学计算中x ^ y = e ^ (y*lnx) ---> e为对数函数中的那个特殊的底数

    菲涅尔函数与光照方向无关
*/
float3 F_Function(float HdotL, float3 F0){
    float Fre = clamp(exp2((-5.55473 * HdotL - 6.98316) * HdotL), 0, 1);
    return lerp(F0, 1, Fre);
}

//间接光菲涅尔系数 -- 这个东西待会查一下
float3 Indir_F_Function(float NdotV, float3 F0, float roughness){
    float Fre = exp2((-5.55473 * NdotV - 6.98316) * NdotV);
    return F0 + Fre * saturate(1 - roughness - F0);
}

BRDFSectionInfo InitliazeBRDFSection(float3 F0, float NdotH, float NdotL, float NdotV, float HdotL, float roughness){
    float D = D_Function(NdotH, roughness);
    float G = G_Function(NdotL, NdotV, roughness);
    float3 F = F_Function(HdotL, F0);

    float3 BRDFSection = (D * F * G) / (4 * NdotL * NdotV + 0.0001);
    
    BRDFSectionInfo sectionInfo;
    sectionInfo.BRDFSection = BRDFSection;
    sectionInfo.Fresnel = F;

    return sectionInfo;
}

//间接光照
/*
对于场景中的静态物体, 可以通过预先烘焙好场景中的光照贴图进行采样;对于场景中的动态物体,unity中可以采用光照探针提供间接光(Light Probe)
由于环境光的精度可以不需要那么高,可以将环境光图缩小[别想着从资源层面上缩小到极值,效果展现上还不如不缩]
对于低频的光照处理 -- unity给出的解决方案就是球谐函数,将光照信息进行编码,将编码后的数据保存在一个大小为27的float数组中,在运行时候解压重组为完整的光照信息

傅里叶展开: 一个函数可以由无数个正弦函数和余弦函数组合而成 (??? what the fuck is this? -- 算了不管了,反正大致意思是可以用正余弦函数将某个我们熟知的函数进行重组[用到了积分之类的运算])

在一个单位球面上一点P, 已知存在俯仰角Φ 和 翻转角θ,则当前点坐标为(sinθ*sinΦ, cosθ*sinΦ, cosΦ),假设当前点的颜色是f关于Φ和θ的函数,则有
    c = f<θ, Φ>
当前函数可以通过由无数个球面正余弦函数组成 => f<θ, Φ> = sh0 + sh1 + sh2 + ......
组成的球面正余弦函数越多,原始单位球面上赋予的贴图就越清晰 --- 由于是低频光照贴图,unity中球协函数的组成一般由9个球面正余弦函数组成[unity在底层中提供对应的接口 ShadeSH9() [build-in] 
和 sampleSH9() [universal]]

球协光照的内置参数 -- 
    half4 unity_SHAr,unity_SHAg,unity_SHAb,unity_SHBr,unity_SHBg,unity_SHBb,unity_SHC
先将环境贴图cubemap积分成模糊的全局光照贴图, 再将全局光照贴图投影到球谐光照的基函数上存储[三阶的伴随勒让德多项式], 参数如下所示:

                 m= -2                           m= -1                                m= 0                                  m= 1                             m= 2

l = 0                                                                            (1/2)*sqrt(1/PI)

l = 1                                  (1/2)*sqrt(3/PI)*(y/r)                 (1/2)*sqrt(3/PI)*(z/r)                    (1/2)*sqrt(3/PI)*(x/r)

l = 2  (1/2)*sqrt(15/PI)*(y*x/r^2)     (1/2)*sqrt(15/PI)*(y*z/r^2)      (1/4)*sqrt(5/PI)*(2*(z^2)-x^2-y^2/r^2)       (1/2)*sqrt(15/PI)*(x*z/r^2)     (1/2)*sqrt(15/PI)*(x^2-y^2/r^2)

其中r = x^2 + y^2 + z^2
unity舍弃了（m=0 && l=0） 和  (m = 0 && l = 2)的情况, 通过用unity_SHA表示l=1时的参数,unity_SHB表示l=2&&(m=-2||m=-1||m=1)的三个参数,单独用unity_SHC 表示(l=2&&m=2)的情况

unity urp中,可以直接调用Lighting.hlsl中的SampSH(half3 normalWS)去计算球谐光照
*/

//反射探针的图像输出
real3 IndirSpecCube(float3 normalWs, float3 viewDir, float roughness, float AO){
    float3 reflectDirWs = reflect(-viewDir, normalWs);
    //unity内部不是线性, 调整下拟合曲线求近似值 --- ? 这啥啊
    roughness = roughness * (1.7 - 0.7 * roughness);
    float midLevel = roughness * 6;
    float4 specColor = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectDirWs, midLevel);
    
    #if !defined(UNITY_USE_NATIVE_HDR)
        return DecodeHDREnvironment(specColor, unity_SpecCube0_HDR) * AO;
    #else
        return specColor.rgb * AO;
    #endif
}

/*
使用一张现有的LUT图像进行采样, 获得一个间接高光的影响因子[图片采样的uv值为float2(NdotV, roughness)]
但是采样过程比较消耗性能,unity底层使用曲线拟合去得到结果
*/
real3 IndirSpecFactor(float roughness, float smoothness, float3 BRDFspe, float3 F0, float NdotV){
    #ifdef UNITY_COLORSPACE_GAMMA
        float SurReduction = 1 - 0.28 * roughness * roughness;
    #else
        float SurReduction = 1/(roughness * roughness + 1);
    #endif

    #if defined(SHADER_API_GLES)
        float Reflectivity = BRDFspe.x;
    #else
        float Reflectivity = max(max(BRDFspe.x, BRDFspe.y), BRDFspe.z);
    #endif 

    half GrazingTSection = saturate(Reflectivity + smoothness);
    float fre = Pow4(1-NdotV);
    return lerp(F0, GrazingTSection, fre) * SurReduction;
}

/*
    直接光高光反射 + 漫反射 [PBR基础光照模型]
*/
float4 PBR_DirectRender(float3 albedo, BRDFSectionInfo sectionInfo, Light light, float NdotL, float metallic){

    float3 directSpecColor = sectionInfo.BRDFSection * light.color * NdotL * PI;

    //直接光漫反射
    float3 ks = sectionInfo.Fresnel;
    float3 kd = (1-ks)*(1-metallic);
    float3 directDiffColor = kd * albedo * light.color * NdotL;
    
    //直接光 -- 面光部分和背光部分做一个阴影过度
    half3 radiance = light.color * (NdotL * (light.shadowAttenuation));

    float3 directColor = (directDiffColor + directSpecColor) * (light.shadowAttenuation * light.distanceAttenuation * NdotL) ;
    return float4(directColor , 1.0);
}

///PBR 间接光照渲染整合 漫反射 + 高光反射
float4 PBR_IndirectRender(float3 F0, float3 albedo, float3 normalWs, float3 BRDFspe ,float3 viewDir,float NdotV, float roughness, float smoothness, float metallic, float AO){
    //环境光间接光照漫反射
    half3 SHColor = SampleSH(normalWs) * AO;
    float3 indirks = Indir_F_Function(NdotV, F0, roughness);
    float3 indirkd = (1-indirks)*(1-metallic);
    float3 indirDiffColor = indirkd * SHColor * albedo;
    //环境光间接光照高光反射
    float3 indirSpecCubeColor = IndirSpecCube(normalWs, viewDir, roughness, AO);
    float3 indirSpecFactor = IndirSpecFactor(roughness, smoothness, BRDFspe, F0, NdotV);
    float3 indirSpecColor = indirSpecCubeColor * indirSpecFactor;
    
    return float4((indirDiffColor + indirSpecColor), 1.0);
}

float4 HellsingPBRender(float3 albedo, float3 F0, Light light, float3 normalDir, HellsingInputData inputData){
    half3 cameraPos = GetCameraPositionWS();
    half3 viewDir = normalize(cameraPos - inputData.posWs);

    half3 lightDir = normalize(light.direction);
    half3 halfDir = normalize(lightDir + viewDir);

    half NdotL = max(0, dot(normalDir, lightDir));
    half NdotV = max(0, dot(normalDir, viewDir));
    half NdotH = max(0, dot(normalDir, halfDir));
    half HdotL = max(0, dot(halfDir, lightDir));

    BRDFSectionInfo BRDFData = InitliazeBRDFSection(F0, NdotH, NdotL, NdotV, HdotL, inputData.roughness); 

    float4 directColor = PBR_DirectRender(albedo, BRDFData, light, NdotL, inputData.metallic);
    float4 indirectColor = PBR_IndirectRender(F0, albedo, inputData.normalWs, BRDFData.BRDFSection, viewDir, NdotV, inputData.roughness, inputData.smoothness, inputData.metallic, inputData.ao);
    
    float3 color = directColor.rgb + indirectColor;

    return float4(color, 1.0);
}

//HLSLPROGRAM

VertexOutput vert(VertexInput i){
    VertexOutput o;
    o.posCs = TransformObjectToHClip(i.posOs.xyz);
    o.uv.xy = TRANSFORM_TEX(i.uv.xy, _MainTex);
    o.uv.zw = TRANSFORM_TEX(i.uv.zw, _NormalMap);
    
    half3 posWs = mul(unity_ObjectToWorld, i.posOs).xyz;
    half3 normal = TransformObjectToWorldNormal(i.normal, true);
    half3 tangent = TransformObjectToWorldDir(i.tangent.xyz, true);
    half3 biNormal = cross(normal, tangent) * i.tangent.w;

    o.TtoW0 = float4(tangent, posWs.x);
    o.TtoW1 = float4(biNormal, posWs.y);
    o.TtoW2 = float4(normal, posWs.z);
    o.shadowCoords = TransformWorldToShadowCoord(posWs);

    OUTPUT_LIGHTMAP_UV(i.lightMapUV, unity_LightmapST, o.lightMapUV);
    OUTPUT_SH(normal, o.vertexSH);

    return o;
}

float4 frag(VertexOutput i) : SV_TARGET{
    HellsingInputData inputData = InitliazeInputData(i);

    Light mainLight = GetMainLight(i.shadowCoords);

    float3 normalDir = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv.zw), _NormalScale);
    normalDir.z = sqrt(1.0 - dot(normalDir.xy, normalDir.xy));
    float3x3 matrixT = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
    
    //transpose() 获取传入矩阵的转至矩阵
    matrixT = transpose(matrixT);
    normalDir = NormalizeNormalPerPixel(mul(matrixT, normalDir));

    float3 albedo = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy) * _MainColor).rgb;
    float3 F0 = lerp(float3(0.04, 0.04, 0.04), albedo, inputData.metallic);

    float4 color = HellsingPBRender(albedo, F0, mainLight, normalDir, inputData);

    //其他光源渲染信息
    #ifdef _ADDITIONAL_LIGHTS
        uint addLightCounts = GetAdditionalLightsCount();
        Light addLight;
        
        for(int index = 0u; index < addLightCounts; index ++){
            addLight = GetAdditionalLight(index, inputData.posWs);
            addLight.shadowAttenuation = AdditionalLightRealtimeShadow(index, inputData.posWs);
            //color.rgb += (max(0, dot(normalDir, addLight.direction)) * 0.5 + 0.5) * albedo * addLight.color * (addLight.distanceAttenuation * addLight.shadowAttenuation);
            color.rgb += HellsingPBRender(albedo, F0, addLight, normalDir, inputData);
        }
    #endif

    return color;
}

#endif