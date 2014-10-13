Texture2D Texture1 : register( t0 );
Texture2D Texture2 : register( t1 );
Texture2D Texture3 : register( t2 );
Texture2D Texture4 : register( t3 );
Texture2D Texture5 : register( t4 );
Texture2D Texture6 : register( t5 );
Texture2D Texture7 : register( t6 );
Texture2D Texture8 : register( t7 );

TextureCube CubeTexture1 : register( t8 );
TextureCube CubeTexture2 : register( t9 );
TextureCube CubeTexture3 : register( t10 );
TextureCube CubeTexture4 : register( t11 );
TextureCube CubeTexture5 : register( t12 );
TextureCube CubeTexture6 : register( t13 );
TextureCube CubeTexture7 : register( t14 );
TextureCube CubeTexture8 : register( t15 );

SamplerState TexSampler : register( s0 );

cbuffer MaterialVars : register (b0)
{
    float4 MaterialAmbient;
    float4 MaterialDiffuse;
    float4 MaterialSpecular;
    float4 MaterialEmissive;
    float MaterialSpecularPower;
};

cbuffer LightVars : register (b1)
{
    float4 AmbientLight;
    float4 LightColor[4];
    float4 LightAttenuation[4];
    float3 LightDirection[4];
    float LightSpecularIntensity[4];
    uint IsPointLight[4];
    uint ActiveLights;
}

cbuffer ObjectVars : register(b2)
{
    float4x4 LocalToWorld4x4;
    float4x4 LocalToProjected4x4;
    float4x4 WorldToLocal4x4;
    float4x4 WorldToView4x4;
    float4x4 UVTransform4x4;
    float3 EyePosition;
};

cbuffer MiscVars : register(b3)
{
    float ViewportWidth;
    float ViewportHeight;
    float Time;
};

struct A2V
{
    float4 pos : POSITION0;
    float3 normal : NORMAL0;
    float4 tangent : TANGENT0;
    float4 color : COLOR0;
    float2 uv : TEXCOORD0;
};

struct V2P
{
    float4 pos : SV_POSITION;
    float4 diffuse : COLOR;
    float2 uv : TEXCOORD0;
    float3 worldNorm : TEXCOORD1;
    float3 worldPos : TEXCOORD2;
    float3 toEye : TEXCOORD3;
    float4 tangent : TEXCOORD4;
    float3 normal : TEXCOORD5;
};

struct P2F
{
    float4 fragment : SV_Target;
};

//
// returns texture dimensions of the specified texture as a float2
//
float2 GetTextureDimensions(Texture2D tex)
{
    float x;
    float y;

    tex.GetDimensions(x,y);

    return float2(x, y);
}

//
// returns texel delta of the specified texture as a float2
//
float2 GetTexelDelta(Texture2D tex)
{
    float x;
    float y;

    tex.GetDimensions(x,y);

    return float2(1.0f/x, 1.0f/y);
}

//
// runs an edge detection filter on the input
//
float4 EdgeDetectionFilter(Texture2D tex, float2 uv)
{
    float dx;
    float dy;
    tex.GetDimensions(dx,dy);
    dx = 1.0f/dx;
    dy = 1.0f/dy;

    float4 color0 = -2.0f * tex.Sample(TexSampler, uv + float2(-dx, 0));
    float4 color1 = -tex.Sample(TexSampler, uv + float2(-dx, dy));
    float4 color2 = -tex.Sample(TexSampler, uv + float2(-dx, -dy));
    float4 color3 = 2.0f * tex.Sample(TexSampler, uv + float2(dx, 0));
    float4 color4 = tex.Sample(TexSampler, uv + float2(dx, dy));
    float4 color5 = tex.Sample(TexSampler, uv + float2(dx, -dy));
    float4 sumX = color0 + color1 + color2 + color3 + color4 + color5;

    float4 color6 = -2.0f * tex.Sample(TexSampler, uv + float2(0, -dy));
    float4 color7 = -tex.Sample(TexSampler, uv + float2(dx, -dy));
    float4 color8 = color2;
    float4 color9 = 2.0f * tex.Sample(TexSampler, uv + float2(0, dy));
    float4 color10 = color4;
    float4 color11 = tex.Sample(TexSampler, uv + float2(-dx, dy));
    float4 sumY= color6 + color7 + color8 + color9 + color10 + color11;

    return sqrt(sumX * sumX + sumY * sumY);
}


