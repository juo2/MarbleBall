Shader "Custom/Iridescence/SimpleIridescence" {
	Properties {
		[Space(20)] [Header(MainTex and ColorRamp)] [Space(20)] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Range(0,1)] _Emission ("Emission", Float) = 0
		[HDR] _EmissionColor ("Color", Vector) = (0,0,0,1)
		_ColorRamp ("ColorRamp", 2D) = "white" {}
		_Blend ("Blend", Range(0, 1)) = 0.5
		[Space(20)] [Header(Smoothness and Metallic)] [Space(20)] _Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}