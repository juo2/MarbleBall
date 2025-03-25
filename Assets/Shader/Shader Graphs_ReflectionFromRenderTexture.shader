Shader "Shader Graphs/ReflectionFromRenderTexture" {
	Properties {
		_FresnelPower ("FresnelPower", Float) = 4
		_Overlay ("Overlay", Vector) = (1,0,0,1)
		[NoScaleOffset] _RenderTexture ("RenderTexture", 2D) = "white" {}
		_Base_Color ("Base Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _BaseTexture ("BaseTexture", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		_Metallic ("Metallic", Range(0, 1)) = 1
		[HideInInspector] _BUILTIN_QueueOffset ("Float", Float) = 0
		[HideInInspector] _BUILTIN_QueueControl ("Float", Float) = -1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}