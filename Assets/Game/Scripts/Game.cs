﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MPInput;

public class Game : MonoBehaviour
{
    public GameObject BuilderPrefab;
    public GameObject ArchitectPrefab;

    private List<BuilderPawn> BuilderPawns = new List<BuilderPawn>();
    private ArchitectPawn ArchitectPawn = null;

	void Start ()
    {
        bool[] connectedControllers = MP_Input.GetConnectedControllers();

        for(int i = 0; i < connectedControllers.Length; i++)
        {
            if(connectedControllers[i])
            {
                MP_InputDeviceInfo device = new MP_InputDeviceInfo(MP_eInputType.Controller, i);

                GameObject newBuilder = GameObject.Instantiate(BuilderPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                BuilderPawn newBuilderPawn = newBuilder.GetComponent<BuilderPawn>();
                
                if (newBuilderPawn != null)
                {
                    newBuilderPawn.CreatePawn(device);
                    BuilderPawns.Add(newBuilderPawn);
                }
            }
        }
	}
}
