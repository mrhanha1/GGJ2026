using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Settings;
using Game.Services.Input;
using Game.Core;

namespace PuzzleGame.Gameplay.Test
{
    /// <summary>
    /// Visual test controller for Phase 3 - Placement logic testing
    /// </summary>
    public class PuzzleTestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleBoard board;
        [SerializeField] private PieceFactory factory;
        [SerializeField] private Camera mainCamera;

        [Header("Test Controls")]
        [SerializeField] private PieceType selectedType = PieceType.OR;
        [SerializeField] private int selectedShapeIndex = 0;
        [SerializeField] private PieceShape[] availableShapes;

        [Header("Visual Settings")]
        [SerializeField] private GameObject previewCellPrefab;
        [SerializeField] private Color validColor = new Color(1f, 1f, 1f, 0.7f);
        [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.4f);
        [SerializeField] private Color andColor = new Color(0.5f, 0.5f, 1f, 1f);
        [SerializeField] private Color orColor = new Color(0.5f, 1f, 0.5f, 1f);
        [SerializeField] private Color notColor = new Color(1f, 0.5f, 0.5f, 1f);

        private PuzzlePiece currentPiece;
        private GameObject previewContainer;
        private SpriteRenderer[] previewCells;
        private Vector2Int currentGridPosition;
        private bool isPlacementValid;

        private IInputService inputService;

        private void Start()
        {
            inputService = ServiceLocator.Instance.Get<IInputService>();
            if (inputService == null)
            {
                Debug.LogError("[PuzzleTestController] InputService not found!");
                return;
            }

            // Create preview container
            previewContainer = new GameObject("PiecePreview");
            previewContainer.transform.SetParent(transform);

            // Load available shapes from factory if not assigned
            if (availableShapes == null || availableShapes.Length == 0)
            {
                Debug.LogWarning("[PuzzleTestController] No shapes assigned. Please assign shapes in Inspector.");
            }

            SpawnTestPiece();

            Debug.Log("[PuzzleTestController] Started. Controls:");
            Debug.Log("- Move mouse to preview placement");
            Debug.Log("- Left Click to place piece");
            Debug.Log("- Right Click or R to rotate piece");
            Debug.Log("- Use Inspector to change piece type/shape and spawn new piece");
        }

