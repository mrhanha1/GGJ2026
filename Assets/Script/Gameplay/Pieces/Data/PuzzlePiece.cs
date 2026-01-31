using UnityEngine;
using System.Collections.Generic;

namespace PuzzleGame.Gameplay.Pieces
{
    [System.Serializable]
    public class PuzzlePiece
    {
        [SerializeField] private PieceShape shape;
        [SerializeField] private PieceType type;
        [SerializeField] private int rotation; // 0, 1, 2, 3 (90 degree increments)

        private bool[,] currentShape;

        public PuzzlePiece(PieceShape shape, PieceType type)
        {
            this.shape = shape;
            this.type = type;
            this.rotation = 0;
            UpdateCurrentShape();
        }

        /// <summary>
        /// Rotate piece 90 degrees clockwise
        /// </summary>
        public void Rotate()
        {
            rotation = (rotation + 1) % 4;
            UpdateCurrentShape();
        }

        /// <summary>
        /// Set rotation to specific angle (0-3)
        /// </summary>
        public void SetRotation(int rot)
        {
            rotation = Mathf.Clamp(rot, 0, 3);
            UpdateCurrentShape();
        }

        /// <summary>
        /// Update current shape based on rotation
        /// </summary>
        private void UpdateCurrentShape()
        {
            currentShape = shape.GetShape();

            // Apply rotation
            for (int i = 0; i < rotation; i++)
            {
                currentShape = RotateArray(currentShape);
            }
        }

        /// <summary>
        /// Rotate 2D array 90 degrees clockwise
        /// </summary>
        private bool[,] RotateArray(bool[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            bool[,] rotated = new bool[height, width];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    rotated[height - 1 - y, x] = array[x, y];
                }
            }

            return rotated;
        }

        /// <summary>
        /// Get occupied cells with current rotation
        /// </summary>
        public List<Vector2Int> GetOccupiedCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            int width = currentShape.GetLength(0);
            int height = currentShape.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (currentShape[x, y])
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }

            return cells;
        }

        /// <summary>
        /// Get occupied cells at specific world position
        /// </summary>
        public List<Vector2Int> GetOccupiedCells(Vector2Int offset)
        {
            List<Vector2Int> cells = GetOccupiedCells();
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i] += offset;
            }
            return cells;
        }

        // Getters
        public PieceShape Shape => shape;
        public PieceType Type => type;
        public int Rotation => rotation;
        public int Width => currentShape.GetLength(0);
        public int Height => currentShape.GetLength(1);
        public bool[,] CurrentShape => currentShape;
    }
}