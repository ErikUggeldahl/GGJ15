using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour
{

    public void StartSelfDestruct(float time)
    {
        StartCoroutine(SelfDestructPriv(time));
    }

    private IEnumerator SelfDestructPriv(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
