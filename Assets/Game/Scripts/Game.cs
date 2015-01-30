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
		Pause,
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

	public GameObject menu;
	public GameObject resume;

	public GameObject noControllerAlert;

    void Awake()
    {
        Instance = this;
    }

	void Start ()
    {
		// Hide menu
		menu.SetActive(false);

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
					newBuilder.GetComponent<BuilderHealth>().onPlayerDeath += CheckIfAllPlayerDead;
                    newBuilderPawn.Initialize(device);
                    BuilderPawns.Add(newBuilderPawn);
                }
            }
        }
	}

    void Update()
    {
		if(Input.GetJoystickNames().Length == 0)
			noControllerAlert.SetActive(true);
		else
			noControllerAlert.SetActive(false);
    }

	void CheckIfAllPlayerDead()
	{
		bool areAllPlayersDead = true;
		
		for (int i = 0; i < BuilderPawns.Count; i++)
		{
			if(BuilderPawns[i].BuilderHealthScript.IsAlive)
				areAllPlayersDead = false;
		}
		
		if(areAllPlayersDead)
			Game.Instance.GameOver();
	}

	void LateUpdate()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && CurrentGameState != GameState.GameOver)
		{
			if (CurrentGameState == GameState.Pause)
				Resume();
			else
				Pause();
		}
			
	}

	void Pause()
	{
		CurrentGameState = GameState.Pause;
		Time.timeScale = 0.0f;
		menu.SetActive(true);
	}

	public void Resume()
	{
		CurrentGameState = GameState.Game;
		Time.timeScale = 1.0f;
		menu.SetActive(false);
	}

	public void GameOver()
	{
		Time.timeScale = 1.0f;
		CurrentGameState = GameState.GameOver;
		menu.SetActive(true);
		resume.SetActive(false);
	}

	public void Retry()
	{
		Time.timeScale = 1.0f;
		Application.LoadLevel(0);
	}

	public void Exit()
	{
		Application.Quit();
	}
}