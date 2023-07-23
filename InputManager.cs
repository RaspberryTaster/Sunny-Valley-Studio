using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private Camera sceneCamera;
    private Vector3 lastPosition;
    [SerializeField] private LayerMask placementLayerMask;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(sceneCamera.transform.position, lastPosition);
    }

    public Vector3 GetSelectedMapPosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(placementLayerMask.Contains(hit.transform.gameObject.layer))
            {
                lastPosition = hit.point;
                Debug.Log("Position stuff: " + hit.transform.gameObject);
            }

        }

        return lastPosition;
    }
}
