// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent_SinCurve" {
	Properties{
		[HDR] _Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_WaveFrequency("Wave Frequency", float) = 1
		_WaveMagnitude("Wave Magnitude", Range(0,10)) = 0
		_WaveOffset("Wave Offset", float) = 0
	}

		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
					#pragma multi_compile_fog

					#include "UnityCG.cginc"

					#define PI 3.1415926538

					struct appdata_t {
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_FOG_COORDS(1)
						UNITY_VERTEX_OUTPUT_STEREO
					};

					fixed4 _Color;
					sampler2D _MainTex;
					float4 _MainTex_ST;
					float _WaveFrequency;
					float _WaveMagnitude;
					float _WaveOffset;

					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
						float u = i.texcoord.x;
						float fade = (1 - abs(u - 0.5 ) * 2);
						float fade2 = (1 - abs(u - 0.5 *_WaveMagnitude) * 1/_WaveMagnitude);

						float _WaveMagnitude10 = _WaveMagnitude * 10;
						float a = (u - _Time.y * 0.5) * _WaveMagnitude10;//u * _WaveFrequency;// 
						float s = (sin(a) + 1) * _WaveMagnitude * 0.1 * fade; //abs(sin(a)) * 0.25;// 
						s *= fade2;



						float speed = 10 * _WaveMagnitude;
						float m = 1;

						s *= sin(_Time.y * speed);// ((sin(_Time.y* speed) + 0.8) * 0.5);

						s += _WaveOffset;

						//col.rgb = 1;
						col.a = 1;


							if (i.texcoord.y > (s - 0.005) && i.texcoord.y < (s + 0.005)) {

								col.rgb = 1;
								//col.rgb *= 10;
							}




						UNITY_APPLY_FOG(i.fogCoord, col);
						return col;
					}
				ENDCG
			}
		}

}
