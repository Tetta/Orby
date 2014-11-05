Shader "Ferr/Unlit Textured Vertex Color" {
	Properties {
		_MainTex("Texture (RGB)", 2D) = "white" {}
	}
	SubShader {
		LOD 200
		Cull Off
		
		Pass {
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4    _MainTex_ST;

			struct appdata_ferr {
			    float4 vertex   : POSITION;
			    float4 texcoord : TEXCOORD0;
			    fixed4 color    : COLOR;
			};
			struct VS_OUT {
				float4 position : SV_POSITION;
				float4 color    : COLOR;
				float2 uv       : TEXCOORD0;
			};

			VS_OUT vert (appdata_ferr input) {
				VS_OUT result;
				result.position = mul (UNITY_MATRIX_MVP, input.vertex);
				result.uv       = TRANSFORM_TEX (input.texcoord, _MainTex);
				result.color    = input.color;

				return result;
			}

			half4 frag (VS_OUT input) : COLOR {
				half4 color = tex2D(_MainTex, input.uv);
				return half4(color.rgb * input.color.rgb, 1);
			}
			ENDCG
		}
	}
}
