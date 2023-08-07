using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Building_System.Test
{
    [System.FlagsAttribute]
    public enum HorizontalWallDirection
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
    }

    public class Node : MonoBehaviour
    {
        public Vector3 position;
        public HorizontalWallDirection wallDirections;
        public Dictionary<Wall, HorizontalWallDirection> wallDictionary;

        /*
        public Node(Vector3 position)
        {
            this.position = position;
        }
        */

        public void SetPos(Vector3 pos)
        {
            position = pos;
            transform.position = pos;
        }
        public void AddWall(Wall wall, HorizontalWallDirection horizontalWallDirection)
        {
            if (wallDictionary == null)
            {
                wallDictionary = new Dictionary<Wall, HorizontalWallDirection>();
            }

            wallDirections |= horizontalWallDirection;
            wallDictionary[wall] = horizontalWallDirection;
        }


        public void Remove(Wall wall)
        {
            var dir = wallDictionary[wall];
            wallDirections &= ~dir;
            wallDictionary.Remove(wall);
        }

        //public void 

    }
}
