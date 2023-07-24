using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementSystem : Singleton<PlacementSystem>
{
    [SerializeField] GameObject mouseIndicator;
    [SerializeField] public Tilemap subGrid;
    public Tilemap mainGrid;
    //public Tilemap tilemap;
    public PlaceableObject objectToPlace;
    private Vector3 initalPosition;
    private Vector3 mousePosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = InputManager.Instance.GetSelectedMapPosition();

        mouseIndicator.transform.position = mousePosition;

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
                        objectToPlace.Move();
                        initalPosition = objectToPlace.transform.position;
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



            objectToPlace.Drop(initalPosition,this);
        }


    }
    private void LateUpdate()
    {
        if (objectToPlace == null) return;
        SetPosition(mousePosition);
    }

    public void SetPosition(Vector3 pos)
    {
        //check if position is out of bounds, if its out of bounds do not move it there.

        Vector3Int gridPosition = subGrid.WorldToCell(pos);
        objectToPlace.transform.position = subGrid.GetCellCenterWorld(gridPosition);
    }

}
