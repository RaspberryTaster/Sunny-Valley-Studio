using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectGrid : MonoBehaviour
{
    public PlaceableObject placeableObject;
    public TilemapRenderer rnd;

    public void ApplyColor(int index)
    {
        if (rnd == null) return;
        rnd.material.SetInt("_Setting", index);
    }

    private void Update()
    {
        if(placeableObject== null) return;
        ApplyColor(placeableObject.CanBePlaced ? 1 : 0);
    }

    private void OnEnable()
    {
        Material mat = new Material(rnd.sharedMaterial);
        rnd.material = mat;
    }
}
