Shader "Custom/Ground" 
{
	Properties 
	{
		_DirtTex ("Dirt (RGB)", 2D) = "grey" {}
		_DirtTile ("Dirt Tile", float) = 1

		_GrassTex ("Grass (RGB)", 2D) = "grey" {}
		_GrassTile ("Grass Tile", float) = 1

		_BlendModulate ("Grass (RGB)", 2D) = "grey" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		float _ClearingRadius;

		sampler2D _DirtTex;
		float _DirtTile;

		sampler2D _GrassTex;
		float _GrassTile;

		sampler2D _BlendModulate;

		struct Input 
		{
			float3 worldPos;
			float2 uv_DirtTex;
			float2 uv_GrassTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 dirt = tex2D (_DirtTex, IN.worldPos.xz * _DirtTile);
			half4 grass = tex2D (_GrassTex, IN.worldPos.xz * _GrassTile);
			half4 blendModulate = tex2D (_BlendModulate, IN.worldPos.xz * _GrassTile);

			float distanceFromCenter = length(IN.worldPos);
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
