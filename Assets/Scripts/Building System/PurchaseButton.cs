using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseButton : MonoBehaviour
{
    public PlaceableObject prefab;

    public void Execute()
    {
        if(prefab != null)
            BuildManager.Instance.Purchase(prefab);
    }
}
