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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}

        LOD 200
        
      //  ZTest Always

        ZTest LEqual      
        ZWrite On        
        Blend SrcAlpha OneMinusSrcAlpha 
       // Blend SrcAlpha One 
      // Blend One One
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS      : SV_POSITION;
                float2 uv               : TEXCOORD0;
                float3 positionWS       : TEXCOORD1;
                float3 normalWS         : TEXCOORD2;
                float3 viewDirWS        : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            UNITY_INSTANCING_BUFFER_START(Props)
            // Properties 블록에 정의된 것과 동일한 이름을 사용하지만
            // 여기서는 인스턴싱 버퍼 내에서만 사용됩니다
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float4, _SecondaryColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _ColorBlend)
            UNITY_DEFINE_INSTANCED_PROP(float, _ScanSpeed)
            UNITY_DEFINE_INSTANCED_PROP(float, _ScanWidth)
            UNITY_DEFINE_INSTANCED_PROP(float, _FresnelPower)
            UNITY_DEFINE_INSTANCED_PROP(float, _MinAlpha)
            UNITY_DEFINE_INSTANCED_PROP(float, _MaxAlpha)
             UNITY_INSTANCING_BUFFER_END(Props)

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                OUT.positionWS = vertexInput.positionWS;
                OUT.normalWS = normalInput.normalWS;
                OUT.viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;

                return OUT;
            }

            half4 frag(Varyings IN) :SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                float4 secondaryColor = UNITY_ACCESS_INSTANCED_PROP(Props, _SecondaryColor);
                float colorBlend = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorBlend);
                float scanSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _ScanSpeed);
                float scanWidth = UNITY_ACCESS_INSTANCED_PROP(Props, _ScanWidth);
                float fresnelPower = UNITY_ACCESS_INSTANCED_PROP(Props, _FresnelPower);
                float minAlpha = UNITY_ACCESS_INSTANCED_PROP(Props, _MinAlpha);
                float maxAlpha = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxAlpha);

                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float4 blendColor = lerp(color, secondaryColor, colorBlend);

                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), fresnelPower);

                float scanEffect = 1.0;

                float alpha = lerp(minAlpha, maxAlpha, fresnel * scanEffect);
                color.a *= 0.5;

               // half4 finalColor = half4(blendColor.rgb, alpha * color.a);
                float3 finalColor = blendColor.rgb * (1.0 + fresnel * 2.0);
               // float luminance = dot(finalColor, float3(0.3, 0.6, 0.1)); // RGB → Grayscale 변환
               // float3 saturatedColor = lerp(luminance, finalColor, 1.5); // 채도 1.5배 증가
                return half4(finalColor * 2, saturate(alpha * 1));
              //  return finalColor;
                //return half4(saturatedColor, alpha * color.a);
            }

            ENDHLSL
        }
        /*
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
            float3 worldNormal;
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
            
         //   fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            
         //   fixed4 blendedColor = lerp(color, secondaryColor, colorBlend);
            o.Emission = color.rgb;
            
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), normalize(IN.viewDir))), fresnelPower);
            
          //  float scanPos = IN.worldPos.y * 0.2 + _Time.y * scanSpeed;
          //  float scanWave = smoothstep(0.0, scanWidth, frac(scanPos));
            float scanEffect = 1;//lerp(0.7, 1.0, scanWave);
            
            float alpha = lerp(minAlpha, maxAlpha, fresnel * scanEffect);
            color.a *= 0.5f;
            o.Alpha = alpha * color.a;
        }
        ENDCG
        */
    }
    FallBack "Universal Render Pipeline/Transparent"
  //  FallBack "Transparent/Diffuse"
}