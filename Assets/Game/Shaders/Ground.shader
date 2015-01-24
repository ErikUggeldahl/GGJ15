Shader "Custom/Ground" 
{
	Properties 
	{
		_DirtTex ("Dirt (RGB)", 2D) = "grey" {}
		_DirtTile ("Dirt Tile", float) = 1

		_GrassTex ("Grass (RGB)", 2D) = "grey" {}
		_GrassTile ("Grass Tile", float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		sampler2D _DirtTex;
		float _DirtTile;

		sampler2D _GrassTex;
		float _GrassTile;

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

			o.Albedo = grass.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