        private void Update()
        {
            if (inputService == null || currentPiece == null) return;

            // Get pointer position
            Vector2 pointerPosition = inputService.Puzzle.Point.ReadValue<Vector2>();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, 0));

            // Convert to grid position
            Vector3Int gridPos3D = board.WorldToGridPosition(worldPos);
            currentGridPosition = new Vector2Int(gridPos3D.x, gridPos3D.y);

            // Check if placement is valid
            isPlacementValid = board.CanPlacePiece(currentPiece, currentGridPosition);

            // Update preview
            UpdatePreview();

            // Handle rotate input (Right Click or R key)
            if (inputService.Puzzle.Submit.triggered) // You'll map this to Right Click or R
            {
                RotatePiece();
            }

            // Handle place input (Left Click)
            if (inputService.UI.Click.triggered)
            {
                TryPlacePiece();
            }
        }

        /// <summary>
        /// Spawn new test piece based on inspector settings
        /// </summary>
        [ContextMenu("Spawn Test Piece")]
        public void SpawnTestPiece()
        {
            if (availableShapes == null || availableShapes.Length == 0)
            {
                Debug.LogError("No shapes available!");
                return;
            }

            selectedShapeIndex = Mathf.Clamp(selectedShapeIndex, 0, availableShapes.Length - 1);
            PieceShape shape = availableShapes[selectedShapeIndex];

            currentPiece = new PuzzlePiece(shape, selectedType);
            CreatePreviewCells();

            Debug.Log($"[PuzzleTestController] Spawned piece: {shape.ShapeName} ({selectedType})");
        }

        /// <summary>
        /// Rotate current piece
        /// </summary>
        private void RotatePiece()
        {
            if (currentPiece == null) return;

            currentPiece.Rotate();
            CreatePreviewCells(); // Recreate cells after rotation

            Debug.Log($"[PuzzleTestController] Rotated to: {currentPiece.Rotation}");
        }

        /// <summary>
        /// Try to place current piece
        /// </summary>
        private void TryPlacePiece()
        {
            if (currentPiece == null) return;

            bool placed = board.PlacePiece(currentPiece, currentGridPosition);

            if (placed)
            {
                Debug.Log($"[PuzzleTestController] Placed {selectedType} piece at {currentGridPosition}");

                // Check if complete
                if (board.IsComplete())
                {
                    Debug.Log("========== LEVEL COMPLETE! ==========");
                }

                // Spawn new piece
                SpawnTestPiece();
            }
            else
            {
                Debug.LogWarning($"[PuzzleTestController] Cannot place piece at {currentGridPosition}");
            }
        }

        /// <summary>
        /// Create preview cells for current piece
        /// </summary>
        private void CreatePreviewCells()
        {
            // Clear old preview
            if (previewCells != null)
            {
                foreach (var cell in previewCells)
                {
                    if (cell != null) Destroy(cell.gameObject);
                }
            }

            var cells = currentPiece.GetOccupiedCells();
            previewCells = new SpriteRenderer[cells.Count];

            for (int i = 0; i < cells.Count; i++)
            {
                GameObject cellObj = new GameObject($"PreviewCell_{i}");
                cellObj.transform.SetParent(previewContainer.transform);

                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.sortingOrder = 10;

                // Set color based on piece type
                Color baseColor = GetTypeColor(currentPiece.Type);
                sr.color = baseColor;

                previewCells[i] = sr;
            }
        }

        /// <summary>
        /// Update preview position and color
        /// </summary>
        private void UpdatePreview()
        {
            if (previewCells == null || currentPiece == null) return;

            var cells = currentPiece.GetOccupiedCells();
            Color displayColor = isPlacementValid ? validColor : invalidColor;

            for (int i = 0; i < previewCells.Length && i < cells.Count; i++)
            {
                Vector2Int cellPos = cells[i] + currentGridPosition;
                Vector3 worldPos = board.GridToWorldPosition(new Vector3Int(cellPos.x, cellPos.y, 0));

                previewCells[i].transform.position = worldPos;

                // Blend type color with valid/invalid color
                Color baseColor = GetTypeColor(currentPiece.Type);
                previewCells[i].color = Color.Lerp(baseColor, displayColor, 0.5f);
            }
        }

        /// <summary>
        /// Get color for piece type
        /// </summary>
        private Color GetTypeColor(PieceType type)
        {
            switch (type)
            {
                case PieceType.AND: return andColor;
                case PieceType.OR: return orColor;
                case PieceType.NOT: return notColor;
                default: return Color.white;
            }
        }

        /// <summary>
        /// Create a simple square sprite
        /// </summary>
        private Sprite CreateSquareSprite()
        {
            Texture2D tex = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        }

        private void OnDestroy()
        {
            if (previewContainer != null)
                Destroy(previewContainer);
        }

        // Inspector buttons
        [ContextMenu("Toggle Require Fully Inside")]
        public void ToggleRequireFullyInside()
        {
            if (board.Settings != null)
            {
                bool current = board.Settings.RequireFullyInside;
                board.Settings.SetRequireFullyInside(!current);
                Debug.Log($"[PuzzleTestController] Require Fully Inside: {!current}");
            }
        }

        [ContextMenu("Toggle Fill All Tiles")]
        public void ToggleFillAllTiles()
        {
            if (board.Settings != null)
            {
                bool current = board.Settings.FillAllTiles;
                board.Settings.SetFillAllTiles(!current);
                Debug.Log($"[PuzzleTestController] Fill All Tiles: {!current}");
            }
        }

        [ContextMenu("Reset Board")]
        public void ResetBoard()
        {
            board.InitializeBoard();
            Debug.Log("[PuzzleTestController] Board reset");
        }
    }
}