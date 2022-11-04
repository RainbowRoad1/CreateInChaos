Shader "Custom/Boom"
{
    Properties
    {
        _Rate("Start time", float) = 0.0
        _Cut("Cut", float) = 0.0
    }
    SubShader 
    {
        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }
        ZWrite off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Rate;
            float _Cut;

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

            float Rand(float2 c)
            {
                return frac(sin(dot(c,float2(12.9898,78.233)))*43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Rate;
                float2 c = float2(cos(_Cut), sin(_Cut)) * (1 - t);
                t = sqrt(1 - (1 - t) * (1 - t)) * 2;
                clip(min(1, t) - length(i.uv));
                float d = length(i.uv - c) - t + 1;
                clip(d);
                return d + 0.5;
            }
            ENDCG
        }
    }
}
