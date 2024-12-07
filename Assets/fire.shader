Shader "Custom/PSXFireShader"
{
    Properties
    {
        _MainTex("Flame Texture", 2D) = "white" {}
        _Color("Color Tint", Color) = (1, 0.5, 0, 1)
        _ScrollSpeed("Scroll Speed", Float) = 1.0
        _Pixelation("Pixelation Level", Float) = 256.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _ScrollSpeed;
            float _Pixelation;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 PixelateUV(float2 uv, float pixelation)
            {
                float2 pixelatedUV = floor(uv * pixelation) / pixelation;
                return pixelatedUV;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Scroll the texture UVs
                float2 uv = i.uv;
                uv.y += _Time.y * _ScrollSpeed;

                // Pixelation
                uv = PixelateUV(uv, _Pixelation);

                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, uv);

                // Apply color tint
                texColor *= _Color;

                // Add emission-like brightness
                texColor.rgb += texColor.a * 0.5;

                return texColor;
            }
            ENDCG
        }
    }
}
