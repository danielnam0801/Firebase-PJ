Shader "Gondr/Shader18"
{
    Properties
    {        
        _MainTex("Main Texture", 2D) = "white" {}
        _Duration("Duration", Float) = 6.0
        _Slide("Slide", Range(0,1)) = 0
        _WaveCount("WaveCount", Range(1,100)) = 1
        _WaveSpeed("WaveSpeed", Range(1,100)) = 6
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 vertex: SV_POSITION;  //위치
                float2 uv: TEXCOORD0;
                float4 position: TEXCOORD1;  //커스텀 데이터
                float4 screenPos: TEXCOORD2;
            };
    
            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex;
                o.uv = v.texcoord;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float _Duration;
            float _Slide;
            float _WaveCount;
            float _WaveSpeed;

            float2 rotate(float2 pt, float theta, float aspect)
            {
                float c = cos(theta);
                float s = sin(theta);
                float2x2 mat = float2x2(c, -s, s, c);
                pt.y /= aspect; //사각형 공간으로 가져와서 회전시키고 다시 원래 공간으로 되돌린다.
                pt = mul(pt, mat);
                pt.y *= aspect;
                return pt;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = i.position.xy * 2.0;
                float len = length(pos);
                float2 ripple = i.uv + pos / len * 0.03 * cos(len * _WaveCount - _Time.y * _WaveSpeed);
                float theta = fmod(_Time.y, _Duration) * (UNITY_PI * 2 / _Duration);
                float delta = (sin(_Time.y) + 1.0) / 2.0;
                float2 uv = lerp(ripple, i.uv, delta);
                fixed3 color = tex2D(_MainTex, uv).rgb;
                
                return fixed4( color, 1.0 );
            }
            ENDCG
        }
    }
}
