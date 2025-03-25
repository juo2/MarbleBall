Shader "Unlit/ColorWithShadow" {
	Properties {
		[HDR] _Color ("Albedo", Vector) = (1,1,1,1)
		_EmissionColor ("Emission Color", Vector) = (0,0,0,1)
		[Header(Stencil)] _Stencil ("Stencil ID [0;255]", Float) = 0
		_ReadMask ("ReadMask [0;255]", Float) = 255
		_WriteMask ("WriteMask [0;255]", Float) = 255
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
		[Header(Rendering)] _Offset ("Offset", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull Mode", Float) = 2
		[Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
		[Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask ("Color Mask", Float) = 15
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}