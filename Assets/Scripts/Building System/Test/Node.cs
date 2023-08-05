using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Building_System.Test
{
    [System.FlagsAttribute]
    public enum WallDirection
    {
        None = 0,
        North = 1,
        East = 2,
        South = 3,
        West = 4,
        DiagonalEast = 5,
        DiagonalWest = 6,

    }

    public class Node :MonoBehaviour
    {
        public WallDirection wallDirections;

        public Node(WallDirection wallDirections)
        {
            this.wallDirections = wallDirections;
        }
    }
}
