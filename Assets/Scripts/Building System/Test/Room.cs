using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Building_System.Test
{
    [System.Serializable]
    public class Room
    {
        public List<Wall> walls = new List<Wall>();
        public 
        public Vector3 position;

        public void AddWall(Wall wall)
        {
            if (wall == null) return;
            
            walls.Add(wall);
            walls = walls.Where(element => element != null).ToList();
            RecalculatePosition();
        }

        private void RecalculatePosition()
        {
            
            if (walls.Count == 0)
            {
                position = Vector3.zero;
            }
            else
            {
                position = walls.Select(w => w.transform.position).Aggregate((acc, pos) => acc + pos) / walls.Count;
            }
        }
    }
}
