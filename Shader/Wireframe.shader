Shader "Refiaa/Wireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WireColor ("Wire Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _WireColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            [maxvertexcount(6)]
            void geom(triangle v2f inTriangles[3], inout LineStream<g2f> outLines)
            {
                for (int i = 0; i < 3; i++)
                {
                    g2f o;
                    o.pos = inTriangles[i].pos;
                    o.color = _WireColor;
                    outLines.Append(o);

                    o.pos = inTriangles[(i+1)%3].pos;
                    o.color = _WireColor;
                    outLines.Append(o);
                }
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
