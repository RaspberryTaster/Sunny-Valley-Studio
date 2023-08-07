using Assets.Scripts.Building_System.Test;
using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
                Vector3 placementPosition = Position(hit.point);

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

     
    private List<Vector3> FloodFill(Vector3 initialPosition)
    {
        HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
        Queue<PosDir> positionsToCheck = new Queue<PosDir>();
        List<Vector3> filledPositions = new List<Vector3>(); // Store filled positions

        positionsToCheck.Enqueue(new PosDir(initialPosition,Vector3.zero));

        while (positionsToCheck.Count > 0)
        {
            PosDir p = positionsToCheck.Dequeue();
            Vector3 position = p.pos;

            if (!IsWithinBoundsXZ(position) || visitedPositions.Contains(position))
            {
                continue;
            }

            visitedPositions.Add(position);

            if(filledPositions.Contains(position))
            {
                continue;
            }
            else if (placedWall.Keys.Contains(position))
            {
                //found walls
                //if you found both positions of the walls then add wall to room if not dont

                continue;
            }
            else
            {
                Instantiate(floodPrefab, PlacementUtils.WorldPositionToGridPosition(position,groundGrid), Quaternion.identity);
                //wallPositions.Add(position);
                filledPositions.Add(position); // Add filled position to the list
            }

            positionsToCheck.Enqueue(new PosDir(position +(Vector3.forward / 2), (Vector3.forward / 2)));
            positionsToCheck.Enqueue(new PosDir(position + (Vector3.back / 2), (Vector3.back / 2)));
            positionsToCheck.Enqueue(new PosDir(position + (Vector3.left / 2), (Vector3.left / 2)));
            positionsToCheck.Enqueue(new PosDir(position + (Vector3.right / 2), (Vector3.right / 2)));
        }

        return filledPositions; // Return the list of filled positions
    }

}
