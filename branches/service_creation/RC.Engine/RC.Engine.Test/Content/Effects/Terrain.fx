//
// Terrain.fx : Render a terrain using a heightmap
//

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 Tex : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float  Height : COLOR1; 
    float2 Tex : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    float4 pos = input.Position;
    float4 worldPosition = mul(pos, World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Height = pos.z;
    output.Position = mul(viewPosition, Projection);
	output.Tex = input.Tex;
	
    return output;
}

texture multiTextureBottom;
sampler SamplerBottom = sampler_state
{
    Texture = (multiTextureBottom);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

texture multiTextureMiddle;
sampler SamplerMiddle = sampler_state
{
    Texture = (multiTextureMiddle);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};

texture multiTextureTop;
sampler SamplerTop = sampler_state
{
    Texture = (multiTextureTop);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};


float bottomTextureUnblendedMax;
float middleTextureUnblendedMin;
float middleTextureUnblendedMax;
float topTextureUnblendedMin;

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color, color0, color1, color2;
	
	color = float4(1,0,0,1); 
	color0 = tex2D (SamplerBottom, input.Tex);
	color1 = tex2D (SamplerMiddle, input.Tex);
	color2 = tex2D (SamplerTop, input.Tex);
	
	if (input.Height >= topTextureUnblendedMin)
		color = color2;
	else if (input.Height >= middleTextureUnblendedMax && input.Height < topTextureUnblendedMin)
		color = color1 + ((input.Height-middleTextureUnblendedMax)/(topTextureUnblendedMin-middleTextureUnblendedMax))*(color2-color1);
	else if (input.Height < middleTextureUnblendedMax && input.Height >= middleTextureUnblendedMin)
		color = color1;
	else if (input.Height < middleTextureUnblendedMin && input.Height >= bottomTextureUnblendedMax)
		color = color0 + ((input.Height-bottomTextureUnblendedMax)/(middleTextureUnblendedMin-bottomTextureUnblendedMax))*(color1-color0);
	else if (input.Height < bottomTextureUnblendedMax)
		color = color0;		
		
	color.w = 1;

    return color;
}

technique Terrain
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}