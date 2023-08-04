using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class YourMainScript : MonoBehaviour
{
    public Wall startingWall; // Assign the starting wall in the Inspector
    private WallPathFinder pathFinder;

    private void Start()
    {
        pathFinder = new WallPathFinder();


        // Now you have all the paths that lead back to the starting wall in the 'allPaths' list
        // You can process and use this information as needed
    }
    [Button]
    public void Find()
    {
        List<List<Wall>> allPaths = pathFinder.FindAllPaths(startingWall);
        Debug.Log("All paths: " + allPaths.Count);
        foreach(var x in allPaths)
        {
            Debug.Log("Wall size: " + x.Count);
        }
    }
}
