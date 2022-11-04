Shader "Custom/sky_background"
{
    Properties
    {
        btm_color("color1", Color) = (1, 0, 0, 1)
        mid_color("color2", Color) = (1, 0, 1, 1)
        top_color("color3", Color) = (0, 0, 1, 1)
        _TilingOffset("Y",vector) = (1, 1, 0, 0)
        mul_out("mul", float) = 1
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
            #define PI 3.1415926

            float4 btm_color;
            float4 mid_color;
            float4 top_color;
            vector _TilingOffset;
            float mul_out;
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
                o.uv = v.uv * _TilingOffset.xy + _TilingOffset.zw;
                o.uv.x -= _TilingOffset.z * 0.5;
                o.uv.y *= 2.0;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float a = i.uv.y + sin(i.uv.x * PI) * 0.1;
                return (a < 1 ? lerp(btm_color, mid_color, a) : lerp(mid_color, top_color, a - 1)) * mul_out;
            }
            ENDCG
        }
    }
}
