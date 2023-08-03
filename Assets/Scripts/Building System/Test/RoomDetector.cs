using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    private bool roomDetected = false;

    [Button]
    // Method to start the room detection process
    public void DetectRoom()
    {
        roomDetected = false;
        Collider[] colliders = GameObject.FindObjectsOfType<Collider>();

        Debug.Log(colliders.Length);    
        if (colliders.Length >= 3)
        {
            foreach (var collider in colliders)
            {
                if (!roomDetected && collider.GetComponent<TestWall>())
                {
                    DFS(collider.transform, new List<Transform>());
                }
            }
        }

        if (roomDetected)
        {
            Debug.Log("A complete room has been detected!");
        }
        else
        {
            Debug.Log("No complete room detected.");
        }
    }

    private void DFS(Transform currentWall, List<Transform> visitedWalls)
    {
        if (visitedWalls.Contains(currentWall))
        {
            roomDetected = true;
            return;
        }

        visitedWalls.Add(currentWall);

        foreach (var connectedWall in currentWall.GetComponent<TestWall>().connectedWalls)
        {
            DFS(connectedWall, visitedWalls);
        }
    }
}
