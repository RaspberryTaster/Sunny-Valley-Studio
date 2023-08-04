using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPlacer : MonoBehaviour
{
    public Tilemap placementGrid;
    public Renderer boundsPlane;
    public GameObject placementPrefab;
    public GameObject floodPrefab;

    private Camera mainCamera;
    private Dictionary<GameObject, Vector3Int> placedObjects = new Dictionary<GameObject, Vector3Int>();

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
                        Vector3Int gridPosition = placementGrid.WorldToCell(placementPosition);
                        placedObjects.Add(placedObject, gridPosition);
                    }
                }
            }
        }
    }

    public Vector3 Position(Vector3 position)
    {
        Vector3Int gridPosition = placementGrid.WorldToCell(position);
        return placementGrid.GetCellCenterWorld(gridPosition);
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
}
