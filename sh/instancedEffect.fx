float4x4 View;
float4x4 Projection;

bool TextureEnabled=true;
Texture Texture;
sampler TextureSampler = sampler_state {
	texture = <Texture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter= LINEAR;
	AddressU = wrap;
	AddressV = wrap;
};
 
struct VertexShaderInput{
    // PER VERTEX DATA
    float4 Position : POSITION0;
    float2 TextureCoords : TEXCOORD0;
 
    // PER INSTANCE DATA
    float4x4 World : TEXCOORD3;
    float4 Colour : COLOR0;
};
 
struct VertexShaderOutput{
    float4 Position : POSITION0;
    float4 Colour : COLOR0;
    float2 TextureCoords: TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input){
    VertexShaderOutput output;
 
    // Use per instance World matrix to get World Pos
    float4 worldPosition = mul(input.Position, transpose(input.World));
 
    // Transform with camera view and projection to get screen pos
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    // Send texture coords and per instance colour to pixel shader
    output.Colour =  input.Colour;
    output.TextureCoords = input.TextureCoords;
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0{
    // Get texture pixel
    float4 colour = float4(1,1,1,1);
	if(TextureEnabled)colour=tex2D(TextureSampler, input.TextureCoords);
 
    // Multiply by per instance colour value
    colour *= input.Colour;
    return colour;
}
 
technique Technique1{
    pass Pass1{
        // TODO: set renderstates here.
 
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
