Shader "Shader Graphs/RefractionFromRenderTexture" {
	Properties {
		_IoR ("IoR", Range(-1, 15)) = 0.15
		_FresnelPower ("FresnelPower", Float) = 4
		_Specular ("Specular", Float) = 99
		_Color ("Color", Vector) = (1,0,0,1)
		[NoScaleOffset] _RenderTexture ("RenderTexture", 2D) = "white" {}
		[HideInInspector] _BUILTIN_Surface ("Float", Float) = 0
		[HideInInspector] _BUILTIN_Blend ("Float", Float) = 0
		[HideInInspector] _BUILTIN_AlphaClip ("Float", Float) = 0
		[HideInInspector] _BUILTIN_SrcBlend ("Float", Float) = 1
		[HideInInspector] _BUILTIN_DstBlend ("Float", Float) = 0
		[HideInInspector] _BUILTIN_ZWrite ("Float", Float) = 1
		[HideInInspector] _BUILTIN_ZWriteControl ("Float", Float) = 0
		[HideInInspector] _BUILTIN_ZTest ("Float", Float) = 4
		[HideInInspector] _BUILTIN_CullMode ("Float", Float) = 2
		[HideInInspector] _BUILTIN_QueueOffset ("Float", Float) = 0
		[HideInInspector] _BUILTIN_QueueControl ("Float", Float) = -1
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
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}