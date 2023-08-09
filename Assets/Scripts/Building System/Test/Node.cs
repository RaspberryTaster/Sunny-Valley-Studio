using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Building_System.Test
{
    [System.FlagsAttribute]
    public enum WallDirection
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        Diagonal_Alpha = 16,
        Diagonal_Beta = 32,
    }

    public class Node : MonoBehaviour
    {
        public Vector3 position;
        public WallDirection wallDirections;
        public Dictionary<Wall, WallDirection> wallDictionary;


        public void SetPos(Vector3 pos)
        {
            position = pos;
            transform.position = pos;
        }

        public void AddWall(Wall wall, WallDirection wallDirection)
        {
            if (wallDictionary == null)
            {
                wallDictionary = new Dictionary<Wall, WallDirection>();
            }

            wallDirections |= wallDirection;
            wallDictionary[wall] = wallDirection;
        }

        public void Remove(Wall wall)
        {
            if (wallDictionary.TryGetValue(wall, out WallDirection direction))
            {
                wallDirections &= ~direction;
                wallDictionary.Remove(wall);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if ((wallDirections & WallDirection.North) == 0)
            {
                Gizmos.DrawSphere(position + Vector3.forward, 0.2f);
            }

            if ((wallDirections & WallDirection.South) == 0)
            {
                Gizmos.DrawSphere(position + Vector3.back, 0.2f);
            }

            if ((wallDirections & WallDirection.West) == 0)
            {
                Gizmos.DrawSphere(position + Vector3.left, 0.2f);
            }

            if ((wallDirections & WallDirection.East) == 0)
            {
                Gizmos.DrawSphere(position + Vector3.right, 0.2f);
            }
        }
    }
}
