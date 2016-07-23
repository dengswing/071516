//Shader "Hidden/RimLightSpecBump" {
Shader "Mobile/RimLightSpecBump" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 2.0

	//add
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
	_Outline("Outline width", Range(.000, 0.05)) = .005
//add
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float4 _RimColor;
float _RimPower;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	half rim = 1 - saturate(dot (normalize(IN.viewDir), o.Normal));
    o.Emission = _RimColor.rgb * pow (rim, _RimPower);
}
ENDCG
	
//add
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

uniform float _Outline;
uniform float4 _OutlineColor;
v2f vert(appdata v)
{
	v2f o;
	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	fixed3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	fixed2 offset = TransformViewToProjection(norm.xy);
	o.pos.xy += offset *  _Outline*0.7;
	o.color = _OutlineColor;
	return o;
}

#pragma vertex vert  
#pragma fragment frag  
half4 frag(v2f i) :COLOR
{
	return i.color;
}

ENDCG
}
//add


}

FallBack "Specular"
}
