// unity IBL
// http://jordanmellow.com/news.php?extend.17

Shader "Custom/Illustrative" 
{
	Properties
    {
    	// Lighting
    	//_DiffuseWarp ("Diffuse Warp",2D) = "grey" {}
    	//_SpecWarp ("Spec Warp",2D) = "grey" {}
    	//_IBLDiffuseCube ("IBL Diffuse Cube",CUBE) = "grey" {}
    	//_AmbientIntensity ("Ambient Intensity",float) = 1.0
    	
    	
    	// Surface
        _DiffuseColor ("Diffuse Colour",color) = (1.0,1.0,1.0,1.0)
        _Desaturate ("Desaturate",float) = 0
        _DiffuseMultiply ("Diffuse Brightness",float) = 1.0
        _DiffuseMap ("Diffuse (RGB) AmbientOcclusion(A)", 2D) = "white" {}
        _AmbientOcclusionIntensity ("Ambient Occlusion Intensity",float) = 1.0
         
        _NormalMap ("Normal Map(RGB)", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity",float) = 1.0
         
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
        
        // Blended 2nd texture
        _BlendDiffuseColor ("Blend Diffuse Colour",color) = (1.0,1.0,1.0,1.0)
        _BlendDiffuseMultiply ("Blend Diffuse Brightness",float) = 1.0
        _BlendDiffuseMap ("Blend Diffuse (RGB) AmbientOcclusion(A)", 2D) = "white" {}
        _BlendAmbientOcclusionIntensity ("Blend Ambient Occlusion Intensity",float) = 1.0
         
        _BlendNormalMap ("Blend Normal Map(RGB)", 2D) = "bump" {}
        _BlendNormalIntensity ("Blend Normal Intensity",float) = 1.0
         
        _BlendSpecColor ("Blend Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _BlendSpecularMultiply ("Blend Specular Brightness",float) = 0.1
        _BlendSpecAdd ("Blend Specular Boost", float) = 0
        _BlendSpecMap ("Blend Specular Map (RGB)", 2D) = "grey" {}    
        _BlendGloss ("Blend Specular Glossiness", float) = 20
    }
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Illustrative
		#pragma target 3.0
        #pragma multi_compile NORMALMAP_ON NORMALMAP_OFF
        #pragma multi_compile SPECULAR_ON SPECULAR_OFF
        //#pragma multi_compile FRESNEL_ON FRESNEL_OFF
        #pragma multi_compile RIMLIGHT_ON RIMLIGHT_OFF
        
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
        float _Desaturate;
        sampler2D _DiffuseMap;
        float _AmbientOcclusionIntensity;
         
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
        
        // Blended 2nd Texture
        float3 _BlendDiffuseColor;
        float _BlendDiffuseMultiply;
        sampler2D _BlendDiffuseMap;
        float _BlendAmbientOcclusionIntensity;
         
        sampler2D _BlendNormalMap;
        float _BlendNormalIntensity;   
         
        float _BlendSpecularMultiply;
        float _BlendSpecAdd;
        sampler2D _BlendSpecMap;
         
        float _BlendGloss;
		
		//----------------------------------------------------------
		//--------------------- Surface Shader ---------------------
		//----------------------------------------------------------
		struct Input 
		{
			float2 uv_DiffuseMap;
            float3 worldNormal;
            
            #if FRESNEL_ON || RIMLIGHT_ON
            float3 viewDir;
            #endif
            
            float4 color : COLOR;
            
            INTERNAL_DATA
		};

		void surf (Input IN, inout Ill_SurfaceOutput o) 
		{
			
			
			float4 diffuseMapCol = tex2D (_DiffuseMap, IN.uv_DiffuseMap);
			float AmbientOcclusion1 =  ((1-_AmbientOcclusionIntensity) + (diffuseMapCol.a * _AmbientOcclusionIntensity));
			float3 Diff1 = _DiffuseMultiply * _DiffuseColor.rgb * diffuseMapCol.rgb * IN.color;
			
			o.Albedo.rgb = Diff1;
			float desaturatedAlbedo = (o.Albedo.r + o.Albedo.g + o.Albedo.b)/3;
			o.Albedo.rgb = lerp(o.Albedo,float3(desaturatedAlbedo,desaturatedAlbedo,desaturatedAlbedo),_Desaturate);
			float AmbientOcclusion = AmbientOcclusion1;
             
            #if SPECULAR_ON
            float Gloss1 = _Gloss;
            float3 Spec1 = _SpecAdd + _SpecularMultiply * tex2D (_SpecMap, IN.uv_DiffuseMap) * _SpecColor;
            o.Specular = Spec1;
            o.Gloss = Gloss1;
            #endif
             
            #if NORMALMAP_ON
            float3 Normal1 = UnpackNormal(tex2D(_NormalMap, IN.uv_DiffuseMap));
            //Normal1.z *= 1/_NormalIntensity;
            Normal1 = normalize(Normal1);
            o.Normal = Normal1;
            #endif
            
            #if NORMALMAP_ON
            float3 worldNormal = WorldNormalVector ( IN, o.Normal );
            #else
            float3 worldNormal = IN.worldNormal;
            #endif
            
            // IBL Ambient
            float3 cubeAmbient = texCUBE(_IBLDiffuseCube,worldNormal);
            o.Emission += o.Albedo.rgb * cubeAmbient * _AmbientIntensity * ((1-_AmbientOcclusionIntensity) + (AmbientOcclusion * _AmbientOcclusionIntensity));
             
            #if FRESNEL_ON && SPECULAR_ON || RIMLIGHT_ON   
            float facing = saturate(1.0 - max(dot( normalize(IN.viewDir.xyz), o.Normal), 0.0));     
             
                #if FRESNEL_ON && SPECULAR_ON      
                float fresnel = max(_FresnelBias + (1.0-_FresnelBias) * pow(facing, _FresnelPower), 0);
                fresnel = fresnel * o.Specular * _FresnelMultiply;
                o.Specular *= 1+fresnel;
                #endif         
                 
                #if RIMLIGHT_ON
                float rim = max(_RimBias + (1.0-_RimBias) * pow(facing, _RimPower), 0);
                rim = rim * _RimMultiply;
                //rim *= max(dot( float3(0,1,0), worldNormal), 0);
                rim *= clamp((dot( float3(0,1,0), worldNormal) * 0.5) + 0.5 + 0.2,0,1);
                o.Emission += rim * o.Albedo.rgb * cubeAmbient;
                #endif
            #endif
            
		}
		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "MI_Illustrative"
}
