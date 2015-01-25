using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetPoller : MonoBehaviour
{
	private const string URL = "http://www.blackcarbongames.com/cgi-bin/vote.py";
	private const int POLL_INTERVAL = 5;

	private static NetPoller instance;
	public static NetPoller GetInstance()
	{
		return instance;
	}

	[SerializeField]
	Text resultText;

	[SerializeField]
	Animation textAnimator;

	[SerializeField]
	AnimationClip textAnimation;

	public delegate void PolledResultHandler(int good, int bad);
	public event PolledResultHandler Polled;

	private int totalGoodVotes = 0;
	private int totalBadVotes = 0;

	private int currentGoodVotes = 0;
	private int currentBadVotes = 0;

	void Awake()
	{
		instance = this;
	}

	IEnumerator Start()
	{
		yield return StartCoroutine(ExecuteRequest());
		totalGoodVotes = currentGoodVotes;
		totalBadVotes = currentBadVotes;

		StartCoroutine(Poll());
	}

	IEnumerator Poll()
	{
		while (true)
		{
			yield return StartCoroutine(ExecuteRequest());

			int goodDelta = currentGoodVotes - totalGoodVotes;
			int badDelta = currentBadVotes - totalBadVotes;

			int goodDifference = Mathf.Clamp(goodDelta - badDelta, 0, int.MaxValue);
			int badDifference = Mathf.Clamp(badDelta - goodDelta, 0, int.MaxValue);

			//Debug.Log("Delta: " + goodDelta + " : " + badDelta + "\n" + "Difference: " + goodDifference + " : " + badDifference);

			if (goodDifference > 0 || badDifference > 0)
				Polled(goodDifference, badDifference);

			ShowResults(goodDifference, badDifference);

			totalGoodVotes = currentGoodVotes;
			totalBadVotes = currentBadVotes;

			yield return new WaitForSeconds(POLL_INTERVAL);
		}
	}

	void ShowResults(int good, int bad)
	{
		string winner;
		int votes;
		if (good > bad)
		{
			winner = "Trees";
			votes = good;
		}
		else if (bad > good)
		{
			winner = "Bears";
			votes = bad;
		}
		else
		{
			resultText.text = "";
			return;
		}

		Debug.Log("Playing animation");

		resultText.text = winner + " Win!\n" + votes + " more votes!";
		textAnimator.Play();
	}

	IEnumerator ExecuteRequest()
	{
		var request = new WWW (URL);
		yield return request;
		try
		{
			if (request.isDone)
			{

				var result = request.text;
				char[] splitStrs = { ' ' };
				var resultSplit = result.Split(splitStrs);

				currentGoodVotes = int.Parse(resultSplit[1]);
				currentBadVotes = int.Parse(resultSplit[3]);
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("Net error:" + e.Message);
		}
	}
}
