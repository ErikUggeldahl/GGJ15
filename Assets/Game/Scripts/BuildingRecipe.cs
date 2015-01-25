using UnityEngine;
using System.Collections;

public class BuildingRecipe : MonoBehaviour {
	
	public struct Recipe
	{
		public int wood;
		public int rock;
	}

	public static Recipe wall = new Recipe();
	public static Recipe turret = new Recipe();

	void Start()
	{
		wall.wood = 1;
		wall.rock = 0;

		turret.wood = 3;
		turret.rock = 2;
	}

}
