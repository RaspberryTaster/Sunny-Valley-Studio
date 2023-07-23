using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CreateWall : MonoBehaviour
{
    public bool creating;
    public GameObject polePrefab;
    public GameObject wallPrefab;
    public Grid wallGrid;
    private GameObject lastPole;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartFence();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetFence();
        }
        else
        {
            if (creating)
            {
                UpdateFence();
            }
        }
    }

    void StartFence()
    {
        creating = true;
        GameObject startPole = Instantiate(polePrefab, Position(InputManager.Instance.GetSelectedMapPosition()), Quaternion.identity);
        lastPole = startPole;
    }

    void SetFence()
    {
        creating = false;   
    }

    void UpdateFence()
    {
        Vector3 current = Position(InputManager.Instance.GetSelectedMapPosition());
        if(!current.Equals(lastPole.transform.position)) { 
            
        }
    }

    public Vector3 Position(Vector3 position)
    {
        Vector3Int gridPosition = wallGrid.WorldToCell(position);
        return wallGrid.CellToWorld(gridPosition);
    }
}
