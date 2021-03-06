#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct minimalVertexInput
{
    uint vertexID     : SV_VertexID;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct minimalVertexOutput
{
    half4 position  : POSITION;
    half2 texcoord : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

#if defined(UNITY_SINGLE_PASS_STEREO)
float4 UnityStereoAdjustedTexelSize(float4 texelSize)
{
    texelSize.x = texelSize.x * 2.0; // texelSize.x = 1/w. For a double-wide texture, the true resolution is given by 2/w.
    texelSize.z = texelSize.z * 0.5; // texelSize.z = w. For a double-wide texture, the true size of the eye texture is given by w/2.
    return texelSize;
}
#else
float4 UnityStereoAdjustedTexelSize(float4 texelSize)
{
    return texelSize;
}
#endif

void GetProceduralQuad(in uint vertexID, out float4 positionCS, out float2 uv)
{
    positionCS = GetQuadVertexPosition(vertexID);
    positionCS.xy = positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f);
    uv = GetQuadTexCoord(vertexID);// * _ScaleBias.xy + _ScaleBias.zw;
}

SAMPLER(sampler_LinearClamp);
#define SAMPLE_SCREEN_TEX(tex, uv) SAMPLE_TEXTURE2D_X(tex, sampler_LinearClamp, UnityStereoTransformScreenSpaceTex(uv))
