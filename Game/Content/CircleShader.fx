#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct PixelInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float radius;
float2 textureSize; // Dimensions of the texture in pixels
float pixelSize;    // Size of each block in pixels

float4 CircleShader(PixelInput input) : COLOR
{
    // Quantize texture coordinates to the nearest block
    float2 blockCoords = floor(input.TexCoord * textureSize / pixelSize) * pixelSize / textureSize;

    // Adjust blockCoords to the center of the block
    float2 blockCenter = blockCoords + (0.5 * pixelSize / textureSize);

    // Calculate the circle's center in normalized texture coordinates
    float2 center = float2(0.5, 0.5);

    // Calculate the distance from the block's center to the circle's center
    float distance = length(blockCenter - center);

    // Check if the block is inside the circle
    if (distance > radius)
    {
        return float4(0, 0, 0, 0); // Transparent outside the circle
    }

    // Use the input color (or modify as needed)
    return input.Color;
}

technique CircleEffect
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL CircleShader();
    }
};
