using UnityEngine;
using System.Collections;

using MPInput;

public class Game : MonoBehaviour
{
    public GameObject BuilderPrefab;

	void Start ()
    {
        bool[] connectedControllers = MP_Input.GetConnectedControllers();

        for(int i = 0; i < connectedControllers.Length; i++)
        {
            if(connectedControllers[i])
            {
                MP_InputDeviceInfo device = new MP_InputDeviceInfo(MP_eInputType.Controller, i);

                GameObject newBuilder = GameObject.Instantiate(BuilderPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                BuilderPawn newBuilderPawn
                if (newBuilder.GetComponent<BuilderPawn>() != null)
                {
                    newBuilder.GetComponent<BuilderPawn>().CreatePawn();
                }
            }
        }
	}
}
