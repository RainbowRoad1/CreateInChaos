Shader "Custom/GLitchBlock"
{
    Properties
    {
        _MainTex("tex", 2D) = "white"{}
        _Speed("_Speed", float) = 0
        _BlockSize("_BlockSize", float) = 0
        _MaxRGBSplitX("_MaxRGBSplitX", float) = 0
        _MaxRGBSplitY("_MaxRGBSplitY", float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }
        Cull Off ZWrite Off ZTest Off
        GrabPass {"_GrabTexture"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
	        float _Speed;
	        float _BlockSize;
	        float _MaxRGBSplitX;
	        float _MaxRGBSplitY;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabUv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
                o.uv = v.uv;
                return o;
            }

            inline float randomNoise(float2 seed)
            {
                return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
            }

            inline float randomNoise(float seed)
            {
                return randomNoise(float2(seed, 1.0));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 projUv = i.grabUv.xy / i.grabUv.w;

                half2 block = randomNoise(floor(projUv * _BlockSize));

                float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
                float splitRGBNoise = pow(randomNoise(7.2341), 17.0);
                float offsetX = displaceNoise - splitRGBNoise * _MaxRGBSplitX;
                float offsetY = displaceNoise - splitRGBNoise * _MaxRGBSplitY;

                float noiseX = 0.05 * randomNoise(13.0);
                float noiseY = 0.05 * randomNoise(7.0);
                float2 offset = float2(offsetX * noiseX, offsetY* noiseY);

                half4 colorR = tex2D(_GrabTexture, projUv);
                half4 colorG = tex2D(_GrabTexture, projUv + offset);
                half4 colorB = tex2D(_GrabTexture, projUv - offset);

                return half4(colorR.r , colorG.g, colorB.z, (colorR.a + colorG.a + colorB.a));
            }
            ENDCG
        }
    }
}
