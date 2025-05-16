Shader "Custom/HoloShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (0,1,1,1)
        _SecondaryColor ("Secondary Color", Color) = (1,0,1,1)
        _ColorBlend ("Color Blend", Range(0, 1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ScanSpeed ("Scan Speed", Range(0.1, 10)) = 2.0
        _ScanWidth ("Scan Width", Range(0.1, 2)) = 0.5
        _FresnelPower ("Edge Glow Power", Range(0.1, 5)) = 2.0
        _MinAlpha ("Minimum Alpha", Range(0, 1)) = 0.3
        _MaxAlpha ("Maximum Alpha", Range(0, 1)) = 0.9
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Unlit alpha:fade noforwardadd
        #pragma multi_compile_instancing
        
        sampler2D _MainTex;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            // Properties 블록에 정의된 것과 동일한 이름을 사용하지만
            // 여기서는 인스턴싱 버퍼 내에서만 사용됩니다
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _SecondaryColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _ColorBlend)
            UNITY_DEFINE_INSTANCED_PROP(float, _ScanSpeed)
            UNITY_DEFINE_INSTANCED_PROP(float, _ScanWidth)
            UNITY_DEFINE_INSTANCED_PROP(float, _FresnelPower)
            UNITY_DEFINE_INSTANCED_PROP(float, _MinAlpha)
            UNITY_DEFINE_INSTANCED_PROP(float, _MaxAlpha)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(s.Emission, s.Alpha);
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            
            // 인스턴스화된 프로퍼티 가져오기
            fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            fixed4 secondaryColor = UNITY_ACCESS_INSTANCED_PROP(Props, _SecondaryColor);
            float colorBlend = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorBlend);
            float scanSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _ScanSpeed);
            float scanWidth = UNITY_ACCESS_INSTANCED_PROP(Props, _ScanWidth);
            float fresnelPower = UNITY_ACCESS_INSTANCED_PROP(Props, _FresnelPower);
            float minAlpha = UNITY_ACCESS_INSTANCED_PROP(Props, _MinAlpha);
            float maxAlpha = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxAlpha);
            
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            
            fixed4 blendedColor = lerp(color, secondaryColor, colorBlend);
            o.Emission = blendedColor.rgb * texColor.rgb;
            
            float fresnel = pow(1.0 - saturate(dot(normalize(o.Normal), normalize(IN.viewDir))), fresnelPower);
            
            float scanPos = IN.worldPos.y * 0.2 + _Time.y * scanSpeed;
            float scanWave = smoothstep(0.0, scanWidth, frac(scanPos));
            float scanEffect = lerp(0.7, 1.0, scanWave);
            
            float alpha = lerp(minAlpha, maxAlpha, fresnel * scanEffect);
            o.Alpha = alpha * blendedColor.a;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}