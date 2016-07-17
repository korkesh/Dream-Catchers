Shader "Custom/CellShaderWithRamp" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RampTex("Ramp", 2D) = "white" {}
		_Smoothness ("Edge Smoothness", Range(-1,1)) = 0.025
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf CellShadingForward

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		float _Smoothness;
		sampler2D _RampTex;

		half4 LightingCellShadingForward(SurfaceOutput s, half3 lightDir, half attenuation) 
		{
			half NdotL = dot(s.Normal, lightDir); // Surface Normals dotted with the light direction vector
			//NdotL = smoothstep(0, _Smoothness, NdotL);
			NdotL = tex2D(_RampTex, fixed2(NdotL, 0.5));

			half4 c; // Colour
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * attenuation * 2); // Calculate colour 
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
