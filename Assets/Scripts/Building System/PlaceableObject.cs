using Assets.Scripts.Building_System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableObject : MonoBehaviour,ICollidable
{
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] Vertices;
    public bool CanBePlaced
    {
        get
        {
            bool isIntersecting = intersectingObjects.Count > 0;
            
            return !isIntersecting;
            
        }
    }

    List<ICollidable> intersectingObjects = new();
    [SerializeField]private ObjectGrid objectGrid;
    [SerializeField] public NavMeshSurface currentFloor;

    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[1] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
        Vertices[2] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
        Vertices[3] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
    }

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = PlayerController.Instance.objectPlacementGrid.WorldToCell(worldPos);
        }

        Size = new Vector3Int(Mathf.Abs((vertices[0] - vertices[1]).x), Mathf.Abs((vertices[0] - vertices[3]).y), 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.TryGetComponent(out ICollidable intersecting))
        {
            intersectingObjects.Add(intersecting);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.TryGetComponent(out ICollidable intersecting))
        {
            intersectingObjects.Remove(intersecting);
        }
    }
    private Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    public void Move()
    {
        objectGrid.gameObject.SetActive(true);
    }
    public void Drop(ObjectPlacementState placementSystem)
    {
        //Debug.Log("Drop");
        placementSystem.objectToPlace = null;
        objectGrid.gameObject.SetActive(false);
        currentFloor?.BuildNavMesh();

    }

    public void Drop(Vector3 position, ObjectPlacementState placementSystem)
    {
        placementSystem.SetPosition(position);
        //LET GO
        Drop(placementSystem);
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    private void Awake()
    {

        Floor currentFloor = FloorManager.Instance.GetFloorByHeight(transform.position.y);
        currentFloor.Subscribe(this);

        objectGrid.gameObject.SetActive(false);
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
        objectGrid.placeableObject = this;

    }

    private void Update()
    {
      

    }

    private void OnDrawGizmos()
    {
        if (Vertices == null)
            return;

        for (int i = 0; i < Vertices.Length; i++)
        {
            Gizmos.color = GetVertexColor(i);
            Gizmos.DrawSphere(transform.TransformPoint(Vertices[i]), 0.1f);
        }
    }


    private Color GetVertexColor(int index)
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow }; // Add more colors as needed
        return colors[index % colors.Length];
    }

}
