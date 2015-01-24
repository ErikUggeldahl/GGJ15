using UnityEngine;
using System.Collections;

public class TestNetPoller : MonoBehaviour
{
	void Start()
	{
		NetPoller.GetInstance().Polled += PrintResults;
	}

	void PrintResults(int good, int bad)
	{
		Debug.Log("Result: " + good + " : " + bad);
	}
}
