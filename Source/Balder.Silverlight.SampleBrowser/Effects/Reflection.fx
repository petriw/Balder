sampler2D input : register(s0);

float4 reflectionHeight : register(c1);
float4 ddxUvDdyUv : register(c6);

float4 Reflect(float2 uv : TEXCOORD) : COLOR
{
  float edge = 0.5;
  if (uv.y > edge)
  {
    uv.y = edge - (uv.y - edge);
    return tex2D(input, uv) * uv.y;
  }
  return tex2D(input, uv);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
  return Reflect(uv);
}