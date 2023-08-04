using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleRoomFinder : MonoBehaviour
{
    public Wall a;
    public Wall b;
    public List<Wall> rankedWalls;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if(a == null ||b == null) return;   
        //using linq rank the walls based on distance 
        Debug.Log("Wall distance: " + Vector3.Distance(a.wallObject.transform.position, b.wallObject.transform.position));
    }

    private void OnDrawGizmos()
    {
        if (a != null && b != null)
        {
            Vector3 bpos = (b.farPoint.position);
            Gizmos.DrawSphere(bpos,.5f);
        }
    }
}
