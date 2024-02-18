Shader "Unlit/Circle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.y+=sin(v.vertex.z*10+_Time.y);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float circle(float2 uv,float pos, float r,float s)
            {
                return smoothstep(r-s,r+s,length(uv-pos));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 red = fixed4(1,0,0,1);
                fixed4 blue = fixed4(0,0,1,1);
                //float isLeft = smoothstep(0.9,0,1,i.uv.x);
                //col*=lerp(red,blue,isLeft);
                //i.uv = i.uv*2-1;
                
                //col *= step(-.8,i.uv.x);
                //渐变条纹
                //i.uv = floor(i.uv*10)/10;
                //col*=i.uv.x;
                //栅栏
                //col*=1-step(0.95,sin(i.uv.x*100));
                //格子
                //col*=1-step(0.95,sin(i.uv.x*100));
                //col*=1-step(0.95,sin(i.uv.y*100));
                //涟漪
                //i.uv = i.uv*2-1;
                //col*=step(0.95,sin(length(i.uv)*100));
                //i.uv = i.uv*2-1;
                //col*=smoothstep(0.4,0.6,sin(length(i.uv)*30+(sin(_Time.y*5)/2 +0.5)*5));
                //col*=lerp(fixed4(1,0,0,1),fixed4(0,0,1,1),sin(_Time.y*5)/2 +0.5);
                return col;
               
            }
            ENDCG
        }
    }
}
