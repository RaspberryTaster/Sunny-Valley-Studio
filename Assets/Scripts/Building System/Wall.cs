using Assets.Scripts.Building_System;
using Assets.Scripts.Building_System.Test;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Flags]
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
    public WallOrientation wallOrientation;
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
        //flood fill and find the rooms
        //FloodFillRoom.
    }
    private void OnEnable()
    {
        
    }

    public void Set()
    {

        if(isDiagonal)
        {
            //get node at wallboject position.
            SetDiagonalOrientation(FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position));
        }
        else
        {
            InformNeighbour(FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + wallObject.transform.forward / 2));
            InformNeighbour(FloodFillRoom.Instance.GetNodeAtPos(wallObject.transform.position + -wallObject.transform.forward / 2));
        }
    }

    //get the nose that this above.
    //set the orientation
    public void SetDiagonalOrientation(Node n)
    {
        Vector3 rel = transform.position - n.position;
        
        if (rel == new Vector3(-0.5f, 0, -0.5f) || rel == new Vector3(0.5f, 0, 0.5f))
        {
            wallOrientation = WallOrientation.DIAGONAL_ALPHA;
            n.wallDirections |= WallDirection.Diagonal_Alpha;
        }
        else if (rel == new Vector3(0.5f, 0, -0.5f) || rel == new Vector3(-0.5f, 0, 0.5f))
        {
            wallOrientation = WallOrientation.DIGONAL_BETA;
            n.wallDirections |= WallDirection.Diagonal_Beta;
        }

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

            // no bro instead tell them they have both directions.
            
        }
        else
        {
            Vector3 direction = wallObject.transform.position - new Vector3(n.position.x,wallObject.transform.position.y,n.position.z);
            Debug.Log(direction);
            direction.Normalize();

            float angle = Vector3.Angle(Vector3.forward, direction); // Angle between the forward vector and the direction vector

            if (angle < 45.0f)
            {
                if ((n.wallDirections & WallDirection.North) == 0)
                {
                    n.wallDirections |= WallDirection.North;
                    Debug.Log(n.position + " is to the north");
                }
            }
            else if (angle < 135.0f)
            {
                if (direction.x > 0)
                {
                    if ((n.wallDirections & WallDirection.East) == 0)
                    {
                        n.wallDirections |= WallDirection.East;
                        Debug.Log(n.position + " is to the east");
                    }
                }
                else
                {
                    if ((n.wallDirections & WallDirection.West) == 0)
                    {
                        n.wallDirections |= WallDirection.West;
                        Debug.Log(n.position + " is to the west");
                    }
                }
            }
            else
            {
                if ((n.wallDirections & WallDirection.South) == 0)
                {
                    n.wallDirections |= WallDirection.South;
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
        Gizmos.DrawSphere(wallObject.transform.position + wallObject.transform.forward, .2f);
        Gizmos.DrawSphere(wallObject.transform.position + -wallObject.transform.forward, .2f);
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(wallObject.transform.position + -wallObject.transform.right/2, .2f);
        //Gizmos.DrawSphere(wallObject.transform.position + wallObject.transform.right/2, .2f);
    }
}
