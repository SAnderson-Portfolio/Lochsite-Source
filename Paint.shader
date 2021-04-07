Shader "Unlit/Paint"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white"
		_PaintTex("Paint Texture", 2D) = "white"
		
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100 //Level of Detail

        Pass
        {

		Lighting Off

            CGPROGRAM 
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // Helper functions for rendering

            struct appdata //Pass information in, in a packed array, 
            {
                float4 vertex : POSITION; //Symantic binder
                float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
            };

            struct v2f//Vert 2 frag
            {
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
                float4 pos : SV_POSITION;// Screen space position
            };

            sampler2D _MainTex;
			sampler2D _PaintTex;
            float4 _MainTex_ST;
			float4 _PaintTex_ST;

            v2f vert (appdata v)//Vertex function
            {
                v2f o;//Output
                o.pos = UnityObjectToClipPos(v.vertex);//Change to clip space
                o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord1, _PaintTex);
                return o;
            }

            half4 frag (v2f o) : SV_Target//Fragment function
            {
				half4 main_color = tex2D(_MainTex, o.uv0); // main texture
				half4 paint = (tex2D(_PaintTex, o.uv1)); // painted on texture

				float4 endresult;
				if (paint.r <= 0.0f)
					discard;
				else
					endresult = paint;
				return endresult;
            }
            ENDCG
        }
    }
}
