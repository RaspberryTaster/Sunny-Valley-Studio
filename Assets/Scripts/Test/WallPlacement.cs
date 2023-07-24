using System.Collections;
using UnityEngine;

public class WallPlacement : Singleton<WallPlacement>
{
    public Transform startPoint;
    public Transform endPoint;
    public GameObject horizontalWallPrefab;
    public GameObject diagonalWallPrefab;

    private GameObject wallsParent; // Parent GameObject to hold the wall cubes


    private Vector3 previousStartPoint;
    private Vector3 previousEndPoint;

    private void Start()
    {
        previousStartPoint = startPoint.position;
        previousEndPoint = endPoint.position;
        //SpawnCubes();
    }

    private void Update()
    {
        // Check if the start or end points have changed
        //UpdateWall();
    }

    public void UpdateWall()
    {
        if (startPoint.position != previousStartPoint || endPoint.position != previousEndPoint)
        {
            DeleteCubes();
            SpawnCubes();
            previousStartPoint = startPoint.position;
            previousEndPoint = endPoint.position;
        }
    }

    private void DeleteCubes()
    {
        // Destroy the parent GameObject (walls) and all its children (cubes)
        Destroy(wallsParent);
    }

    public void DetatchWalls()
    {
        wallsParent.transform.parent = null;
        wallsParent = null;
    }

    public void SetPosition(Vector3 newStartPoint, Vector3 newEndPoint)
    {
        // Calculate the angle between the new start and end points
        float angle = Vector3.Angle(newEndPoint - newStartPoint, Vector3.right);

        // Check if the angle is neither horizontal nor diagonal
        if (!IsHorizontal(angle) && angle % 45 != 0)
        {
            Debug.LogWarning("Cannot change position. The angle between the points is neither horizontal nor diagonal.");
            return;
        }

        // Update the start and end points
        if (startPoint == null)
        {
            GameObject startPointObj = new GameObject("StartPoint");
            startPoint = startPointObj.transform;
            startPoint.parent = transform;

        }
        startPoint.position = newStartPoint;

        if (endPoint == null)
        {
            GameObject endPointObj = new GameObject("EndPoint");
            endPoint = endPointObj.transform;
            endPoint.parent = transform;
        }
        endPoint.position = newEndPoint;
    }

    public void SetEndPosition(Vector3 newEndPoint)
    {
        // Calculate the angle between the new start and end points
        float angle = Vector3.Angle(newEndPoint - startPoint.transform.position, Vector3.right);

        // Check if the angle is neither horizontal nor diagonal
        if (!IsHorizontal(angle) && angle % 45 != 0)
        {
            Debug.LogWarning("Cannot change position. The angle between the points is neither horizontal nor diagonal.");
            return;
        }

        if (endPoint == null)
        {
            GameObject endPointObj = new GameObject("EndPoint");
            endPoint = endPointObj.transform;
            endPoint.parent = transform;
        }
        endPoint.position = newEndPoint;
    }
    private void SpawnCubes()
    {
        wallsParent = new GameObject("WallsParent"); // Create the parent GameObject
        wallsParent.transform.parent = transform;
        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        int numCubes = Mathf.FloorToInt(distance / GetSpacing());
        float spacing = GetSpacing();
        if (IsDiagonal())
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
        Vector3 spawnPosition = startPoint.position + direction * (spacing * i);
        GameObject newCube = Instantiate(wall, spawnPosition, Quaternion.identity);
        newCube.transform.forward = direction;
        // Optionally, you can parent the cubes to the wallsParent for organization.
        newCube.transform.parent = wallsParent.transform;
         // Align the cubes along the line direction
    }


    // Calculate the spacing based on the angle between start and end points
    private float GetSpacing()
    {
        return IsDiagonal()? GetDiagonalSpacing() : 0.5f;
    }

    private float GetDiagonalSpacing()
    {
        return Mathf.Sqrt(0.5f * 0.5f * 2f);
    }

    private bool IsDiagonal()
    {
        float angle = Vector3.Angle(endPoint.position - startPoint.position, Vector3.right);
        return !IsHorizontal(angle) && angle % 45 == 0;
    }

    private static bool IsHorizontal(float angle)
    {
        return angle % 90 == 0;
    }

    private void OnDrawGizmos()
    {
        float spacing = GetSpacing();
        if (startPoint != null && endPoint != null)
        {
            // Draw the line between the start and end points
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // Calculate the number of cubes to be spawned along the line
            Vector3 direction = (endPoint.position - startPoint.position).normalized;
            float distance = Vector3.Distance(startPoint.position, endPoint.position);
            int numCubes = Mathf.FloorToInt(distance / spacing);

            // Draw the cubes along the line
            Gizmos.color = Color.green;
            for (int i = 0; i < numCubes; i++)
            {
                Vector3 spawnPosition = startPoint.position + direction * (spacing * i);
                Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.1f);
            }
        }
    }
}
