using UnityEngine;
using System.Collections;

public class StunAbility : MonoBehaviour
{
    private Camera sceneCamera;

    private int aiLayer;

    private bool isStunning = true;

    [SerializeField]
    GameObject IceCubeObj;

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

        CreateIceCube(hitInfo.transform.position);
    }

    void CreateIceCube(Vector3 location)
    {
        var randomRotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up);
        
        var iceCubeGO = ((GameObject)Instantiate(IceCubeObj, location, randomRotation));
        var groundSnap = new Vector3(iceCubeGO.transform.position.x, 0f, iceCubeGO.transform.position.z);
        iceCubeGO.transform.position = groundSnap;

        var selfDestruct = iceCubeGO.GetComponent<SelfDestruct>();
        selfDestruct.StartSelfDestruct(Duration);
    }
}
