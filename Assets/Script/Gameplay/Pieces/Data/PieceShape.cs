using UnityEngine;
using System.Collections.Generic;

namespace PuzzleGame.Gameplay.Pieces
{
    [CreateAssetMenu(fileName = "New Piece Shape", menuName = "Puzzle Game/Piece Shape")]
    public class PieceShape : ScriptableObject
    {
        [SerializeField] private string shapeName;
        [SerializeField] private int width = 12;
        [SerializeField] private int height = 12;

        // Shape data stored as serialized array for Inspector visibility
        [SerializeField] private bool[] shapeData;

        private void OnEnable()
        {
            // Initialize array if null
            if (shapeData == null)
            {
                shapeData = new bool[width * height];
            }
        }

        private void OnValidate()
        {
            // Ensure array size matches width * height
            int requiredSize = width * height;
            if (shapeData == null || shapeData.Length != requiredSize)
            {
                bool[] newData = new bool[requiredSize];
                if (shapeData != null)
                {
                    int copyLength = Mathf.Min(shapeData.Length, requiredSize);
                    System.Array.Copy(shapeData, newData, copyLength);
                }
                shapeData = newData;
            }
        }

        /// <summary>
        /// Get shape as 2D array
        /// </summary>
        public bool[,] GetShape()
        {
            bool[,] shape = new bool[width, height];

            if (shapeData == null)
                return shape;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    shape[x, y] = shapeData[y * width + x];
                }
            }
            return shape;
        }

        /// <summary>
        /// Set shape from 2D array
        /// </summary>
        public void SetShape(bool[,] shape)
        {
            width = shape.GetLength(0);
            height = shape.GetLength(1);
            shapeData = new bool[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    shapeData[y * width + x] = shape[x, y];
                }
            }
        }

        /// <summary>
        /// Get cell value at position
        /// </summary>
        public bool GetCell(int x, int y)
        {
            if (shapeData == null || x < 0 || x >= width || y < 0 || y >= height)
                return false;
            return shapeData[y * width + x];
        }

        /// <summary>
        /// Set cell value at position
        /// </summary>
        public void SetCell(int x, int y, bool value)
        {
            if (shapeData == null || x < 0 || x >= width || y < 0 || y >= height)
                return;
            shapeData[y * width + x] = value;
        }

        /// <summary>
        /// Get list of occupied cells relative to origin (0,0)
        /// </summary>
        public List<Vector2Int> GetOccupiedCells()
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            if (shapeData == null)
                return cells;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (shapeData[y * width + x])
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }
            return cells;
        }

        /// <summary>
        /// Get rotated shape (90 degrees clockwise)
        /// </summary>
        public bool[,] GetRotatedShape()
        {
            bool[,] rotated = new bool[height, width]; // Swap dimensions

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Rotate 90 degrees clockwise: (x,y) -> (height-1-y, x)
                    rotated[height - 1 - y, x] = shapeData[y * width + x];
                }
            }

            return rotated;
        }

        /// <summary>
        /// Resize shape
        /// </summary>
        public void Resize(int newWidth, int newHeight)
        {
            bool[,] oldShape = GetShape();
            width = newWidth;
            height = newHeight;
            shapeData = new bool[width * height];

            // Copy old data
            for (int x = 0; x < Mathf.Min(oldShape.GetLength(0), width); x++)
            {
                for (int y = 0; y < Mathf.Min(oldShape.GetLength(1), height); y++)
                {
                    shapeData[y * width + x] = oldShape[x, y];
                }
            }
        }

        /// <summary>
        /// Clear all cells
        /// </summary>
        public void Clear()
        {
            if (shapeData == null)
                shapeData = new bool[width * height];

            for (int i = 0; i < shapeData.Length; i++)
            {
                shapeData[i] = false;
            }
        }

        /// <summary>
        /// Fill all cells
        /// </summary>
        public void Fill()
        {
            if (shapeData == null)
                shapeData = new bool[width * height];

            for (int i = 0; i < shapeData.Length; i++)
            {
                shapeData[i] = true;
            }
        }

        // Getters
        public string ShapeName => shapeName;
        public int Width => width;
        public int Height => height;
    }
}