using Assets.Scripts.Building_System;
using Assets.Scripts.Building_System.Test;
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


    private void Start()
    {
        Set();
    }
    private void OnEnable()
    {
        
    }

    public void Set()
    {
        InformNeighbour(FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + wallObject.transform.forward/2));
        InformNeighbour(FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + -wallObject.transform.forward/2));
    }


    public void Unset()
    {
        FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + wallObject.transform.forward / 2);
        FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + -wallObject.transform.forward / 2);
    }
    public void InformNeighbour(Node n)
    {
        if(isDiagonal)
        {
            //setnode tell node at your position that it has a diagonal wall on it
        }
        else
        {
            Vector3 direction = wallObject.transform.position - new Vector3(n.position.x,wallObject.transform.position.y,n.position.z);
            Debug.Log(direction);
            direction.Normalize();

            float angle = Vector3.Angle(Vector3.forward, direction); // Angle between the forward vector and the direction vector

            if (angle < 45.0f)
            {
                if ((n.wallDirections & HorizontalWallDirection.North) == 0)
                {
                    n.wallDirections |= HorizontalWallDirection.North;
                    Debug.Log(n.position + " is to the north");
                }
            }
            else if (angle < 135.0f)
            {
                if (direction.x > 0)
                {
                    if ((n.wallDirections & HorizontalWallDirection.East) == 0)
                    {
                        n.wallDirections |= HorizontalWallDirection.East;
                        Debug.Log(n.position + " is to the east");
                    }
                }
                else
                {
                    if ((n.wallDirections & HorizontalWallDirection.West) == 0)
                    {
                        n.wallDirections |= HorizontalWallDirection.West;
                        Debug.Log(n.position + " is to the west");
                    }
                }
            }
            else
            {
                if ((n.wallDirections & HorizontalWallDirection.South) == 0)
                {
                    n.wallDirections |= HorizontalWallDirection.South;
                    Debug.Log(n.position + " is to the south");
                }
            }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(wallObject.transform.position + wallObject.transform.forward/2, .2f);
        Gizmos.DrawSphere(wallObject.transform.position + -wallObject.transform.forward/2, .2f);
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(wallObject.transform.position + -wallObject.transform.right/2, .2f);
        //Gizmos.DrawSphere(wallObject.transform.position + wallObject.transform.right/2, .2f);
    }
}
