Shader "Custom/ghost block"
{
    Properties
    {
        _MainTex("tex", 2D) = "white"{}
        _RendererColor ("RendererColor", Color) = (1.0,1.0,1.0,1.0)
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

            sampler2D _MainTex;
            float4 _RendererColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 pos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = v.vertex.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(frac(i.pos.x + i.pos.y + _Time.y) - 0.3);
                float4 col = tex2D(_MainTex, i.uv);
                return fixed4(col.rgb, col.a * (0.5 + 0.25 * sin(_Time.y))) * _RendererColor;
            }
            ENDCG
        }
    }
}
