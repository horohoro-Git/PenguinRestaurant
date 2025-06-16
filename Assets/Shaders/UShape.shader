Shader "Custom/TopBottomBendingPaper"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TopBend ("Top Bend Amount", Range(-1, 1)) = 0.5  // 양수=오른쪽, 음수=왼쪽
        _BottomBend ("Bottom Bend Amount", Range(-1, 1)) = -0.5
        _BendSharpness ("Bend Sharpness", Range(1, 10)) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TopBend;
            float _BottomBend;
            float _BendSharpness;

            v2f vert (appdata v)
            {
                v2f o;
                
                // 1. 상단/하단 구분 (0=하단, 1=상단)
                float isTop = step(0.5, v.uv.y); // uv.y > 0.5면 1, 아니면 0
                
                // 2. 상단 또는 하단 굴곡량 선택
                float bendAmount = lerp(_BottomBend, _TopBend, isTop);
                
                // 3. 굴곡 적용 (Y 위치에 따라 강도 조절)
                float bendFactor = pow(abs(v.uv.y - 0.5) * 2, _BendSharpness); // 중앙=0, 끝=1
                v.vertex.x += bendAmount * bendFactor;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}