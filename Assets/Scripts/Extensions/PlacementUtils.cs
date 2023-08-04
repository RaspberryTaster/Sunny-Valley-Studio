using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Extensions
{
    /// <summary>
    /// Utility class for positioning and snapping GameObjects to a grid represented by a Tilemap.
    /// </summary>
    public static class PlacementUtils
    {
        /// <summary>
        /// Converts a world position to the nearest grid position based on the provided Tilemap.
        /// If the position is out of bounds, it will be snapped to the nearest valid cell center.
        /// </summary>
        /// <param name="pos">The world position to convert.</param>
        /// <param name="tilemap">The Tilemap representing the grid.</param>
        /// <returns>The snapped grid position in world coordinates.</returns>
        public static Vector3 WorldPositionToGridPosition(Vector3 pos, Tilemap tilemap)
        {
            Vector3Int gridPosition = tilemap.WorldToCell(pos);
            return tilemap.GetCellCenterWorld(gridPosition);
        }

        /// <summary>
        /// Snaps the GameObject's position to the nearest grid position based on the provided Tilemap.
        /// If the position is out of bounds, it will be snapped to the nearest valid cell center.
        /// </summary>
        /// <param name="gameObject">The GameObject to snap to the grid.</param>
        /// <param name="tilemap">The Tilemap representing the grid.</param>
        public static void SnapToGrid(this GameObject gameObject, Tilemap tilemap)
        {
            Vector3 snappedPosition = WorldPositionToGridPosition(gameObject.transform.position, tilemap);
            gameObject.transform.position = snappedPosition;
        }

        /// <summary>
        /// Snaps a position to the nearest grid position based on the provided Tilemap and updates the GameObject's position.
        /// If the position is out of bounds, it will be snapped to the nearest valid cell center.
        /// </summary>
        /// <param name="gameObject">The GameObject whose position will be updated.</param>
        /// <param name="pos">The position to snap to the grid.</param>
        /// <param name="tilemap">The Tilemap representing the grid.</param>
        public static void SnapToGrid(this GameObject gameObject, Vector3 pos, Tilemap tilemap)
        {
            Vector3 snappedPosition = WorldPositionToGridPosition(pos, tilemap);
            gameObject.transform.position = snappedPosition;
        }

        /// <summary>
        /// Determines if the line connecting the startPoint and endPoint is diagonal.
        /// </summary>
        /// <param name="startPoint">The starting point of the line.</param>
        /// <param name="endPoint">The ending point of the line.</param>
        /// <returns><c>true</c> if the line is diagonal, otherwise <c>false</c>.</returns>
        public static bool IsDiagonal(Vector3 startPoint, Vector3 endPoint)
        {
            float angle = Vector3.Angle(endPoint - startPoint, Vector3.right);
            return !IsHorizontal(angle) && angle % 45 == 0;
        }

        /// <summary>
        /// Determines if the angle is horizontal (90 degrees).
        /// </summary>
        /// <param name="angle">The angle to check.</param>
        /// <returns><c>true</c> if the angle is horizontal, otherwise <c>false</c>.</returns>
        public static bool IsHorizontal(float angle)
        {
            return angle % 90 == 0;
        }



    }
}
