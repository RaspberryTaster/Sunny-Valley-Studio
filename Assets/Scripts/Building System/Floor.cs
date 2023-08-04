using Assets.Scripts.Building_System.Test;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Floor : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public List<GameObject> objectsOnFloor = new List<GameObject>();

    private List<PlaceableObject> placeableObjects = new List<PlaceableObject>();
    public Room[] rooms;
    // ... Other script logic for Floor ...


    private void Start()
    {
        navMeshSurface.BuildNavMesh();
    }
    public void Subscribe(PlaceableObject p)
    {
        placeableObjects.Add(p);
        p.currentFloor = navMeshSurface;
    }

    public void Unsubscribe(PlaceableObject p)
    {
        placeableObjects.Remove(p); 
    }

}
