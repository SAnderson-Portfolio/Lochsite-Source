Shader "Unlit/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_WaterTex("Water Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_Intensity("Intensity", Range(-0.02, 0.02)) = 0.02

		[Header(Wave X Values)]
		_FrequencyX("Frequency_X", float) = 8
		_AmplitudeX("Amplitude_X", float) = 1.3
		_SpeedX("Speed_X", float) = 0.01
		[Header(Wave Y Values)]
		_FrequencyY("Frequency_Y", float) = 7
		_AmplitudeY("Amplitude_Y", float) = 2.7
		_SpeedY("Speed_Y", float) = 0.01 
		[Header(Blending)]
		_BlendRatio("Blend Ratio Main", Range(0.0, 1.0)) = 0.2
		_BlendRatioRipple("Blend Ratio Ripple", Range(0.0, 1.0)) = 0.3
		_BlendRatioDistort("Blend Ratio Distort", Range(0.0, 1.0)) = 0.1
		[Toggle(Distort_Background)] _DistortBackground("Distort Background", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature Distort_Background
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
				float2 uv4 : TEXCOORD3;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
				float2 uv4 : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _WaterTex;
			float4 _WaterTex_ST;

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			float _MousePositionX;
			float _MousePositionY;
			float _Intensity;
			int _DrawRipple;
			float _DrawRange;

			float _FrequencyX;
			float _FrequencyY;
			float _AmplitudeX;
			float _AmplitudeY;
			float _SpeedX;
			float _SpeedY;

			float _BlendRatio;
			float _BlendRatioRipple;
			float _BlendRatioDistort;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _WaterTex);
				//8 1.3 0.01 are good numbers but testing is required for optimal look.
				o.uv2.x += sin((o.uv2.x + o.uv2.y) * _FrequencyX + _Time.g * _AmplitudeX) * _SpeedX;
				//7 2.7 0.01 are good numbers but testing is required for optimal look.
				o.uv2.y += cos((o.uv2.x - o.uv2.y) * _FrequencyY + _Time.g * _AmplitudeY) * _SpeedY;

				o.uv4 = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv4.x += sin((o.uv4.x + o.uv4.y) * 9.0 + _Time.g * 3.0) * -0.02;
				//7 2.7 0.01 are good numbers but testing is required for optimal look.
				o.uv4.y += cos((o.uv4.x - o.uv4.y) * 9.0 + _Time.g * 3.0) * -0.02;

				o.uv3 = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
                // sample the texture
				float2 mousePosition = float2(_MousePositionX, _MousePositionY);
				fixed4 mainCol;
				fixed4 waterCol = fixed4(0, 0, 0, 0);
				half noiseSample = tex2D(_NoiseTex, i.uv3);
				float2 newSampleLocation = i.uv - (noiseSample * _Intensity);
				if (_DrawRipple == 1  && distance(i.vertex.xy, mousePosition) >= _DrawRange - 20 && distance(i.vertex.xy, mousePosition) <= _DrawRange)
				{//Change the pixels based off distance from mouse position when the user has clicked.
					mainCol = tex2D(_MainTex, newSampleLocation);
					mainCol += float4(0, 0, 0.2, 0);
					waterCol = tex2D(_WaterTex, newSampleLocation);
					return lerp(mainCol, waterCol, _BlendRatioRipple);
				}
				else
				{
				/*#ifdef Distort_Background//Originally i had this section for the designers to play around with by being able to activate and deactivate the distortion effect.
					mainCol = tex2D(_MainTex, i.uv);
					fixed4 offsetCol = tex2D(_MainTex, newSampleLocation);
					fixed4 offsetCol2 = tex2D(_MainTex, i.uv4);
					fixed4 finalOffset = lerp(offsetCol, offsetCol2, 0.5);
					mainCol = lerp(mainCol, finalOffset, _BlendRatioDistort);
					waterCol = tex2D(_WaterTex, i.uv2);
					return lerp(mainCol, waterCol, _BlendRatio);
				#else
					mainCol = tex2D(_MainTex, i.uv);
					waterCol = tex2D(_WaterTex, i.uv2);
					return lerp(mainCol, waterCol, _BlendRatio);
				#endif*/
					//While developing it decided to just always distort the image.
					mainCol = tex2D(_MainTex, i.uv);
					fixed4 offsetCol = tex2D(_MainTex, newSampleLocation);
					fixed4 offsetCol2 = tex2D(_MainTex, i.uv4);
					fixed4 finalOffset = lerp(offsetCol, offsetCol2, 0.5);
					mainCol = lerp(mainCol, finalOffset, _BlendRatioDistort);
					waterCol = tex2D(_WaterTex, i.uv2);
					return lerp(mainCol, waterCol, _BlendRatio);
				}
            }

            ENDCG
        }
    }
}
