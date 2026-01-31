using UnityEngine;
using System.Collections.Generic;
using PuzzleGame.Gameplay.Pieces;

namespace PuzzleGame.Gameplay.Visual
{
    /// <summary>
    /// Visual representation of a puzzle piece using SpriteRenderers (for world space drag)
    /// </summary>
    public class PieceVisualWorld : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Sprite cellSprite; // Square sprite - assign in Inspector
        [SerializeField] private float cellSize = 1f; // World units

        [Header("Type Colors")]
        [SerializeField] private Color andColor = new Color(0.5f, 0.5f, 1f, 1f); // Blue
        [SerializeField] private Color orColor = new Color(0.5f, 1f, 0.5f, 1f); // Green
        [SerializeField] private Color notColor = new Color(1f, 0.5f, 0.5f, 1f); // Red

        [Header("Validity Colors")]
        [SerializeField] private Color validColor = new Color(1f, 1f, 1f, 0.7f); // White transparent
        [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.4f); // Red transparent

        private List<SpriteRenderer> cellRenderers = new List<SpriteRenderer>();
        private Color baseColor;

        /// <summary>
        /// Generate visual representation of piece
        /// </summary>
        public void GenerateVisual(PuzzlePiece piece)
        {
            // Clear existing cells
            ClearVisual();

            // Get piece color based on type
            baseColor = GetTypeColor(piece.Type);

            // Get occupied cells
            var cells = piece.GetOccupiedCells();

            // Create sprite renderer for each cell
            foreach (var cell in cells)
            {
                GameObject cellObj = new GameObject($"Cell_{cell.x}_{cell.y}");
                cellObj.transform.SetParent(transform);
                cellObj.transform.localPosition = new Vector3(cell.x * cellSize, cell.y * cellSize, 0);

                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite = cellSprite;
                sr.color = baseColor;
                sr.sortingLayerName = "Default";
                sr.sortingOrder = 10; // Above board tiles

                cellRenderers.Add(sr);
            }
        }

        /// <summary>
        /// Set alpha transparency for all cells
        /// </summary>
        public void SetAlpha(float alpha)
        {
            foreach (var renderer in cellRenderers)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }

        /// <summary>
        /// Set tint color for validity feedback
        /// </summary>
        public void SetTint(Color tint)
        {
            foreach (var renderer in cellRenderers)
            {
                renderer.color = tint;
            }
        }

        /// <summary>
        /// Set valid placement visual
        /// </summary>
        public void SetValidVisual()
        {
            SetTint(validColor);
        }

        /// <summary>
        /// Set invalid placement visual
        /// </summary>
        public void SetInvalidVisual()
        {
            SetTint(invalidColor);
        }

        /// <summary>
        /// Reset to base color
        /// </summary>
        public void ResetVisual()
        {
            SetTint(baseColor);
        }

        /// <summary>
        /// Clear all visual cells
        /// </summary>
        public void ClearVisual()
        {
            foreach (var renderer in cellRenderers)
            {
                if (renderer != null)
                    Destroy(renderer.gameObject);
            }
            cellRenderers.Clear();
        }

        /// <summary>
        /// Get color based on piece type
        /// </summary>
        private Color GetTypeColor(PieceType type)
        {
            switch (type)
            {
                case PieceType.AND:
                    return andColor;
                case PieceType.OR:
                    return orColor;
                case PieceType.NOT:
                    return notColor;
                default:
                    return Color.white;
            }
        }

        private void OnDestroy()
        {
            ClearVisual();
        }
    }
}