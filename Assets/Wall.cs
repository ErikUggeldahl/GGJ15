using UnityEngine;
using System.Collections;

public class Wall : Building
{
    public Transform mesh;

    public void Start()
    {
        int rotation = Random.Range(0,4);

        mesh.Rotate(new Vector3(0, rotation * 90, 0));
    }
}
