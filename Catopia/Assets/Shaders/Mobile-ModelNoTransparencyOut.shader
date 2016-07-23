Shader "Mobile/ModelNoTransparencyOut"
{
	Properties
	{
		_Color("Main Color", Color) = (0.9,0.9,0.9,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		//_Outline ("Outline width",float) = 0.1
		_Outline("Outline width", Range(.000, 0.05)) = .005


		_MainTex("Base (RGB)", 2D) = "white" { }
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
		CGINCLUDE
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
	ENDCG
		SubShader
	{
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
#pragma vertex vert  
#pragma fragment frag  
		half4 frag(v2f i) :COLOR
	{
		//i.color.a = _AP;  
		return i.color;
	}
		ENDCG
	}
		//Tags { "RenderType"="Transparent" "Queue"="Transparent" }  
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

		finalMask = lerp(finalMask,c* _MaskColor5*_Color*_MaskStrength,mask.r);
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
		Fallback "Diffuse"
}
