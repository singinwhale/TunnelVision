Shader "Unlit/TunnelShader"
{
	Properties
	{
		_backgroundColor ("Background Color", Color) = (0,0,0,1)
		_foregroundColor ("Foreground Color", Color) = (0,0,0,1)
		_MainTex("Texture",2D) = "white"
		_offset ("Offset", Float) = 0
		_length ("Length", Float) = 0
		_sharpness ("Sharpness", Float) = 15
		_frequency ("Frequency", Float) = 100
		_intensity ("Intensity", Float) = 1
		_rotationSpeed ("Rotation Speed", Float) = 1
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

			float _offset;
			float _length;
			fixed4 _backgroundColor;
			fixed4 _foregroundColor;
			float _sharpness;
			float _intensity;
			float _frequency;
			float _rotationSpeed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float weight = clamp((sin(i.uv.y*_frequency + i.uv.x*_length*_rotationSpeed)*0.5+0.5)*_sharpness,0,1);
				fixed4 col = lerp(_foregroundColor, _backgroundColor, weight);
				//col.w = 0;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col*_intensity;
			}
			ENDCG
		}
	}
}
