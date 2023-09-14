using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Singleton<PlayerController>
{
    private IPlayerState currentState;

    public Tilemap objectPlacementGrid;
    public Tilemap wallGrid;
    public GameObject mouseIndicator;

    public IPlayerState _wallPlacementState;
    public IPlayerState _nullPlayerState;
    public IPlayerState _objectPlacementState; 
    private void Awake()
    {
        _wallPlacementState = new WallPlacementState(wallGrid, mouseIndicator);
        _nullPlayerState = new NullPlayerState();
        _objectPlacementState = new ObjectPlacementState(objectPlacementGrid, mouseIndicator);

    }
    public void SetState(IPlayerState state)
    {
        currentState = state;
        currentState.Enter(this);
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void LateUpdate()
    {
        currentState?.LateUpdate();
    }
    public void Start()
    {
        // Initialize the player with a default state, e.g., Wall Placement.
        SetNullState();
    }

    public void SetWallPlacementState()
    {
        SetState(_wallPlacementState);
    }
    public void SetNullState()
    {
        SetState(_nullPlayerState);
    }
    public void SetObjectPlacementState()
    {
        SetState(_objectPlacementState);
    }

    public void SetObjectPlacementState(PlaceableObject placeableObject)
    {
        SetState(new ObjectPlacementState(objectPlacementGrid, mouseIndicator, placeableObject));
    }
}
