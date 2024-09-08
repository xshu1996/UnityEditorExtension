Shader "UI/Mask/Particles/Alpha Blended Premultiply"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01, 3.0)) = 1.0
        
        _MinX("Min X", Float) = -10
        _MinY("Min Y", Float) = 10
        _MaxX("Max X", Float) = -10
        _MaxY("Max Y", Float) = 10
    }
    
    Category{
        
        Tags { "Queue"="Transparent" "IgnoreProject"="True" "RenderType"="Transparent" }
        Blend One OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off
        
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_particles

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                    float4 vertex : SV_POSITION;
                    float3 vpos : TEXCOORD2;
                };

                
                float _MinX;
                float _MinY;
                float _MaxX;
                float _MaxY;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    
                    o.uv = v.uv;
                    o.color = v.color;
                    o.vpos = v.vertex.xyz;
                    
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    // just invert the colors
                    // col.rgb = 1 - col.rgb;

                    // col.a *= step(_MinX, i.vpos.x) * step(_MinY, i.vpos.y) * step(i.vpos.x, _MaxX) * step(i.vpos.y, _MaxY);
                    col.a *= (i.vpos.x >= _MinX);
                    col.a *= (i.vpos.y >= _MinY);
                    col.a *= (i.vpos.x <= _MaxX);
                    col.a *= (i.vpos.y <= _MaxY);
                    
                    col.rgb *= col.a;
                    
                    return col;
                }
                ENDCG
            }
        }
    }
}
