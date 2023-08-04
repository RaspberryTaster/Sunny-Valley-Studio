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
    public CeilingDirection ceilingDirection;
    public GameObject wallNeighbourTransform;

    public GameObject FrontCeiling;
    public GameObject BackCeiling;
    public Transform farPoint;
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



    private void OnDrawGizmos()
    {

        if (wallNeighbourTransform == null) return;
        // Calculate the position of the gizmo using transform.forward and distance
        Vector3 gizmoPosition = wallNeighbourTransform.transform.position + wallNeighbourTransform.transform.forward * distance;

        // Draw a sphere gizmo at the calculated position
        Gizmos.color = Color.red; // You can change the color if you want
        Gizmos.DrawSphere(gizmoPosition, 0.1f); // Adjust the size of the sphere if needed

        gizmoPosition = wallNeighbourTransform.transform.position +  -wallNeighbourTransform.transform.forward * distance;

        Gizmos.color = Color.blue; // You can change the color if you want
        Gizmos.DrawSphere(gizmoPosition, 0.1f); // Adjust the size of the sphere if needed
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
