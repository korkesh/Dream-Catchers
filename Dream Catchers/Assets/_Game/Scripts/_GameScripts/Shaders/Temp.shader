Shader "Custom/MyToonShader" {
	Properties{
		_MainTint("Diffuse Tint", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecPower("Specular Power", Range(0,30)) = 1

		_ReflectionCoefficient("Reflection Coefficient", Range(0,1)) = 0.3
		_ToonLevels("Toon Levels", Range(1, 10)) = 4

		// Colour of toon outline
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
	}
		SubShader{
		//Tags { "RenderType"="Opaque" }

		CGPROGRAM
#pragma surface surf Toon

		sampler2D _MainTex;
	float4 _MainTint;
	float4 _SpecularColor;
	float _SpecPower;

	float _ReflectionCoefficient;
	float _ToonLevels;

	inline fixed4 LightingToon(SurfaceOutput a, fixed3 lightDir, half3 viewDir, fixed atten) {
		//Calculate diffuse and the reflection vector

		float NdotL = dot(a.Normal, lightDir);
		float3 reflectance = 2.0 * NdotL  * a.Normal - lightDir;
		float3 c_l = _LightColor0.rgb * max(NdotL, 0);
		fixed4 c_r = _MainTint / 3.14f;
		float c_p = _ReflectionCoefficient * (_SpecPower + 2) / (2 * 3.14f);

		_ToonLevels = floor(_ToonLevels);
		float scaleFactor = 1.0f / _ToonLevels;

		float3 ambient = c_r * floor(max(NdotL, 0) * _ToonLevels) * _LightColor0.rgb * scaleFactor;
		float3 diffuse = c_r * floor(max(NdotL, 0) * _ToonLevels) * _LightColor0.rgb * NdotL * scaleFactor;

		float specMask;
		if (pow(dot(normalize(reflectance),normalize(viewDir)) , _SpecPower) > 0.5)
			specMask = 1.0f;
		else specMask = 0.0f;

		float3 spec = c_p * c_l * pow(dot(normalize(reflectance),normalize(viewDir)) , _SpecPower) * _SpecularColor * specMask;

		fixed4 c;
		c.rgb = spec + ambient + diffuse;
		c.a = 1.0;
		return c;

	}

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _MainTint;
		//o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
	}

	ENDCG
		// Use the Outline Pass from the default Toon shader
		UsePass "Toon/Basic Outline/OUTLINE"
	}
		FallBack "Diffuse"
}
