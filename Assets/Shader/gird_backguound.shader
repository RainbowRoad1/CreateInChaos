Shader "Custom/grid_backguound"
{
    Properties
    {
        width("width", int) = 10
        height("height", int) = 20
        side_width("side_width", float) = 0.2
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
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            int width;
            int height;
            float side_width;

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
                o.uv = v.uv * float2(width + side_width, height + side_width) - 0.5 * float2(side_width, side_width);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = frac(i.uv);
                float2 side = step(float2(0.1, 0.1), uv) * step(float2(0.1, 0.1), 1 - uv);
                float k = side.x * side.y;
                return fixed4(k, k, k, 6) * 0.1;
            }
            ENDCG
        }
    }
}
