using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KillCounterInfo : MonoBehaviour
{
	private Text textObject = null;
	public int PlayerIndex;

	void Awake()
	{
		textObject = this.gameObject.GetComponent<Text>();
	}

	void Update()
	{
		if(PlayerIndex == 0)
		{
			if(Game.Instance.BuilderPawns.Count > 0)
				textObject.text = "Blue Player Kills: " + Game.Instance.BuilderPawns[PlayerIndex].KillCount.ToString();
			else
				textObject.text = "";
		}
		else if(PlayerIndex == 1)
		{
			if(Game.Instance.BuilderPawns.Count > 1)
				textObject.text = "Red Player Kills: " + Game.Instance.BuilderPawns[PlayerIndex].KillCount.ToString();
			else
				textObject.text = "";
		}
		else if(PlayerIndex == 2)
		{
			if(Game.Instance.BuilderPawns.Count > 2)
				textObject.text = "Green Player Kills: " + Game.Instance.BuilderPawns[PlayerIndex].KillCount.ToString();
			else
				textObject.text = "";
		}
		else if(PlayerIndex == 3)
		{
			if(Game.Instance.BuilderPawns.Count > 3)
				textObject.text = "Yellow Player Kills: " + Game.Instance.BuilderPawns[PlayerIndex].KillCount.ToString();
			else
				textObject.text = "";
		}
	}
}
