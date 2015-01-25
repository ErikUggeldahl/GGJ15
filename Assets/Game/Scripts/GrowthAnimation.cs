using UnityEngine;
using System.Collections;

public class GrowthAnimation : MonoBehaviour
{
	public void StartGrowth(float duration)
	{
		if (duration == 0)
		{
			Debug.LogWarning("Cannot grow with time 0. Causes division by 0.");
			return;
		}

		StartCoroutine(Grow(duration));
	}

	IEnumerator Grow(float duration)
	{
		transform.localScale = Vector3.zero;
		renderer.enabled = false;

		var lerpParam = 0f;

		while (lerpParam < 1f)
		{
			lerpParam += Time.deltaTime / duration;
			Mathf.Clamp01(lerpParam);

			transform.localScale = Vector3.one * lerpParam;
			yield return null;
			renderer.enabled = true;
		}

		Destroy(this);
	}
}
