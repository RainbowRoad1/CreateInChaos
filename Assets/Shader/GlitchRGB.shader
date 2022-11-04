Shader "Custom/GLitchRGB"
{
    Properties
    {
        _MainTex("tex", 2D) = "white"{}
        _offset("offset", float) = 0.03
        _alpha("alpha", float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }
        Cull Off ZWrite Off ZTest Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #define PI 3.1415926

            sampler2D _MainTex;
            float _offset;
            float _alpha;
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float d = _offset * _alpha;
                float a = _alpha * 0.2;
                float3 col;
                col.r = tex2D(_MainTex, float2(i.uv.x + d, i.uv.y)).r;
                col.g = tex2D(_MainTex, float2(i.uv.x - d, i.uv.y)).g;
                col.b = tex2D(_MainTex, i.uv).b;
                return fixed4(a + (1 - a) * col, 1);
            }
            ENDCG
        }
    }
}
