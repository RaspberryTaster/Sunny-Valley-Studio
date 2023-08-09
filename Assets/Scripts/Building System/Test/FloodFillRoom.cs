using Assets.Scripts.Building_System.Test;
using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public struct PosDir
{
    public Vector3 pos;
    public Vector3 dir;

    public PosDir(Vector3 pos, Vector3 dir)
    {
        this.pos = pos;
        this.dir = dir;
    }
}

public class FloodFillRoom : Singleton<FloodFillRoom>
{
    public Tilemap wallPlacementGrid;
    public Tilemap groundGrid;
    public Renderer boundsPlane;
    public GameObject floodPrefab;
    public Node nodePrefab;
    private Camera mainCamera;
    public List<Vector3> wallPositions;
    private Dictionary<Vector3, List<Wall>> placedWall = new Dictionary<Vector3, List<Wall>>();
    private Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 placementPosition = PlacementUtils.WorldPositionToGridPosition(hit.point, groundGrid);

                // Check if placement position is within bounds
                if (IsWithinBoundsXZ(placementPosition))
                {
                    StartFloodFill(placementPosition);
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 placementPosition = Position(hit.point);

                // Check if placement position is within bounds
                if (IsWithinBoundsXZ(placementPosition))
                {
                    StartFloodFill(placementPosition);
                }
            }

        }
    }


    public Node GetNodeAtPos(Vector3 position)
    {
        //if

        var p = PlacementUtils.WorldPositionToGridPosition(position, groundGrid);
        if (nodes.ContainsKey(p))
        {
            return nodes[p];
        }
        else
        {

            var n = Instantiate(nodePrefab);
            n.SetPos(p);
            nodes.Add(p, n);
            return n;
        }

    }
    public void AddWall(Wall w)
    {
        Vector3 key = Position(w.transform.position);
        Vector3 key1 = Position(w.endPoint.position);
        wallPositions.Add(key);
        wallPositions.Add(key1);

        // Add the wall to the placedWall dictionary
        if (!placedWall.ContainsKey(key))
        {
            placedWall[key] = new List<Wall>();
        }
        placedWall[key].Add(w);

        if (!placedWall.ContainsKey(key1))
        {
            placedWall[key1] = new List<Wall>();
        }
        placedWall[key1].Add(w);
    }

    public void RemoveWall(Wall w)
    {
        Vector3 key = Position(w.transform.position);
        Vector3 key1 = Position(w.endPoint.position);
        wallPositions.Remove(key);
        wallPositions.Remove(key1);

        // Remove the wall from the placedWall dictionary
        if (placedWall.ContainsKey(key))
        {
            placedWall[key].Remove(w);
        }

        if (placedWall.ContainsKey(key1))
        {
            placedWall[key1].Remove(w);
        }
    }

    public Vector3 Position(Vector3 position)
    {
        Vector3Int gridPosition = wallPlacementGrid.WorldToCell(position);
        return wallPlacementGrid.GetCellCenterWorld(gridPosition);
    }

    private bool IsWithinBoundsXZ(Vector3 position)
    {
        Bounds bounds = boundsPlane.bounds;

        bool withinXBounds = position.x >= bounds.min.x && position.x <= bounds.max.x;
        bool withinZBounds = position.z >= bounds.min.z && position.z <= bounds.max.z;

        return withinXBounds && withinZBounds;
    }

    private void StartFloodFill(Vector3 position)
    {


        FloodFill(position);
        //StartFloodFill(position);
    }


    public void CreateRoom(Vector3 position)
    {

        GenerateRoom(FloodFill(position));
    }
    private void GenerateRoom(List<Vector3> filledPositions)
    {
        List<Vector3> nodePositions = new List<Vector3>();
        List<Wall> encounteredWalls = new List<Wall>();

        foreach (var position in filledPositions)
        {
            Node node = GetNodeAtPos(position);
            if (node != null)
            {
                nodePositions.Add(position);
            }

            if (placedWall.ContainsKey(position))
            {
                foreach (var wall in placedWall[position])
                {
                    if (!encounteredWalls.Contains(wall))
                    {
                        encounteredWalls.Add(wall);
                    }
                }
            }
        }

        Room newRoom = new Room(nodePositions, encounteredWalls);
        // Now you have the room data in the newRoom object, you can do further processing or use it as needed.
    }


    private List<Vector3> FloodFill(Vector3 initialPosition)
    {
        HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
        Queue<PosDir> positionsToCheck = new Queue<PosDir>();
        List<Vector3> filledPositions = new List<Vector3>(); // Store filled positions

        positionsToCheck.Enqueue(new PosDir(initialPosition, Vector3.zero));
        


        while (positionsToCheck.Count > 0) // Continue until all positions are processed
        {
            
            PosDir p = positionsToCheck.Dequeue();
            Vector3 position = p.pos;
            Node curNode = Instance.GetNodeAtPos(position);
            if (!IsWithinBoundsXZ(position) || visitedPositions.Contains(position))
            {
                continue;
            }

            visitedPositions.Add(position);

            if (filledPositions.Contains(position))
            {
                continue;
            }
            else if (placedWall.Keys.Contains(position))
            {
                // Handle walls
                continue;
            }
            else
            {
                Instantiate(floodPrefab, PlacementUtils.WorldPositionToGridPosition(position, groundGrid), Quaternion.identity);
                filledPositions.Add(position); // Add filled position to the list
            }

            var wallDirections = curNode.wallDirections;

            if((curNode.wallDirections & WallDirection.Diagonal_Alpha) != 0)
            {
                //bool approachingTop = p.dir == Vector3.left || p.dir == Vector3.forward;
                //thjat means you only option is to go 
                //that means you cant go any further down or right
                //can only go the opposite  of for or left

                if(p.dir == Vector3.forward)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
                else if(p.dir == Vector3.left)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if(p.dir == Vector3.right)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
            }
            else if((curNode.wallDirections & WallDirection.Diagonal_Beta) != 0)
            {
                if(p.dir == Vector3.forward)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
                else if(p.dir == Vector3.right)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if(p.dir == Vector3.left)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
            }
            else
            {
                if ((wallDirections & WallDirection.North) == 0 && p.dir != Vector3.forward)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back)); // Avoid going back
                }

                if ((wallDirections & WallDirection.South) == 0 && p.dir != Vector3.back)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward)); // Avoid going forward
                }

                if ((wallDirections & WallDirection.West) == 0 && p.dir != Vector3.left)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right)); // Avoid going right
                }

                if ((wallDirections & WallDirection.East) == 0 && p.dir != Vector3.right)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left)); // Avoid going left
                }


            }

            //if wall directions include Wall.diagonal_alpha 
        }

        return filledPositions; // Return the list of filled positions
    }

}
