Shader "UI/StaticGlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _Speed ("Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Intensity;
            float _Speed;
            float _Time;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = frac(sin(dot(i.uv * _Time.y * _Speed, float2(12.9898, 78.233))) * 43758.5453);
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb += noise * _Intensity;
                return col;
            }
            ENDCG
        }
    }
}
