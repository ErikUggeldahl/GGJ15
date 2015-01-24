using UnityEngine;
using System.Collections;

public class StunAbility : MonoBehaviour
{
    private Camera sceneCamera;

    private int aiLayer;

    private bool isStunning = true;

	[SerializeField]
	float stunDuration;

	[SerializeField]
	float slowdownPourcentage;

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
        if (!Input.GetMouseButton(0) || !isStunning)
            return;

        RaycastHit hitInfo;
        var mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        var isHit = Physics.Raycast(mouseRay, out hitInfo, float.PositiveInfinity, 1 << aiLayer);

        if (!isHit)
            return;

        var aiMovement = hitInfo.transform.GetComponent<AIMovement>();
        if (aiMovement == null)
            return;

        // Replace this with the stun function when it becomes available
		aiMovement.Stun(2f, 0.1f);
    }
}
