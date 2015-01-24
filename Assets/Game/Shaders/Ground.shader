Shader "Custom/Ground" 
{
	Properties 
	{
		_DirtColor("Dirt Color", color) = (1,1,1,1)
		_DirtTex ("Dirt (RGB)", 2D) = "white" {}
		_DirtTile ("Dirt Tile", float) = 1

		_GrassColor("Grass Color", color) = (1,1,1,1)
		_GrassTex ("Grass (RGB)", 2D) = "white" {}
		_GrassTile ("Grass Tile", float) = 1

		_BlendModulate ("Grass (RGB)", 2D) = "grey" {}
		
		_NormalMap ("Normal (RGB)", 2D) = "grey" {}
		_NormalMapScale ("Normal Map Scale", float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Illustrative

//--------------------------------------------------------
        //--------------------- Lighting Variables ---------------
        //--------------------------------------------------------
        
        sampler2D _DiffuseWarp;
        sampler2D _SpecWarp;
        samplerCUBE _IBLDiffuseCube;
        float _AmbientIntensity;
        
        
		
		//---------------------------------------------------------
		//--------------------- Lighting --------------------------
		//---------------------------------------------------------
		struct Ill_SurfaceOutput
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed3 Specular;
			fixed Gloss;
			fixed Alpha;
		};
		
		half4 LightingIllustrative (Ill_SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			// Calculate diffuse
			fixed halfLambert = max (0, dot (s.Normal, lightDir));
			fixed diffuseValue = (dot (s.Normal, lightDir) * 0.5) + 0.5;
			half3 warpedDiffuse = tex2D(_DiffuseWarp, half2(diffuseValue,0.5));
			
			
			// Gaussian Specular
			half3 halfway = normalize (lightDir + viewDir);
			half Dot = saturate(dot(s.Normal, halfway));
			half Threshold = 0.04;
			half CosAngle = pow (Threshold, 1 / (s.Gloss*2));
			half NormAngle = (Dot -1) / (CosAngle -1);
			half spec = exp(- NormAngle * NormAngle ) * halfLambert;
			//half3 warpedSpec = tex2D(_SpecWarp, half2(clamp(spec,0,1),0.5));
			half3 warpedSpec = spec;
			half3 specFinal = warpedSpec * s.Specular * _LightColor0.rgb * clamp(diffuseValue * 4,0,1) ;
			
			
			fixed4 c;			
			// Combine diffuse and specular
			c.rgb = (s.Albedo * warpedDiffuse * 2 * _LightColor0.rgb + specFinal)* atten;	
			//c.rgb = (s.Albedo * diffuseValue * 2 * _LightColor0.rgb + spec * s.Specular * _LightColor0.rgb )* atten;				
			c.a = s.Alpha;	
			return c;
		}

		sampler2D _MainTex;

		float _ClearingRadius;

		float4 _DirtColor;
		sampler2D _DirtTex;
		float _DirtTile;

		float4 _GrassColor;
		sampler2D _GrassTex;
		float _GrassTile;

		sampler2D _BlendModulate;
		
		sampler2D _NormalMap;
		float _NormalMapScale;

		struct Input 
		{
			float3 worldPos;
			float2 uv_DirtTex;
			float2 uv_GrassTex;
		};

		void surf (Input IN, inout Ill_SurfaceOutput o) 
		{
			float2 gridUV = (IN.worldPos.xz + 0.5f) / _NormalMapScale;

			o.Normal = UnpackNormal (tex2D (_NormalMap,gridUV));
		
			half4 dirt = tex2D (_DirtTex, IN.worldPos.xz * _DirtTile) * _DirtColor;
			half4 grass = tex2D (_GrassTex, IN.worldPos.xz * _GrassTile) * _GrassColor;
			half4 blendModulate = tex2D (_BlendModulate, gridUV);

			float distanceFromCenter = length(floor(IN.worldPos + 0.5f));
			float blendRadius = 3.0f;

			float blend = distanceFromCenter - _ClearingRadius + blendRadius;
			blend /= blendRadius;
			blend = clamp(blend + (blendModulate - 0.5f),0,1);			

			o.Albedo = lerp(dirt.rgb,grass.rgb,blend);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
