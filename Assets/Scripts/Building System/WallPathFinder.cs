using System.Collections.Generic;
using UnityEngine;

public class WallPathFinder
{
    private List<Wall> visitedWalls;
    private List<List<Wall>> allPaths;

    public List<List<Wall>> FindAllPaths(Wall startWall)
    {
        visitedWalls = new List<Wall>();
        allPaths = new List<List<Wall>>();

        DFS(startWall, new List<Wall>());

        return allPaths;
    }

    private void DFS(Wall currentWall, List<Wall> currentPath)
    {
        visitedWalls.Add(currentWall);
        currentPath.Add(currentWall);

        foreach (Wall connectedWall in currentWall.connectedWalls)
        {
            if (!visitedWalls.Contains(connectedWall))
            {
                DFS(connectedWall, new List<Wall>(currentPath));
            }
            else if (connectedWall == currentPath[0])
            {
                // Found a path that leads back to the starting wall
                allPaths.Add(new List<Wall>(currentPath));
            }
        }
    }
}
