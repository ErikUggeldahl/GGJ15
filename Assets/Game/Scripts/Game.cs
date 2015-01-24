using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public GameObject BuilderPrefab;
    public GameObject ArchitectPrefab;

    private List<BuilderPawn> BuilderPawns = new List<BuilderPawn>();
    private ArchitectPawn ArchitectPawn = null;

    public GameObject architectSpawnLocation;
    public GameObject[] builderSpawnLocations;

    void Awake()
    {
        Instance = this;
    }

	void Start ()
    {
        GameObject newArchitect = GameObject.Instantiate(ArchitectPrefab, architectSpawnLocation.transform.position, Quaternion.identity) as GameObject;
        ArchitectPawn = newArchitect.GetComponent<ArchitectPawn>();

        // Connected controllers
        bool[] connectedControllers = MP_Input.GetConnectedControllers();

        // Create builders
        for(int i = 0; i < connectedControllers.Length; i++)
        {
            if(connectedControllers[i])
            {
                MP_InputDeviceInfo device = new MP_InputDeviceInfo(MP_eInputType.Controller, i);

                GameObject newBuilder = GameObject.Instantiate(BuilderPrefab, builderSpawnLocations[i].transform.position, Quaternion.identity) as GameObject;

                BuilderPawn newBuilderPawn = newBuilder.GetComponent<BuilderPawn>();
                
                if (newBuilderPawn != null)
                {
                    newBuilderPawn.Initialize(device);
                    BuilderPawns.Add(newBuilderPawn);
                }
            }
        }
	}
}