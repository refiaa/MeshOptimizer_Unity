Shader "Refiaa/Wireframe"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _WireColor ("Wire Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        fixed4 _EmissionColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _EmissionColor.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        Pass
        {
            Name "Wireframe"
            Tags { "LightMode" = "ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma target 4.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };

            fixed4 _WireColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            [maxvertexcount(24)]
            void geom(triangle v2f input[3], inout LineStream<g2f> OutputStream)
            {
                for (int i = 0; i < 3; i++)
                {
                    g2f o;
                    o.pos = input[i].pos;
                    o.color = _WireColor;
                    OutputStream.Append(o);

                    o.pos = input[(i+1)%3].pos;
                    o.color = _WireColor;
                    OutputStream.Append(o);
                }
            }

            fixed4 frag(g2f IN) : SV_Target
            {
                return IN.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
