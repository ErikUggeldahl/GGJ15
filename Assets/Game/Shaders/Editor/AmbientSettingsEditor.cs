using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(AmbientSettings))]
public class AmbientSettingsEditor : Editor 
{	
	public override void OnInspectorGUI()		
	{		
		GUI.changed = false;
		
		AmbientSettings CI = target as AmbientSettings;

		CI.DiffuseRamp = EditorGUILayout.ObjectField ("Diffuse Ramp", CI.DiffuseRamp,typeof(Texture2D), false) as Texture2D;
		CI.SpecRamp = EditorGUILayout.ObjectField ("Spec Ramp", CI.SpecRamp,typeof(Texture2D), false) as Texture2D;
		CI.IBLCube = EditorGUILayout.ObjectField ("IBL Ambient Cube", CI.IBLCube, typeof(Cubemap), false) as Cubemap;
		CI.AmbientIntensity = EditorGUILayout.FloatField ("Ambient Intensity", CI.AmbientIntensity);

		CI.UpdateAmbientSettings();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(CI);
		}
		
	}
	
}
