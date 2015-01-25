using UnityEngine;
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

			if (goodDelta > 0 || badDelta > 0)
				Polled(goodDelta, badDelta);

			totalGoodVotes = currentGoodVotes;
			totalBadVotes = currentBadVotes;

			yield return new WaitForSeconds(POLL_INTERVAL);
		}
	}

	IEnumerator ExecuteRequest()
	{
		var request = new WWW (URL);
		yield return request;
		
		var result = request.text;
		char[] splitStrs = {' '};
		var resultSplit = result.Split(splitStrs);
		
		currentGoodVotes = int.Parse(resultSplit[1]);
		currentBadVotes = int.Parse(resultSplit[3]);
	}
}
