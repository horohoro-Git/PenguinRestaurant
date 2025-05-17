Shader "Custom/GridShader"
{
    Properties
    {
        _GridColor("Grid Color", Color) = (1,1,1,1)
        _GridSpacing("Grid Spacing", Float) = 2.0
        _LineWidth("Line Width", Float) = 0.1
        _EnableGrid("Enable Grid", Float) = 1.0
        _VectorOffset("Grid Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _GridColor;
            float _GridSpacing;
            float _LineWidth;
            float _EnableGrid;
            float4 _VectorOffset;
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_EnableGrid < 0.5)
                    return fixed4(0, 0, 0, 0); // 완전 투명
                float3 localPos = i.worldPos - _VectorOffset.xyz;
                float2 gridUV = localPos.xz / _GridSpacing;
                float2 grid = abs(frac(gridUV - 0.5) - 0.5) / fwidth(gridUV);
                float gridLine = min(grid.x, grid.y);
              
                // 선의 두께 조절 (값이 작을수록 두꺼운 선)
                float gridLineStrength = 1.0 - smoothstep(0.0, _LineWidth, gridLine);

                // 그리드 선만 색상 적용, 나머지는 완전 투명
                fixed4 col = _GridColor;
                col.a = gridLineStrength * 0.5f;

                return col;
            }
            ENDCG
        }
    }
}