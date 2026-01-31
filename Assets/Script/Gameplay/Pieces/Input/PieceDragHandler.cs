using UnityEngine;
using UnityEngine.Events;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Visual;
using Game.Services.Input;

namespace PuzzleGame.Gameplay.Input
{
    /// <summary>
    /// Handles dragging and dropping puzzle pieces with validity checking
    /// </summary>
    public class PieceDragHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PieceVisualWorld visual;
        [SerializeField] private PuzzleBoard board;
        [SerializeField] private Camera mainCamera;

        [Header("Drag Settings")]
        [SerializeField] private float snapThreshold = 0.3f; // Distance to snap to grid

        // Events
        public UnityEvent<int, Vector2Int> OnSuccessfulPlacement;
        public UnityEvent OnFailedPlacement;

        // State
        private bool isDragging = false;
        private PuzzlePiece currentPiece;
        private int sourceSlotIndex = -1;
        private Vector3Int currentGridPosition;
        private IInputService inputService;

        private void Awake()
        {
            if (visual == null)
                visual = GetComponent<PieceVisualWorld>();

            if (mainCamera == null)
                mainCamera = Camera.main;

            // Initialize events
            if (OnSuccessfulPlacement == null)
                OnSuccessfulPlacement = new UnityEvent<int, Vector2Int>();
            if (OnFailedPlacement == null)
                OnFailedPlacement = new UnityEvent();
        }

        /// <summary>
        /// Initialize with InputService reference
        /// </summary>
        public void Initialize(IInputService input)
        {
            inputService = input;
        }

        /// <summary>
        /// Begin dragging a piece
        /// </summary>
        public void OnBeginDrag(int slotIndex, PuzzlePiece piece)
        {
            if (piece == null || board == null)
            {
                Debug.LogWarning("Cannot begin drag: piece or board is null");
                return;
            }

            isDragging = true;
            currentPiece = piece;
            sourceSlotIndex = slotIndex;

            // Generate visual
            visual.GenerateVisual(piece);
            gameObject.SetActive(true);

            // Update position immediately
            UpdatePosition();
        }

        /// <summary>
        /// Update drag position
        /// </summary>
        public void OnDrag()
        {
            if (!isDragging) return;

            UpdatePosition();
        }

        /// <summary>
        /// End drag and try to place piece
        /// </summary>
        public void OnEndDrag()
        {
            if (!isDragging) return;

            // Check if placement is valid
            Vector2Int gridPos = new Vector2Int(currentGridPosition.x, currentGridPosition.y);
            bool canPlace = board.CanPlacePiece(currentPiece, gridPos);

            if (canPlace)
            {
                // Successful placement
                OnSuccessfulPlacement?.Invoke(sourceSlotIndex, gridPos);
            }
            else
            {
                // Failed placement
                OnFailedPlacement?.Invoke();
            }

            // Reset state
            isDragging = false;
            currentPiece = null;
            sourceSlotIndex = -1;
            gameObject.SetActive(false);
            visual.ClearVisual();
        }

        /// <summary>
        /// Update position and visual feedback
        /// </summary>
        private void UpdatePosition()
        {
            // Get pointer world position
            Vector3 worldPos = GetPointerWorldPosition();

            // Snap to grid
            Vector3Int gridPos = GetGridPosition(worldPos);
            currentGridPosition = gridPos;

            // Update transform position to grid center
            transform.position = board.GridToWorldPosition(gridPos);

            // Update validity visual
            UpdateValidityVisual();
        }

        /// <summary>
        /// Update visual based on placement validity
        /// </summary>
        private void UpdateValidityVisual()
        {
            Vector2Int gridPos = new Vector2Int(currentGridPosition.x, currentGridPosition.y);
            bool canPlace = board.CanPlacePiece(currentPiece, gridPos);

            if (canPlace)
            {
                visual.ResetVisual();
            }
            else
            {
                visual.SetInvalidVisual();
            }
        }

        /// <summary>
        /// Get pointer world position (works for both mouse and touch)
        /// </summary>
        private Vector3 GetPointerWorldPosition()
        {
            Vector3 screenPos;

            // Check for touch input first (mobile)
            if (UnityEngine.Input.touchCount > 0)
            {
                screenPos = UnityEngine.Input.GetTouch(0).position;
            }
            // Fallback to mouse (PC)
            else if (inputService != null)
            {
                Vector2 mousePos = inputService.Puzzle.Point.ReadValue<Vector2>();
                screenPos = new Vector3(mousePos.x, mousePos.y, 0);
            }
            else
            {
                screenPos = UnityEngine.Input.mousePosition;
            }

            // Convert to world position
            screenPos.z = Mathf.Abs(mainCamera.transform.position.z);
            return mainCamera.ScreenToWorldPoint(screenPos);
        }

        /// <summary>
        /// Convert world position to grid position
        /// </summary>
        private Vector3Int GetGridPosition(Vector3 worldPos)
        {
            return board.WorldToGridPosition(worldPos);
        }

        /// <summary>
        /// Cancel current drag
        /// </summary>
        public void CancelDrag()
        {
            if (!isDragging) return;

            isDragging = false;
            currentPiece = null;
            sourceSlotIndex = -1;
            gameObject.SetActive(false);
            visual.ClearVisual();

            OnFailedPlacement?.Invoke();
        }

        // Public getters
        public bool IsDragging => isDragging;
        public PuzzlePiece CurrentPiece => currentPiece;
        public int SourceSlotIndex => sourceSlotIndex;
    }
}