//
// runs a Gaussian blur filter on the input
//
float4 Blur(Texture2D tex, float2 uv)
{
    float dx;
    float dy;
    tex.GetDimensions(dx,dy);
    dx = 1.0f/dx;
    dy = 1.0f/dy;

	float4 colorSum = float4(0,0,0,0);

	colorSum += tex.Sample(TexSampler, uv + float2(-2 * dx, -2 * dy)) * 2;
	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx, -2 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2(      0, -2 * dy)) * 5;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx, -2 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2( 2 * dx, -2 * dy)) * 2;

	colorSum += tex.Sample(TexSampler, uv + float2(-2 * dx, -1 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx, -1 * dy)) * 9;
	colorSum += tex.Sample(TexSampler, uv + float2(      0, -1 * dy)) * 12;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx, -1 * dy)) * 9;
	colorSum += tex.Sample(TexSampler, uv + float2( 2 * dx, -1 * dy)) * 4;

	colorSum += tex.Sample(TexSampler, uv + float2(-2 * dx,       0)) * 5;
	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx,       0)) * 12;
	colorSum += tex.Sample(TexSampler, uv + float2(      0,       0)) * 15;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx,       0)) * 12;
	colorSum += tex.Sample(TexSampler, uv + float2( 2 * dx,       0)) * 5;

	colorSum += tex.Sample(TexSampler, uv + float2(-2 * dx,  1 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx,  1 * dy)) * 9;
	colorSum += tex.Sample(TexSampler, uv + float2(      0,  1 * dy)) * 12;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx,  1 * dy)) * 9;
	colorSum += tex.Sample(TexSampler, uv + float2( 2 * dx,  1 * dy)) * 4;

	colorSum += tex.Sample(TexSampler, uv + float2(-2 * dx,  2 * dy)) * 2;
	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx,  2 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2(      0,  2 * dy)) * 5;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx,  2 * dy)) * 4;
	colorSum += tex.Sample(TexSampler, uv + float2( 2 * dx,  2 * dy)) * 2;

	return colorSum/159;
}


//
// Sharpen filter
//
float4 Sharpen(Texture2D tex, float2 uv)
{
    float dx;
    float dy;
    tex.GetDimensions(dx,dy);
    dx = 1.0f/dx;
    dy = 1.0f/dy;

	float4 colorSum = float4(0,0,0,0);

	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx, -1 * dy)) * -1;
	colorSum += tex.Sample(TexSampler, uv + float2(      0, -1 * dy)) * -1;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx, -1 * dy)) * -1;

	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx,       0)) * -1;
	colorSum += tex.Sample(TexSampler, uv + float2(      0,       0)) * 17;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx,       0)) * -1;

	colorSum += tex.Sample(TexSampler, uv + float2(-1 * dx,  1 * dy)) * -1;
	colorSum += tex.Sample(TexSampler, uv + float2(      0,  1 * dy)) * -1;
	colorSum += tex.Sample(TexSampler, uv + float2( 1 * dx,  1 * dy)) * -1;

	return colorSum/9;
}
//
// desaturate
//
float3 Desaturate(
    float3 color, 
    float3 luminance, 
    float percent
    )
{
    float3 desatColor = dot(color, luminance);
    return lerp(color, desatColor, percent);
}

//
// fresnel falloff
//
float Fresnel(
    float3 surfaceNormal,
    float3 toEye,
    float exp
    )
{
    float x = 1 - saturate(dot(surfaceNormal, toEye));
    return pow(x, exp);
}

//
// panning offset
//
float2 PanningOffset(
    float2 sourceUV,
    float time,
    float speedX,
    float speedY
    )
{
    float2 uvOffset = float2(time, time) * float2(speedX, speedY);

    return sourceUV + uvOffset;
}

