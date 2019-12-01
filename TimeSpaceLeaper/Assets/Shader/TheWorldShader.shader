Shader "Custom/TheWorldShader"
{
	Properties
	{
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _NoiseTex("Texture (R,G=X,Y Distortion; B=Mask; A=Unused)", 2D) = "white" {}
        _Intensity("Intensity", Float) = 0.1
        _Offset("Offset", Float) = 0
        _Color("Tint", Color) = (1,1,1,1)
            _Hue("Hue", Range(0,360)) = 0
        _Saturation("Saturation", Range(0,1)) = 1
        _Value("Value",Range(0,1)) = 1
	}
	SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent" }

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
        {
            "_BackgroundTexture"
        }

		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Off
		// Render the object with the texture generated above, and invert the colors
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;
            sampler2D _NoiseTex;
            float _Intensity;
            float4 _NoiseTex_ST;
            float _Offset;

            int _Hue;
            float _Saturation;
            float _Value;

            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 realPos = (i.grabPos + float2(_Offset,_Offset))%1.0;
                half4 d = tex2D(_NoiseTex, realPos);
                float4 p = i.grabPos + (d * _Intensity);

                half4 bgcolor = tex2Dproj(_BackgroundTexture, p);
                float3 changedColor = rgb2hsv(bgcolor.rgb);
                changedColor.x += _Hue / 360.0;
                changedColor.y *= _Saturation;
                changedColor.z *= _Value;
                if(changedColor.x > 1.0) changedColor.x -= 1.0;
                bgcolor.rgb = hsv2rgb(changedColor);
                return bgcolor;
            }
            ENDCG
        }
	}
}