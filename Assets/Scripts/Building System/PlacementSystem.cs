using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementSystem : Singleton<PlacementSystem>
{
    [SerializeField] GameObject mouseIndicator;
    [SerializeField] public Grid subGrid;
    [SerializeField] private TileBase whiteTile;
    public Grid mainGrid;
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
                objectToPlace = null;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 900))
                {


                    Debug.Log(hit.transform.gameObject);
                    if (hit.transform.gameObject.TryGetComponent<PlaceableObject>(out objectToPlace))
                    {
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

            SetPosition(initalPosition);
            //LET GO
            objectToPlace = null;

            Debug.Log("Let Go");
        }


    }
    private void LateUpdate()
    {
        if (objectToPlace == null) return;
        SetPosition(mousePosition);
    }

    private void SetPosition(Vector3 pos)
    {
        //check if position is out of bounds, if its out of bounds do not move it there.

        Vector3Int gridPosition = subGrid.WorldToCell(pos);
        objectToPlace.transform.position = subGrid.CellToWorld(gridPosition);
    }

}
