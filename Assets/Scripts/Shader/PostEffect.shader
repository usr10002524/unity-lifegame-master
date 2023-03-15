Shader "PostEffect"
{
    Properties{
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _TexelInterval("Texel Interval", float) = 2 //サンプリングするテクセルの間隔
        _ColorRatio("Color Ratio", float) = 0.75 //色の補正値
    }
    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off

        Tags { "RenderType"="Opaque" }

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 _MainTex_TexelSize;
            float2 _Direction;
            float _TexelInterval;
            float _ColorRatio;
            float _LumiThreshold;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = _Direction * _MainTex_TexelSize.xy; //サンプリングの方向を決定

                //////ガウス関数を事前計算した重みテーブル
                float weights[8] = 
                {
                    0.12445063, 0.116910554, 0.096922256, 0.070909835,
                    0.04578283, 0.02608627,  0.013117,    0.0058206334
                };

                fixed4 color = 0;
                for (int j = 0; j < 8; j++) 
                {
                    float2 offset = dir * ((j + 1) * _TexelInterval - 1); //_TexelIntervalでサンプリング距離を調整
                    color.rgb += tex2D(_MainTex, i.uv + offset) * weights[j]; //順方向をサンプリング＆重みづけして加算
                    color.rgb += tex2D(_MainTex, i.uv - offset) * weights[j]; //逆方向をサンプリング＆重みづけして加算
                }

                color.rgb *= _ColorRatio;
                color.a = 1;
                
                

                return color;
            }
            ENDCG
        }
    }
}