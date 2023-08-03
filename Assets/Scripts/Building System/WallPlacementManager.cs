using Assets.Scripts.Extensions;
using UnityEngine;

public class WallPlacementManager : Singleton<WallPlacementManager>
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    public GameObject horizontalWallPrefab;
    public GameObject diagonalWallPrefab;

    private GameObject wallsParent; // Parent GameObject to hold the wall cubes

    private Vector3 previousStartPoint;
    private Vector3 previousEndPoint;

    private void Start()
    {
        previousStartPoint = startPoint;
        previousEndPoint = endPoint;
        //SpawnWalls();
    }

    private void Update()
    {
        // Check if the start or end points have changed
        //UpdateWall();
    }

    public void UpdateWall()
    {
        if (startPoint != previousStartPoint || endPoint != previousEndPoint)
        {
            DeleteCubes();
            SpawnWalls();
            previousStartPoint = startPoint;
            previousEndPoint = endPoint;
        }
    }

    private void DeleteCubes()
    {
        // Destroy the parent GameObject (walls) and all its children (cubes)
        Destroy(wallsParent);
    }

    public void DetatchWalls()
    {
        if (wallsParent != null)
        {
            wallsParent.transform.parent = null;
            wallsParent = null;
        }
    }

    public void SetPosition(Vector3 newStartPoint, Vector3 newEndPoint)
    {
        // Calculate the angle between the new start and end points
        float angle = Vector3.Angle(newEndPoint - newStartPoint, Vector3.right);

        // Check if the angle is neither horizontal nor diagonal
        if (!PlacementUtils.IsHorizontal(angle) && angle % 45 != 0)
        {
            Debug.LogWarning("Cannot change position. The angle between the points is neither horizontal nor diagonal.");
            return;
        }

        startPoint = newStartPoint;
        endPoint = newEndPoint;
    }

    public void SetEndPosition(Vector3 newEndPoint)
    {
        // Calculate the angle between the new start and end points
        float angle = Vector3.Angle(newEndPoint - startPoint, Vector3.right);

        // Check if the angle is neither horizontal nor diagonal
        if (!PlacementUtils.IsHorizontal(angle) && angle % 45 != 0)
        {
            Debug.LogWarning("Cannot change position. The angle between the points is neither horizontal nor diagonal.");
            return;
        }

        endPoint = newEndPoint;
    }

    private void SpawnWalls()
    {
        wallsParent = new GameObject("WallsParent"); // Create the parent GameObject
        wallsParent.transform.parent = transform;
        Vector3 direction = (endPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, endPoint);
        int numCubes = Mathf.FloorToInt(distance / GetSpacing());
        float spacing = GetSpacing();
        if (PlacementUtils.IsDiagonal(endPoint, startPoint))
        {
            for (int i = 0; i < numCubes; i++)
            {
                Spawn(diagonalWallPrefab, direction, spacing, i);
            }
            //spawn diagonal
        }
        else
        {
            for (int i = 0; i < numCubes; i++)
            {
                Spawn(horizontalWallPrefab, direction, spacing, i);
            }
        }
    }

    private void Spawn(GameObject wall, Vector3 direction, float spacing, int i)
    {
        Vector3 spawnPosition = startPoint + direction * (spacing * i);
        GameObject newCube = Instantiate(wall, spawnPosition, Quaternion.identity);
        newCube.transform.forward = direction;
        // Optionally, you can parent the cubes to the wallsParent for organization.
        newCube.transform.parent = wallsParent.transform;
        // Align the cubes along the line direction
    }

    // Calculate the spacing based on the angle between start and end points
    private float GetSpacing()
    {
        return PlacementUtils.IsDiagonal(endPoint,startPoint) ? GetDiagonalSpacing() : 0.5f;
    }

    private float GetDiagonalSpacing()
    {
        return Mathf.Sqrt(0.5f * 0.5f * 2f);
    }

    private void OnDrawGizmos()
    {
        float spacing = GetSpacing();
        if (startPoint != null && endPoint != null)
        {
            // Draw the line between the start and end points
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPoint, endPoint);

            // Draw the start and end points as spheres
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startPoint, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint, 0.1f);

            // Calculate the number of cubes to be spawned along the line
            Vector3 direction = (endPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, endPoint);
            int numCubes = Mathf.FloorToInt(distance / spacing);

            // Draw the cubes along the line
            Gizmos.color = Color.green;
            for (int i = 0; i < numCubes; i++)
            {
                Vector3 spawnPosition = startPoint + direction * (spacing * i);
                Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.1f);
            }
        }
    }
}
