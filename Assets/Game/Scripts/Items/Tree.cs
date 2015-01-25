using UnityEngine;
using System.Collections;

public class Tree : HarvestableResource 
{
    public Color MaxVariance;

    protected override void Start()
    {
        base.Start();

        Color color = renderer.material.GetColor("_DiffuseColor");

        Color modColor = MaxVariance * new Color(Random.Range(-0.25f, 1.0f), Random.Range(-0.25f, 1.0f), Random.Range(-0.25f, 1.0f));

        Color newColor = color + modColor;

        renderer.material.SetColor("_DiffuseColor", newColor);
    }
}