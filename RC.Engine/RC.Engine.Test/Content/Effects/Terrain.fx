//
// Terrain.fx : Render a terrain using a heightmap
//

float4x4 World;
float4x4 View;
float4x4 Projection;
float dx;
float dy;
float du;
float dv;

float3 lightDir = float3(1, 1, 1);	// Light direction in world space
float3 lightDir2 = float3(-1,-1,1); // Second light source

bool IsHeightMap = true;
texture HeightMap;
sampler2D HMSampler = sampler_state
{
    Texture = (HeightMap);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};


// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 Tex : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float  Diffuse : COLOR0;
    float  Height : COLOR1; 
    float2 Tex : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 pos = input.Position;
    float3 normal;
    
    // Compute height in object space
	float3 h = tex2Dlod (HMSampler, float4(input.Tex,0,0));
	pos.z = (h.x + h.y + h.z)/3.0f;
	
	output.Height = pos.z;

	// Compute normal in object space
	float3 h1 = tex2Dlod (HMSampler, float4(input.Tex+float2(du,0),0,0));
	float3 h2 = tex2Dlod (HMSampler, float4(input.Tex+float2(0,-dv),0,0));
		
	h.xyz = pos.xyz;
	
	h1.x = pos.x + dx; 
	h1.y = pos.y;
	h1.z = (h1.x + h1.y + h1.z)/3.0;
	
	h2.x = pos.x;
	h2.y = pos.y + dy;
	h2.z = (h2.x + h2.y + h2.z)/3.0f;

	normal = cross(h1-h, h2-h);
	normal = normalize(normal);

	// T & L
    float4 worldPosition = mul(pos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    lightDir = normalize(lightDir);
    lightDir2 = normalize(lightDir2);
    normal.xyz = mul(float4(normal,0), World).xyz;
    output.Diffuse = saturate (dot(normal, lightDir)) + saturate (dot(normal, lightDir2));
    output.Diffuse = saturate (output.Diffuse);

	output.Tex = input.Tex;

    return output;
}

bool multiTextureEnabled;

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
	
	color = input.Diffuse * float4(1,0,0,1); 
	
	if (multiTextureEnabled)
    {
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
	
		
		color = input.Diffuse * color;
		
	}
	
	color.w = 1;

    return color;
}

technique Terrain
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}