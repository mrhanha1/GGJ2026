using UnityEngine;
using System.Collections.Generic;

namespace PuzzleGame.Gameplay.Pieces
{
    public class PieceFactory : MonoBehaviour
    {
        [Header("Available Shapes")]
        [SerializeField] private List<PieceShape> availableShapes = new List<PieceShape>();

        /// <summary>
        /// Create a random piece with random shape and type
        /// </summary>
        public PuzzlePiece CreateRandomPiece()
        {
            if (availableShapes == null || availableShapes.Count == 0)
            {
                Debug.LogError("No available shapes in PieceFactory!");
                return null;
            }

            // Random shape
            PieceShape randomShape = availableShapes[Random.Range(0, availableShapes.Count)];

            // Random type
            PieceType randomType = (PieceType)Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);

            return new PuzzlePiece(randomShape, randomType);
        }

        /// <summary>
        /// Create piece with specific shape and random type
        /// </summary>
        public PuzzlePiece CreatePiece(PieceShape shape)
        {
            PieceType randomType = (PieceType)Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);
            return new PuzzlePiece(shape, randomType);
        }

        /// <summary>
        /// Create piece with specific shape and type
        /// </summary>
        public PuzzlePiece CreatePiece(PieceShape shape, PieceType type)
        {
            return new PuzzlePiece(shape, type);
        }

        /// <summary>
        /// Add shape to available shapes
        /// </summary>
        public void AddShape(PieceShape shape)
        {
            if (!availableShapes.Contains(shape))
            {
                availableShapes.Add(shape);
            }
        }

        /// <summary>
        /// Remove shape from available shapes
        /// </summary>
        public void RemoveShape(PieceShape shape)
        {
            availableShapes.Remove(shape);
        }
    }
}