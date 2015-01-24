//http://forum.unity3d.com/threads/151466-World-position-from-depth

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AmbientSettings : MonoBehaviour 
{
	public Texture2D DiffuseRamp = null;
	public Texture2D SpecRamp = null;
	public Cubemap IBLCube = null;
	public float AmbientIntensity = 1.0f;

	public void OnEnable()
	{
		UpdateAmbientSettings ();
	}
	
	public void UpdateAmbientSettings()
	{
		if (DiffuseRamp)
			Shader.SetGlobalTexture ("_DiffuseWarp", DiffuseRamp);
		if (SpecRamp)
			Shader.SetGlobalTexture ("_SpecWarp ", SpecRamp);
		if (IBLCube)
			Shader.SetGlobalTexture ("_IBLDiffuseCube", IBLCube);

		Shader.SetGlobalFloat ("_AmbientIntensity", AmbientIntensity);
	}
}
