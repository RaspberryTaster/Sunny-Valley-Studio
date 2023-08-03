using UnityEngine;

public class NullPlayerState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.LogWarning("Enterd: Null State");
    }

    public void LateUpdate()
    {
        //throw new System.NotImplementedException();
    }

    public void Update()
    {
        // Implement Object Placement behavior here.
    }
}