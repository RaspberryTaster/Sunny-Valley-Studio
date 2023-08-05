using Assets.Scripts.Building_System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.FlagsAttribute]
public enum CeilingDirection
{
    None = 0,
    Front = 1,
    Back = 2
}

public class Wall : MonoBehaviour, ICollidable
{
    public bool isDiagonal;
    public CeilingDirection ceilingDirection;
    public Transform endPoint;
    public GameObject FrontCeiling;
    public GameObject BackCeiling;
    public GameObject wallObject;
    [SerializeField] private float distance = 0.5f; // Distance of the sphere gizmo from the object

    public List<Wall> connectedWalls;
    private void OnValidate()
    {
        // Check if the FrontCeiling and BackCeiling game objects are assigned
        if (FrontCeiling == null || BackCeiling == null)
            return;

        // Check the value of the ceilingDirection and enable/disable the ceilings accordingly
        if ((ceilingDirection & CeilingDirection.Front) != 0)
        {
            FrontCeiling.SetActive(true);
        }
        else
        {
            FrontCeiling.SetActive(false);
        }

        if ((ceilingDirection & CeilingDirection.Back) != 0)
        {
            BackCeiling.SetActive(true);
        }
        else
        {
            BackCeiling.SetActive(false);
        }
    }

    private void OnEnable()
    {
        
    }


    public void InformNode()
    {
        if(isDiagonal)
        {
            //setnode tell node at your position that it has a diagonal wall on it
        }
        else
        {
            //tell front and back nodes that they have wall on them.
        }
    }
    private void OnDestroy()
    {
        FloodFillRoom.Instance.RemoveWall(this);
    }

    public void SetPos()
    {
        FloodFillRoom.Instance.AddWall(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<Wall>(out var w) || connectedWalls.Contains(w)) return;
        connectedWalls = connectedWalls.Where(element => element != null).ToList();
        connectedWalls.Add(w);
        //sort by distance
        connectedWalls = connectedWalls.OrderByDescending(wall => Vector3.Distance(this.transform.position, wall.wallObject.transform.position)).ToList();

    }

    private void OnTriggerExit(Collider other)
    {
        Wall w = other.GetComponent<Wall>();
        if (w == null || !connectedWalls.Contains(w)) return;
        connectedWalls = connectedWalls.Where(element => element != null).ToList();
        connectedWalls.Remove(w);
        //sort by distance
        connectedWalls = connectedWalls.OrderByDescending(wall => Vector3.Distance(this.transform.position, wall.wallObject.transform.position)).ToList();
    }

}
