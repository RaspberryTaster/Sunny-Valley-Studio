using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class CreateWall : MonoBehaviour
{
    public bool creating;
    public Tilemap wallGrid;
    public GameObject mouseIndicator;

    // Update is called once per frame
    void Update()
    {


        mouseIndicator.transform.position = Position(InputManager.Instance.GetSelectedMapPosition());

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
                WallPlacementManager.Instance.SetEndPosition(Position(InputManager.Instance.GetSelectedMapPosition()));
                WallPlacementManager.Instance.UpdateWall();
            }
        }
    }

    void StartFence()
    {
        creating = true;
        //GameObject startPole = Instantiate(polePrefab, Position(InputManager.Instance.GetSelectedMapPosition()), Quaternion.identity);
        //lastPole = startPole;
        WallPlacementManager.Instance.SetPosition(Position(InputManager.Instance.GetSelectedMapPosition()), Position(InputManager.Instance.GetSelectedMapPosition()));
    }

    void SetFence()
    {
        creating = false;   
        WallPlacementManager.Instance.DetatchWalls();
    }

    public Vector3 Position(Vector3 position)
    {
        Vector3Int gridPosition = wallGrid.WorldToCell(position);
        return wallGrid.GetCellCenterWorld(gridPosition);
    }
}
