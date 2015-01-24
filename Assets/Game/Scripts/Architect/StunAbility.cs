using UnityEngine;
using System.Collections;

public class StunAbility : MonoBehaviour
{
    private Camera sceneCamera;

    private int aiLayer;

    private bool isStunning = true;

	[SerializeField]
	float Duration;

	[SerializeField]
	float SlowFactor;

    void Start()
    {
        sceneCamera = Camera.main;

        aiLayer = LayerMask.NameToLayer("AI");
    }

    public void EnableStunning()
    {
        isStunning = true;
    }

    public void EnableStunningDelayed()
    {
        StartCoroutine(EnableStunningDelayedPriv());
    }

    private IEnumerator EnableStunningDelayedPriv()
    {
        yield return null;
        EnableStunning();
    }

    public void DisableStunning()
    {
        isStunning = false;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || !isStunning)
            return;

        RaycastHit hitInfo;
        var mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, 1 << aiLayer);

        if (!isHit)
            return;

        var aiMovement = hitInfo.transform.GetComponent<AIMovement>();
        if (aiMovement == null)
            return;

		aiMovement.StartStun(Duration, SlowFactor);
    }
}
