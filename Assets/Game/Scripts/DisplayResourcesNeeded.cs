using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class DisplayResourcesNeeded : MonoBehaviour {

	[SerializeField]
	Color woodColor;

	[SerializeField]
	Color stoneMat;

	[SerializeField]
	GameObject UIPrefab;

	GameObject woodUI;
	GameObject stoneUI;


	public Transform[] anchors;

	Building building;

	// Use this for initialization
	void Start () 
	{
		building = GetComponentInParent<Building>();
		building.onResourceReceived += RefreshUI;
		DisplayResources();
	}

	void DisplayResources()
	{
		if (building.WoodRequired - building.CurrentWood != 0) 
			CreateUICircle (woodColor, building.WoodRequired - building.CurrentWood, "Wood");
		else
			Destroy(woodUI);
		if (building.StoneRequired - building.CurrentStone != 0)
			CreateUICircle (stoneMat, building.StoneRequired - building.CurrentStone, "Stone");
		else
			Destroy(stoneUI);
	}

	void RefreshUI()
	{
		if (woodUI != null) 
		{
			int deltaWood = building.WoodRequired - building.CurrentWood;
			woodUI.transform.GetChild(0).GetComponent<Text>().text = deltaWood.ToString();
			woodUI.transform.GetChild(1).GetComponent<Text>().text = deltaWood.ToString();	

			if (deltaWood == 0)
				Destroy(woodUI);
		}
		if (stoneUI != null)
		{
			int deltaStone = building.StoneRequired - building.CurrentStone;
			stoneUI.transform.GetChild(0).GetComponent<Text>().text = deltaStone.ToString();
			stoneUI.transform.GetChild(1).GetComponent<Text>().text = deltaStone.ToString();	

			if (deltaStone == 0)
				Destroy(stoneUI);
		}
		ArrangeDisplays();
	}

	void CreateUICircle(Color color, int count, string resourceName)
	{
		GameObject ui = GameObject.Instantiate(UIPrefab) as GameObject;
		ui.transform.SetParent(transform);
		ui.transform.position = transform.position;
		ui.tag = resourceName;


		if (resourceName == "Wood")
			woodUI = ui;
		else if (resourceName == "Stone")
			stoneUI = ui;


		ui.GetComponent<RawImage>().color = color;
		ui.transform.GetChild(0).GetComponent<Text>().text = count.ToString();
		ui.transform.GetChild(1).GetComponent<Text>().text = count.ToString();
		ArrangeDisplays();
	}

	void ArrangeDisplays()
	{
		int count = 0;

		if (woodUI != null) 
		{
			woodUI.transform.position = anchors[count].position;
			count++;
		}
		if (stoneUI != null) 
		{
			stoneUI.transform.position = anchors[count].position;
			count++;
		}

	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
