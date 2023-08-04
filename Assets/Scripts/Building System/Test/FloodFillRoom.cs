using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloodFillRoom : Singleton<FloodFillRoom>
{
    public Tilemap wallPlacementGrid;
    public Renderer boundsPlane;
    public GameObject placementPrefab;
    public GameObject floodPrefab;

    private Camera mainCamera;
    private Dictionary<GameObject, Vector3> placedObjects = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 placementPosition = Position(hit.point);

                // Check if placement position is within bounds
                if (IsWithinBoundsXZ(placementPosition))
                {
                    GameObject placedObject = PlacePrefab(placementPosition);
                    if (placedObject != null)
                    {
                        Vector3Int gridPosition = wallPlacementGrid.WorldToCell(placementPosition);
                        placedObjects.Add(placedObject, placementPosition);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 placementPosition = Position(hit.point);

                // Check if placement position is within bounds
                if (IsWithinBoundsXZ(placementPosition))
                {
                    FloodFill(placementPosition);
                }
            }

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

    private GameObject PlacePrefab(Vector3 position)
    {
        GameObject prefabInstance = Instantiate(placementPrefab, position, Quaternion.identity);
        return prefabInstance;
    }

    private void FloodFill(Vector3 position)
    {
        RecursiveFloodFill(position);
    }

    private void RecursiveFloodFill(Vector3 position)
    {
        if (!IsWithinBoundsXZ(position))
        {
            return;
        }

        if (placedObjects.Values.Contains(position))
        {
            //Vector3 worldPosition = wallPlacementGrid.GetCellCenterWorld(position);

            //wallPlacementGrid.SetTile(position, floodPrefab.GetComponent<Tile>());
            return;
        }
        else
        {
            //po
            var g = Instantiate(floodPrefab, position, Quaternion.identity);
            placedObjects.Add(g, position);
            //return; // Stop the flood if a tile is already present here
        }

        RecursiveFloodFill(position + Vector3Int.forward);
        RecursiveFloodFill(position + Vector3Int.back);
        RecursiveFloodFill(position + Vector3Int.left);
        RecursiveFloodFill(position + Vector3Int.right);
    }
}
