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
		#pragma surface surf Lambert

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

		void surf (Input IN, inout SurfaceOutput o) 
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
