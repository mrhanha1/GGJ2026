using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Settings;
using PuzzleGame.Gameplay.Level;

namespace PuzzleGame.Gameplay.Board
{
    public class PuzzleBoard : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int boardWidth = 15;
        [SerializeField] private int boardHeight = 15;

        [Header("Tilemap References")]
        [SerializeField] private Tilemap tilemap; // Main gameplay tilemap
        [SerializeField] private Tilemap backgroundTilemap; // Background image tilemap (NEW)
        [SerializeField] private Tilemap overlayTilemap; // Target overlay tilemap (NEW)

        [Header("Tile Assets")]
        [SerializeField] private TileBase emptyTile;
        [SerializeField] private TileBase filledTile;
        [SerializeField] private TileBase targetTile;

        [Header("Colored Tiles for Image (NEW)")]
        [SerializeField] private TileBase[] coloredTiles; // 7 different colored tiles

        [Header("Gameplay Settings")]
        [SerializeField] private GameplaySettings settings;

        // Internal state
        private bool[,] boardState;
        private bool[,] targetMap;
        private LevelConfig currentLevel;

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

            // Clear background and overlay
            if (backgroundTilemap != null)
                backgroundTilemap.ClearAllTiles();
            if (overlayTilemap != null)
                overlayTilemap.ClearAllTiles();
        }

        /// <summary>
        /// Load level and display target image/overlay (NEW)
        /// </summary>
        public void LoadLevel(LevelConfig level)
        {
            if (level == null)
            {
                Debug.LogWarning("[PuzzleBoard] Level config is null!");
                return;
            }

            currentLevel = level;

            // Show target image as background
            ShowTargetImage();

            // Show target overlay if not filling all tiles
            ShowTargetOverlay();

            Debug.Log($"[PuzzleBoard] Loaded level: {level.LevelName}");
        }

        /// <summary>
        /// Show target image as semi-transparent background (NEW)
        /// </summary>
        private void ShowTargetImage()
        {
            if (backgroundTilemap == null || currentLevel == null)
                return;

            backgroundTilemap.ClearAllTiles();

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Color pixelColor = currentLevel.GetPixelColor(x, y);

                    // Only show if pixel has color (alpha > 0)
                    if (pixelColor.a > 0.1f)
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);

                        // Get colored tile based on color
                        TileBase coloredTile = GetColoredTileFromColor(pixelColor);

                        if (coloredTile != null)
                        {
                            backgroundTilemap.SetTile(pos, coloredTile);

                            // Set alpha to 0.3 for semi-transparent effect
                            backgroundTilemap.SetColor(pos, new Color(pixelColor.r, pixelColor.g, pixelColor.b, 0.3f));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show target overlay (yellow highlight) if fillAllTiles = false (NEW)
        /// </summary>
        public void ShowTargetOverlay()
        {
            if (overlayTilemap == null || settings == null)
                return;

            overlayTilemap.ClearAllTiles();

            // Only show overlay if NOT filling all tiles
            if (settings.FillAllTiles)
                return;

            // Show target tiles as yellow overlay
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (targetMap[x, y])
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        overlayTilemap.SetTile(pos, targetTile);

                        // Set alpha to 0.5 for semi-transparent overlay
                        overlayTilemap.SetColor(pos, new Color(1f, 0.84f, 0f, 0.5f)); // Yellow with alpha
                    }
                }
            }
        }

        /// <summary>
        /// Get colored tile based on pixel color (NEW)
        /// Simple color matching - can be improved
        /// </summary>
        private TileBase GetColoredTileFromColor(Color color)
        {
            if (coloredTiles == null || coloredTiles.Length == 0)
                return filledTile; // Fallback to filled tile

            // Simple approach: use first colored tile
            // You can improve this by matching actual colors
            return coloredTiles[0];
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
        public LevelConfig CurrentLevel => currentLevel;
    }
}