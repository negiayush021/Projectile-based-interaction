Shader "Custom/DottedTrajectory"
{
    Properties
    {
        _Color ("Dot Color", Color) = (1, 1, 1, 1)
        _DotSize ("Dot Size", Range(0.01, 0.5)) = 0.1
        _DotSpacing ("Dot Spacing", Range(0.1, 2.0)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float4 _Color;
            float _DotSize;
            float _DotSpacing;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Tile the UV along X axis based on spacing
                float tiled = fmod(i.uv.x / _DotSpacing, 1.0);

                // Center UV for circle calculation
                float2 centeredUV;
                centeredUV.x = tiled - 0.5;
                centeredUV.y = i.uv.y - 0.5;

                // Calculate distance from center of each tile
                float dist = length(centeredUV);

                // If outside dot radius, discard (transparent)
                if (dist > _DotSize)
                    discard;

                // Smooth edges
                float alpha = 1.0 - smoothstep(_DotSize - 0.02, _DotSize, dist);

                return fixed4(_Color.rgb, _Color.a * alpha);
            }

            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
