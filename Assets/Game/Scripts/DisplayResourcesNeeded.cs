using UnityEngine;
using System.Collections;

public enum ConstructiveType
{
	wall = 0,
	turret
}

public class DisplayResourcesNeeded : MonoBehaviour {

	[SerializeField]
	Color woodColor;

	[SerializeField]
	Color stoneMat;

	[SerializeField]
	GameObject UIPrefab;

	[SerializeField]
	ConstructiveType constructionType;



	// Use this for initialization
	void Start () 
	{
		switch (constructionType) 
		{
		case ConstructiveType.wall:
			DisplayResources(BuildingRecipe.wall);
			break;
		case ConstructiveType.turret:
			DisplayResources(BuildingRecipe.turret);
			break;
		default:
			break;
		}
	}

	void DisplayResources(BuildingRecipe.Recipe recipe)
	{
		if(recipe.wood != 0) 
			CreateUICircle (woodColor, recipe.wood);
		if(recipe.rock != 0)
			CreateUICircle (stoneMat, recipe.rock);

	}

	void CreateUICircle(Color color, int count)
	{
		GameObject ui = GameObject.Instantiate(UIPrefab) as GameObject;
		ui.transform.parent = transform;
		ui.GetComponent<CanvasRenderer> ().SetColor(color);
		ui.GetComponentsInChildren<TextMesh>()[0].text = count.ToString();
		ui.GetComponentsInChildren<TextMesh>()[1].text = count.ToString();
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
