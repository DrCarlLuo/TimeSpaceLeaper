Shader "Custom/SpriteDissolve"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		PixelSnap ("Pixel snap", Float) = 0

        _NoiseTexture("Noise Texture",2D) = "white"{}
        _Amount("Amount",Range(0,1)) = 0
        _Rect ("Rect Display", Vector) = (0,0,1,1)
        _DissolveColor ("Dissolve Color", Color) = (1,0,0,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

            sampler2D _NoiseTexture;
            float _Amount;
            fixed4 _Rect;
            fixed4 _DissolveColor;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                float2 localuv = (IN.texcoord - _Rect.xy) / (_Rect.zw - _Rect.xy);
                float value = tex2D(_NoiseTexture, localuv).r - _Amount;
                fixed4 c;
                c = SampleSpriteTexture (IN.texcoord) * IN.color;

                clip(value + 0.05);
                if(c.a <= 0.001)
                    clip(-1);
                if(value < 0)
                    c = _DissolveColor;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}