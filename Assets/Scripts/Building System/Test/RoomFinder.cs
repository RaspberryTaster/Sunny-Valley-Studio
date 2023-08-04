using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Building_System.Test
{
    public class RoomFinder : MonoBehaviour
    {
        public List<Room> roomers;

        public List<Room> FindRooms()
        {
            List<Room> rooms = new List<Room>();
            Wall[] walls = FindObjectsOfType<Wall>();
            HashSet<Wall> visitedWalls = new HashSet<Wall>();

            foreach (var wall in walls)
            {
                if (!visitedWalls.Contains(wall))
                {
                    Room room = new Room();
                    DFS(wall, room);
                    rooms.Add(room);
                }
            }

            return rooms;
        }

        private void Update()
        {
            // Find rooms and get a list of Room instances
            roomers = FindRooms();

            //Debug.Log(roomers.Count);
            // Do whatever you want with the detected rooms
            foreach (var room in roomers)
            {
                Debug.Log("Found a new room with " + room.walls.Count + " walls!");
            }
        }

        private void DFS(Wall startWall, Room room)
        {
            HashSet<Wall> visitedWalls = new();
            Stack<Wall> stack = new();
            stack.Push(startWall);
            visitedWalls.Add(startWall);
            while (stack.Count > 0)
            {
                Wall currentWall = stack.Pop();
                
                room.AddWall(currentWall);

                if (room.walls.Count >= 3 && currentWall.connectedWalls.Contains(startWall))
                    return;
                foreach (var connectedWall in currentWall.connectedWalls)
                {
                    if (!visitedWalls.Contains(connectedWall))
                    {
                        stack.Push(connectedWall);
                        visitedWalls.Add(connectedWall);
                    }
                }

            }
        }   
 

        private void OnDrawGizmos()
        {
            foreach (var x in roomers)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(x.position, .6f);
            }
        }
    }
}
