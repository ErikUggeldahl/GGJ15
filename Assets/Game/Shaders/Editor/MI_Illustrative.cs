﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MI_Illustrative : CustomMaterialEditor
{  
	protected override void CreateToggleList()
	{     
		Toggles.Add(new FeatureToggle("Vertex Blend","blend","VERTEXBLEND_ON", "VERTEXBLEND_OFF",true));
		Toggles.Add(new FeatureToggle("Normal Enabled","normal","NORMALMAP_ON","NORMALMAP_OFF"));
		Toggles.Add(new FeatureToggle("Specular Enabled","specular","SPECULAR_ON","SPECULAR_OFF"));
		Toggles.Add(new FeatureToggle("Fresnel Enabled","fresnel","FRESNEL_ON","FRESNEL_OFF"));
		Toggles.Add(new FeatureToggle("Rim Light Enabled","rim","RIMLIGHT_ON","RIMLIGHT_OFF"));
	}
}
