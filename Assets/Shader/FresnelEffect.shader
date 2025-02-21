Shader "Custom/FresnelTransparentSurface" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", Range(0,10)) = 2.0
        _FresnelScale("Fresnel Scale", Range(0,5)) = 1.0
    }
        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Lambert alpha:fade

            struct Input {
                float3 worldNormal;
                float3 worldViewDir;
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelScale;

            void surf(Input IN, inout SurfaceOutput o) {
                // 主纹理颜色
                o.Albedo =  _Color.rgb;

                // 计算菲涅尔效果
                float fresnelTerm = pow(1.0 - saturate(dot(IN.worldNormal, IN.worldViewDir)), _FresnelPower);
                fresnelTerm = 1.0 - (1.0 - fresnelTerm) * _FresnelScale; // 控制亮度范围
                
                o.Emission = saturate(_Color * fresnelTerm);

                // 应用到透明通道
                o.Alpha = fresnelTerm * _FresnelColor.a;
            }
            ENDCG
    }
        FallBack "Diffuse"
}