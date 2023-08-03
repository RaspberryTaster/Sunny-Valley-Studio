using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Singleton<PlayerController>
{
    private IPlayerState currentState;

    public Tilemap objectPlacementGrid;
    public Tilemap wallGrid;
    public GameObject mouseIndicator;
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
        SetState(new WallPlacementState(wallGrid, mouseIndicator));
    }
    public void SetNullState()
    {
        SetState(new NullPlayerState());
    }
    public void SetObjectPlacementState()
    {
        SetState(new ObjectPlacementState(objectPlacementGrid, mouseIndicator));
    }

    public void SetObjectPlacementState(PlaceableObject placeableObject)
    {
        SetState(new ObjectPlacementState(objectPlacementGrid, mouseIndicator, placeableObject));
    }
}
