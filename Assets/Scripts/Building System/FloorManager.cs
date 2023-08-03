using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : Singleton<FloorManager>
{
    public int numberOfBasementFloors = 0;
    public int numberOfNormalFloors = 1;
    public float floorHeight = 4f;
    public Floor floorPrefab;

    private Dictionary<float, Floor> floorDictionary = new Dictionary<float, Floor>();
    [SerializeField]private GameObject parentObject; // Parent object for the floors

    private void Awake()
    {
        //parentObject = new GameObject("FloorsParent"); // Create a new empty GameObject as the parent
        SpawnFloors();
    }

    [Button]
    private void SpawnFloors()
    {
        floorDictionary.Clear();
        // Delete the old parent GameObject before spawning new floors
        DestroyParentObject();
        parentObject = new GameObject("Floors Parent");
        parentObject.transform.parent = transform;
        for (int i = -numberOfBasementFloors; i <= numberOfNormalFloors; i++)
        {
            Floor newFloor = SpawnFloor(i);
            float height = i * floorHeight;
            floorDictionary.Add(height, newFloor);
        }


    }

    private Floor SpawnFloor(int floorNumber)
    {
        float heightOffset = floorNumber * floorHeight;
        Vector3 floorPosition = new Vector3(0f, heightOffset, 0f);
        Floor newFloor = Instantiate(floorPrefab, floorPosition, Quaternion.identity);
        newFloor.name = "Floor " + floorNumber;
        newFloor.transform.SetParent(parentObject.transform); // Set the floor as a child of the parent object
        // You can set the properties of the floor object here, e.g., materials, colliders, etc.
        return newFloor;
    }

    private void DestroyParentObject()
    {
        if (parentObject != null)
        {
            // Destroy all child objects first to avoid memory leaks
            foreach (Transform child in parentObject.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(parentObject);
        }
    }

    public Floor GetFloorByHeight(float height)
    {
        float nearestFloorHeight = Mathf.Round(height / floorHeight) * floorHeight;
        if (floorDictionary.TryGetValue(nearestFloorHeight, out Floor floor))
        {
            return floor;
        }
        return null;
    }

    [Button]
    public void AddNormalFloor()
    {
        numberOfNormalFloors++;
        Floor newFloor = SpawnFloor(numberOfNormalFloors);
        float height = numberOfNormalFloors * floorHeight;
        floorDictionary.Add(height, newFloor);
    }

    [Button]
    public void AddBasementFloor()
    {
        numberOfBasementFloors++;
        Floor newFloor = SpawnFloor(-numberOfBasementFloors);
        float height = -numberOfBasementFloors * floorHeight;
        floorDictionary.Add(height, newFloor);
    }
}