//
// parallax offset
//
float2 ParallaxOffset(
    float2 sourceUV,
    float height,
    float depthScale,
    float depthPlane,
    float3 tangentCameraVector
    )
{
    float bias = -(depthScale * depthPlane);
    float heightAdj = (depthScale * height) + bias;

    return sourceUV + (tangentCameraVector.xy * heightAdj);
}

//
// rotate offset
//
float2 RotateOffset(
    float2 sourceUV,
    float time,
    float centerX,
    float centerY,
    float speed
    )
{
    float2 ray = sourceUV - float2(centerX, centerY);
    float theta = time * speed;

    float cosTheta = cos(theta);
    float sinTheta = sin(theta);

    float2x2 rotMatrix = float2x2(float2(cosTheta, -sinTheta), float2(sinTheta, cosTheta));

    return mul(rotMatrix, ray) + float2(centerX, centerY);
}

//
// lambert lighting function
//
float3 LambertLighting(
    float3 lightNormal,
    float3 surfaceNormal,
    float3 materialAmbient,
    float3 lightAmbient,
    float3 lightColor,
    float3 pixelColor
    )
{
    // compute amount of contribution per light
    float diffuseAmount = saturate(dot(lightNormal, surfaceNormal));
    float3 diffuse = diffuseAmount * lightColor * pixelColor;

    // combine ambient with diffuse
    return saturate((materialAmbient * lightAmbient) + diffuse);
}

//
// specular contribution function
//
float3 SpecularContribution(
    float3 toEye,
    float3 lightNormal,
    float3 surfaceNormal,
    float3 materialSpecularColor,
    float materialSpecularPower,
    float lightSpecularIntensity,
    float3 lightColor
    )
{
    // compute specular contribution
    float3 vHalf = normalize(lightNormal + toEye);
    float specularAmount = saturate(dot(surfaceNormal, vHalf));
    specularAmount = pow(specularAmount, max(materialSpecularPower,0.0001f)) * lightSpecularIntensity;
    float3 specular = materialSpecularColor * lightColor * specularAmount;
    
    return specular;
}

//
// combines a float3 RGB value with an alpha value into a float4
//
float4 CombineRGBWithAlpha(float3 rgb, float a) 
{ 
    return float4(rgb.r, rgb.g, rgb.b, a); 
}

P2F main(V2P pixel)
{
    P2F result;

    // we need to normalize incoming vectors
    float3 surfaceNormal = normalize(pixel.normal);
    float3 surfaceTangent = normalize(pixel.tangent.xyz);
    float3 worldNormal = normalize(pixel.worldNorm);
    float3 toEyeVector = normalize(pixel.toEye);

    // construct tangent matrix
    float3x3 localToTangent = transpose(float3x3(surfaceTangent, cross(surfaceNormal, surfaceTangent) * pixel.tangent.w, surfaceNormal));
    float3x3 worldToTangent = mul((float3x3)WorldToLocal4x4, localToTangent);

    // transform some vectors into tangent space
    float3 tangentLightDir = normalize(mul(LightDirection[0], worldToTangent));
    float3 tangentToEyeVec = normalize(mul(toEyeVector, worldToTangent));

    // BEGIN GENERATED CODE
    float3 local0 = SpecularContribution(tangentToEyeVec, tangentLightDir, float3(0.000000f,0.000000f,1.000000f), MaterialSpecular.rgb, MaterialSpecularPower, LightSpecularIntensity[0], LightColor[0].rgb);
    float3 local3 = LambertLighting(tangentLightDir, float3(0.000000f,0.000000f,1.000000f), MaterialAmbient.rgb, AmbientLight.rgb, LightColor[0].rgb, Texture1.Sample(TexSampler, pixel.uv).rgb);
    float3 local4 = local0 + local3;
    float4 local5 = Blur(Texture1, local4.rg);
    result.fragment = CombineRGBWithAlpha(local5.rgb, Texture1.Sample(TexSampler, pixel.uv).a);
    // END GENERATED CODE

    if (result.fragment.a == 0.0f) discard;

    return result;
}

