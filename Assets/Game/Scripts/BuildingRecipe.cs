using UnityEngine;
using System.Collections;

public class BuildingRecipe : MonoBehaviour {
	
	public struct Recipe
	{
		public int wood;
		public int rock;

		public Recipe(int _wood, int _rock)
		{
			wood = _wood;
			rock = _rock;
		}
	}
	
	public static Recipe wall = new Recipe(1,0);
	public static Recipe turret = new Recipe(3,2);

}
