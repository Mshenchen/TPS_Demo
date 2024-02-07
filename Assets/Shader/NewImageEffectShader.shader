Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //实现黑白照片效果
                //红色亮度：0.2,绿色0.7，蓝色0.1
                // float lumi = col.r *0.2 + col.g * 0.7 + col.b*0.1;
                // fixed4 l = fixed4(lumi,lumi,lumi,1);
                // return l;
                //使屏幕亮度变高
                // col.rgb *= 1;
                // return  col;
                //马赛克
                 
                fixed4 col = tex2D(_MainTex, i.uv);
                return  col;
            }
            ENDCG
        }
    }
}
