using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPlacementState : IPlayerState
{
    //sub grid
    public Tilemap tilemap;
    private Vector3 initalPosition;
    private Vector3 mousePosition;

    public GameObject mouseIndicator;
    public PlaceableObject objectToPlace;
    private PlaceableObject purchasedObject;
    
    public ObjectPlacementState(Tilemap tilemap, GameObject mouseIndicator)
    {
        this.tilemap = tilemap;
        this.mouseIndicator = mouseIndicator;
    }


    public ObjectPlacementState(Tilemap tilemap, GameObject mouseIndicator, PlaceableObject purchasedObject)
    {
        this.tilemap = tilemap;
        this.mouseIndicator = mouseIndicator;
        this.purchasedObject = purchasedObject;
    }


    public void Enter(PlayerController player)
    {
        Debug.LogWarning("Enterd: Object Placement State");
        // Perform any setup or actions when entering this state for Object Placement.
    }

    public void Update()
    {
        mousePosition = InputManager.Instance.GetSelectedMapPosition();

        if (mouseIndicator != null)
            mouseIndicator.transform.position = mousePosition;

        if(purchasedObject != null && InputManager.Instance.MouseOnGridLayer())
        {
            objectToPlace =  GameObject.Instantiate(purchasedObject, mousePosition, Quaternion.identity);
            purchasedObject = null;
            PickUpObject();
        }

        if (Input.GetMouseButtonDown(0))
        {

            if (objectToPlace != null)
            {
                if (!objectToPlace.CanBePlaced) return;
                objectToPlace.Drop(this);
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 900))
                {


                    Debug.Log(hit.transform.gameObject);
                    if (hit.transform.gameObject.TryGetComponent(out objectToPlace))
                    {
                        PickUpObject();
                    }
                }
            }


            //PRESS

        }

        if (Input.GetMouseButtonDown(1))
        {
            if (objectToPlace != null)
            {
                objectToPlace.transform.Rotate(Vector3.up, 45f); // Rotate 45 degrees clockwise on the Y-axis
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            objectToPlace.Drop(initalPosition, this);
        }



    }

    private void PickUpObject()
    {
        objectToPlace.Move();
        initalPosition = objectToPlace.transform.position;
    }

    public void LateUpdate()
    {
        if (objectToPlace == null) return;
        SetPosition(mousePosition);
    }

    public void SetPosition(Vector3 pos)
    {
        //check if position is out of bounds, if its out of bounds do not move it there.

        objectToPlace.gameObject.SnapToGrid(pos, tilemap);
    }

}