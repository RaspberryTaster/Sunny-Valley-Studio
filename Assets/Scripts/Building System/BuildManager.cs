using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    // Reference to the PlayerController script

    // Reference to the GameObject called "catalogue"
    public GameObject catalogue;

    // Method to activate the catalogue and set the object placement state
    public void StartBuilding()
    {
        if(catalogue.activeInHierarchy)
        {
            // Activate the catalogue GameObject
            catalogue.SetActive(false);

            // Set the object placement state in the PlayerController
            PlayerController.Instance.SetNullState();
        }
        else
        {
            // Activate the catalogue GameObject
            catalogue.SetActive(true);

            // Set the object placement state in the PlayerController
            PlayerController.Instance.SetObjectPlacementState();
        }

    }

    public void Purchase(PlaceableObject placeableObject)
    {
        PlayerController.Instance.SetObjectPlacementState(placeableObject);
    }
}
