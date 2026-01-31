using UnityEngine;
using System.Collections.Generic;

namespace PuzzleGame.Gameplay.Settings
{
    [CreateAssetMenu(fileName = "GameplaySettings", menuName = "Puzzle Game/Gameplay Settings")]
    public class GameplaySettings : ScriptableObject
    {
        [Header("Placement Rules")]
        [Tooltip("If true, piece must be fully inside the board to place")]
        [SerializeField] private bool requireFullyInside = true;

        [Tooltip("If true, must fill all tiles to win. If false, only fill target tiles")]
        [SerializeField] private bool fillAllTiles = true;

        public bool RequireFullyInside => requireFullyInside;
        public bool FillAllTiles => fillAllTiles;

        /// <summary>
        /// Check if piece can be placed at position
        /// </summary>
        public bool CanPlacePiece(List<Vector2Int> cells, Vector2Int position, int boardWidth, int boardHeight)
        {
            if (requireFullyInside)
            {
                // All cells must be inside board
                foreach (var cell in cells)
                {
                    Vector2Int worldCell = cell + position;
                    if (worldCell.x < 0 || worldCell.x >= boardWidth ||
                        worldCell.y < 0 || worldCell.y >= boardHeight)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // At least one cell must be inside board
                foreach (var cell in cells)
                {
                    Vector2Int worldCell = cell + position;
                    if (worldCell.x >= 0 && worldCell.x < boardWidth &&
                        worldCell.y >= 0 && worldCell.y < boardHeight)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // Runtime setters for testing
        public void SetRequireFullyInside(bool value)
        {
            requireFullyInside = value;
        }

        public void SetFillAllTiles(bool value)
        {
            fillAllTiles = value;
        }
    }
}