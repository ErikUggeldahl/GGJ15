// unity IBL
// http://jordanmellow.com/news.php?extend.17

Shader "Custom/BuildingPreview" 
{
	Properties
    {  
    	// Surface
        _DiffuseColor ("Diffuse Colour",color) = (1.0,1.0,1.0,1.0)
        _DiffuseMultiply ("Diffuse Brightness",float) = 1.0
         
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _SpecularMultiply ("Specular Brightness",float) = 0.1
        _SpecAdd ("Specular Boost", float) = 0
        _SpecMap ("Specular Map (RGB)", 2D) = "white" {}    
        _Gloss ("Specular Glossiness", float) = 20
         
        _FresnelPower ("Fresnel Power",float) = 1.0
        _FresnelMultiply ("Fresnel Multiply", float) = 0.2
        _FresnelBias ("Fresnel Bias", float) = -0.1
         
        _RimPower ("RimLight Power",float) = 1.0
        _RimMultiply ("RimLight Multiply", float) = 0.2
        _RimBias ("RimLight Bias", float) = 0
    }
	SubShader 
	{
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Illustrative alpha
		#pragma target 3.0
        
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
		
		//--------------------------------------------------------
        //--------------------- Surface Variables ----------------
        //--------------------------------------------------------
        float3 _DiffuseColor;
        float _DiffuseMultiply;
         
        sampler2D _NormalMap;
        float _NormalIntensity;   
         
        float _SpecularMultiply;
        float _SpecAdd;
        sampler2D _SpecMap;
         
        float _Gloss;
         
        float _FresnelPower;
        float _FresnelMultiply;
        float _FresnelBias;
         
        float _RimPower;
        float _RimMultiply;
        float _RimBias;
		
		//----------------------------------------------------------
		//--------------------- Surface Shader ---------------------
		//----------------------------------------------------------
		struct Input 
		{
			float2 uv_DiffuseMap;
            float3 worldNormal;
            
            float3 viewDir;
            
            float4 color : COLOR;
            
            INTERNAL_DATA
		};

		void surf (Input IN, inout Ill_SurfaceOutput o) 
		{
			float3 Diff1 = _DiffuseMultiply * _DiffuseColor.rgb * IN.color;
			
			o.Albedo.rgb = Diff1;
			float desaturatedAlbedo = (o.Albedo.r + o.Albedo.g + o.Albedo.b)/3;

            float3 worldNormal = IN.worldNormal;
            
            float facing = saturate(1.0 - max(dot( normalize(IN.viewDir.xyz), o.Normal), 0.0)); 
            
            // IBL Ambient
            float3 cubeAmbient = texCUBE(_IBLDiffuseCube,worldNormal);
            o.Emission += o.Albedo.rgb * cubeAmbient * _AmbientIntensity;           

	        float rim = max(_RimBias + (1.0-_RimBias) * pow(facing, _RimPower), 0);
	        rim = rim * _RimMultiply;
	        //rim *= max(dot( float3(0,1,0), worldNormal), 0);
	        rim *= clamp((dot( float3(0,1,0), worldNormal) * 0.5) + 0.5 + 0.2,0,1);
	        o.Emission += rim * o.Albedo.rgb * cubeAmbient;
	        o.Alpha = clamp(0.4f + rim,0,1);
            
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
