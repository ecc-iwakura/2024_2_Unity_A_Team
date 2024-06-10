Shader "Custom/WaveTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveAmplitude ("Wave Amplitude", Float) = 1.0
        _WaveFrequency ("Wave Frequency", Float) = 10.0
        _Progress ("Progress", Float) = 0.0
        _WaveColor ("Wave Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
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
            float _WaveAmplitude;
            float _WaveFrequency;
            float _Progress;
            float4 _WaveColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // 波の方向を変えないために変更
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distanceFromCenter = distance(i.uv, float2(0.5, 0.5));
                float wave = sin((_Progress - distanceFromCenter) * _WaveFrequency) * _WaveAmplitude; // 波の進行方向を中心から外側に変更
                float alpha = smoothstep(0.0, 1.0, _Progress - wave);
                fixed4 color = tex2D(_MainTex, i.uv);
                color.rgb *= _WaveColor.rgb; // 波エフェクトの色を掛け合わせる
                return color * alpha;
            }
            ENDCG
        }
    }
}
