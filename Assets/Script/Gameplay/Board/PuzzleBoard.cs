using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Settings;

namespace PuzzleGame.Gameplay.Board
{
    public class PuzzleBoard : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int boardWidth = 15;
        [SerializeField] private int boardHeight = 15;

        [Header("Tilemap References")]
        [SerializeField] private Tilemap tilemap;

        [Header("Tile Assets")]
        [SerializeField] private TileBase emptyTile;
        [SerializeField] private TileBase filledTile;
        [SerializeField] private TileBase targetTile;

        [Header("Gameplay Settings")]
        [SerializeField] private GameplaySettings settings;

        // Internal state
        private bool[,] boardState;
        private bool[,] targetMap;

        private void Awake()
        {
            InitializeBoard();
        }

        /// <summary>
        /// Initialize board with empty tiles
        /// </summary>
        public void InitializeBoard()
        {
            boardState = new bool[boardWidth, boardHeight];
            targetMap = new bool[boardWidth, boardHeight];

            // Fill board with empty tiles
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(pos, emptyTile);
                    boardState[x, y] = false;
                }
            }
        }

        /// <summary>
        /// Set tile value at position
        /// </summary>
        public void SetTileValue(Vector3Int pos, bool value)
        {
            if (!IsValidPosition(pos)) return;

            boardState[pos.x, pos.y] = value;
            tilemap.SetTile(pos, value ? filledTile : emptyTile);
        }

        /// <summary>
        /// Get tile value at position
        /// </summary>
        public bool GetTileValue(Vector3Int pos)
        {
            if (!IsValidPosition(pos)) return false;
            return boardState[pos.x, pos.y];
        }

        /// <summary>
        /// Check if position is a target tile
        /// </summary>
        public bool IsTargetTile(Vector3Int pos)
        {
            if (!IsValidPosition(pos)) return false;
            return targetMap[pos.x, pos.y];
        }

        /// <summary>
        /// Set target map from level config
        /// </summary>
        public void SetTargetMap(bool[,] map)
        {
            if (map.GetLength(0) != boardWidth || map.GetLength(1) != boardHeight)
            {
                Debug.LogError("Target map size mismatch!");
                return;
            }

            targetMap = map;
        }

        /// <summary>
        /// Check if piece can be placed at position
        /// </summary>
        public bool CanPlacePiece(PuzzlePiece piece, Vector2Int position)
        {
            if (settings == null)
            {
                Debug.LogWarning("GameplaySettings not assigned!");
                return false;
            }

            var cells = piece.GetOccupiedCells();
            return settings.CanPlacePiece(cells, position, boardWidth, boardHeight);
        }

        /// <summary>
        /// Place piece at position and apply operation
        /// </summary>
        public bool PlacePiece(PuzzlePiece piece, Vector2Int position)
        {
            if (!CanPlacePiece(piece, position))
                return false;

            var cells = piece.GetOccupiedCells(position);

            foreach (var cell in cells)
            {
                // Only process cells inside board
                if (cell.x >= 0 && cell.x < boardWidth && cell.y >= 0 && cell.y < boardHeight)
                {
                    Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
                    bool currentValue = boardState[cell.x, cell.y];
                    bool newValue = ApplyOperation(currentValue, piece.Type);
                    SetTileValue(pos, newValue);
                }
            }

            return true;
        }

        /// <summary>
        /// Apply piece operation to current tile value
        /// </summary>
        private bool ApplyOperation(bool currentValue, PieceType type)
        {
            switch (type)
            {
                case PieceType.OR:
                    return true; // Always set to 1

                case PieceType.AND:
                    return currentValue; // Keep current value

                case PieceType.NOT:
                    return !currentValue; // Flip value

                default:
                    return currentValue;
            }
        }

        /// <summary>
        /// Check if level is complete based on settings
        /// </summary>
        public bool IsComplete()
        {
            if (settings == null)
                return false;

            if (settings.FillAllTiles)
            {
                // Check if all tiles are filled
                for (int x = 0; x < boardWidth; x++)
                {
                    for (int y = 0; y < boardHeight; y++)
                    {
                        if (!boardState[x, y])
                            return false;
                    }
                }
                return true;
            }
            else
            {
                // Check if all target tiles are filled
                for (int x = 0; x < boardWidth; x++)
                {
                    for (int y = 0; y < boardHeight; y++)
                    {
                        if (targetMap[x, y] && !boardState[x, y])
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Convert grid position to world position
        /// </summary>
        public Vector3 GridToWorldPosition(Vector3Int gridPos)
        {
            return tilemap.CellToWorld(gridPos) + tilemap.tileAnchor;
        }

        /// <summary>
        /// Convert world position to grid position
        /// </summary>
        public Vector3Int WorldToGridPosition(Vector3 worldPos)
        {
            return tilemap.WorldToCell(worldPos);
        }

        /// <summary>
        /// Check if position is valid on board
        /// </summary>
        private bool IsValidPosition(Vector3Int pos)
        {
            return pos.x >= 0 && pos.x < boardWidth &&
                   pos.y >= 0 && pos.y < boardHeight;
        }

        // Getters
        public int BoardWidth => boardWidth;
        public int BoardHeight => boardHeight;
        public GameplaySettings Settings => settings;
    }
}