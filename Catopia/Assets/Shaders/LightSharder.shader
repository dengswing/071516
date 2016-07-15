Shader "Faye/OutLightting"
{
	Properties
	{
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0, 0, 0, 1)
		_AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)

		_Size("Size", Range(.001, 0.05)) = .001

		_Falloff("Falloff", Float) = 5
		_FalloffPlanet("Falloff Planet", Float) = 5
		_Transparency("Transparency", Float) = 15
		_TransparencyPlanet("Transparency Planet", Float) = 1


		_Color("Main Color", Color) = (0.9,0.9,0.9,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		//_Outline ("Outline width",float) = 0.1
		_Outline("Outline width", Range(.000, 0.05)) = .005


		//_MainTex("Base (RGB)", 2D) = "white" { }
	//	_Cutoff ("Alphatest Cutoff", float) = 0.05
	_CameraSize("Camera Size", float) = 5

		//	_RimPower("Rim Power", Range(0.5,8.0)) = 3.0

		//_AP ("ap", float) = 1

		_MaskColor5("Mask Color Slot 1 (R Channel )", Color) = (1,1,1,1)
		_MaskColor6("Mask Color Slot 2 (G Channel )", Color) = (1,1,1,1)
		_MaskColor7("Mask Color Slot 3 (B Channel )", Color) = (1,1,1,1)
		_MaskTex("Mask Image", 2D) = "white" { }
	_MaskStrength("Mask Strength", Float) = 1

	}

		SubShader
	{
		Pass
	{
		Name "PlanetBase"
		Tags{ "LightMode" = "Always" }
		Cull Back

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float4 _Color;
	uniform float4 _AtmoColor;
	uniform float _FalloffPlanet;
	uniform float _TransparencyPlanet;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD0;
		float3 worldvertpos : TEXCOORD1;
		float2 texcoord : TEXCOORD2;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.normal = v.normal;
		o.worldvertpos = mul(_Object2World, v.vertex).xyz;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		i.normal = normalize(i.normal);
	float3 viewdir = normalize(_WorldSpaceCameraPos - i.worldvertpos);

	float4 atmo = _AtmoColor;
	atmo.a = pow(1.0 - saturate(dot(viewdir, i.normal)), _FalloffPlanet);
	atmo.a *= _TransparencyPlanet*_Color;

	float4 color = tex2D(_MainTex, i.texcoord)*_Color;
	color.rgb = lerp(color.rgb, atmo.rgb, atmo.a);

	return color*dot(normalize(i.worldvertpos - _WorldSpaceLightPos0), i.normal);
	}
		ENDCG
	}

		Pass
	{
		Name "AtmosphereBase"
		Tags{ "LightMode" = "Always" }
		Cull Front
		Blend SrcAlpha One

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

		uniform float4 _Color;
	uniform float4 _AtmoColor;
	uniform float _Size;
	uniform float _Falloff;
	uniform float _Transparency;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD0;
		float3 worldvertpos : TEXCOORD1;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		v.vertex.xyz += v.normal*_Size;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.normal = v.normal;
		o.worldvertpos = mul(_Object2World, v.vertex);

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		i.normal = normalize(i.normal);
	float3 viewdir = normalize(i.worldvertpos - _WorldSpaceCameraPos);

	float4 color = _AtmoColor;
	color.a = pow(saturate(dot(viewdir, i.normal)), _Falloff);
	color.a *= _Transparency*_Color*dot(normalize(i.worldvertpos - _WorldSpaceLightPos0), i.normal);
	return color;
	}
		ENDCG
	}




			Pass
	{
		Name "OUTLINE"
		Tags{ "Queue" = "Geometry" }
		Cull front

		ZWrite on
		ZTest Less


		//ColorMask RGBA  
		Blend SrcAlpha OneMinusSrcAlpha
		Offset 30,30


		CGPROGRAM
#include "UnityCG.cginc"  
	struct appdata
	{
		half4 vertex : POSITION;
		half3 normal : NORMAL;
	};
	struct v2f
	{
		half4 pos : POSITION;
		half4 color : COLOR;
	};

	//half _RimPower;
	uniform float _S;
	uniform float _AP;
	uniform float _Outline;
	uniform float4 _OutlineColor;
	//uniform float _CameraSize = 60.0f;
	v2f vert(appdata v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		fixed3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		fixed2 offset = TransformViewToProjection(norm.xy);
		//		fixed viewScaler = (o.pos.z + 1) *0.5;  
		//		o.pos.xy += offset * viewScaler * _Outline;  
		o.pos.xy += offset *  _Outline*0.7;

		o.color = _OutlineColor;
		return o;
	}


#pragma vertex vert  
#pragma fragment frag  
	half4 frag(v2f i) :COLOR
	{
		//i.color.a = _AP;  
		return i.color;
	}

		ENDCG



	}

		Cull Off
		//haima add this
		//	AlphaTest Greater	0.1
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma surface surf Ramp
		// alphatest:_Cutoff//原本是跟在上面一句的后面
		sampler2D _MainTex;
	fixed4 _Color;

	fixed4 _MaskColor5;
	fixed4 _MaskColor6;
	fixed4 _MaskColor7;

	sampler2D _MaskTex;
	fixed _MaskStrength;

	struct Input
	{
		half2 uv_MainTex;
		//法线  
		float3 worldNormal;
		//视角方向  
		float3 viewDir;
	};
	half4 LightingRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
		//half NdotL = dot (s.Normal, lightDir);  
		//half diff = max(NdotL,0);
		half4 c;
		// c.rgb = _LightColor0.rgb * (atten * 2);  
		c.rgb = s.Albedo;
		c.a = s.Alpha;

		return c;
	}
	void surf(Input IN, inout SurfaceOutput o)
	{

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);

		fixed4 finalMask = c*_Color*_MaskStrength;

		finalMask = lerp(finalMask, c* _MaskColor5*_Color*_MaskStrength, mask.r);
		finalMask = lerp(finalMask, c*_MaskColor6*_Color*_MaskStrength, mask.g);
		finalMask = lerp(finalMask, c*_MaskColor7*_Color*_MaskStrength, mask.b);

		fixed4 col = 0;
		o.Albedo.rgb = finalMask;
		//o.Alpha = c.a-c.a*(1-_AP);

		//half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
		//o.Emission = o.Albedo.rgb * pow(rim, _RimPower);
	}
	ENDCG


	}

		FallBack "Diffuse"
}