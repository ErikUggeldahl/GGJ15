using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public enum GameState
    {
        PreGame,
        Game,
        Win,
        GameOver
    }

    public GameState CurrentGameState = GameState.PreGame;

    public GameObject BuilderPrefab;
    public GameObject ArchitectPrefab;

    public List<BuilderPawn> BuilderPawns = new List<BuilderPawn>();
    public ArchitectPawn ArchitectPawn = null;

    public GameObject architectSpawnLocation;
    public GameObject[] builderSpawnLocations;

	public GameObject Menu;

    void Awake()
    {
        Instance = this;
    }

	void Start ()
    {
		// Hide menu
		Menu.SetActive(false);

        // ** Create architect player ** //
        GameObject newArchitect = GameObject.Instantiate(ArchitectPrefab, architectSpawnLocation.transform.position, Quaternion.identity) as GameObject;
        ArchitectPawn = newArchitect.GetComponent<ArchitectPawn>();

        // ** Create builder players ** //
        bool[] connectedControllers = MP_Input.GetConnectedControllers();

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

	void LateUpdate()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Exit();
	}

	public void GameOver()
	{
		CurrentGameState = GameState.GameOver;
		Menu.SetActive(true);
	}

	public void Retry()
	{
		Application.LoadLevel(0);
	}

	public void Exit()
	{
		Application.Quit();
	}
}