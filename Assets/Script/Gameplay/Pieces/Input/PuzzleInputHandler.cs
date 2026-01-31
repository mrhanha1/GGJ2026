using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Inventory;
using PuzzleGame.Gameplay.Input;
using Game.Services.Input;
using Game.Services.Audio;
using Game.Core;

namespace PuzzleGame.Gameplay.Input
{
    /// <summary>
    /// Handles puzzle input and connects drag handler with inventory
    /// </summary>
    public class PuzzleInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PuzzleBoard board;
        [SerializeField] private PieceInventory inventory;
        [SerializeField] private PieceDragHandler dragHandler;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip placePieceSFX;
        [SerializeField] private AudioClip invalidPlaceSFX;

        // Services
        private IInputService inputService;
        private IAudioService audioService;

        // State
        private int selectedSlot = -1;

        private void Start()
        {
            // Get services
            inputService = ServiceLocator.Instance.Get<IInputService>();
            audioService = ServiceLocator.Instance.Get<IAudioService>();

            if (inputService == null)
            {
                Debug.LogError("[PuzzleInputHandler] InputService not found!");
                return;
            }

            // Initialize drag handler with input service
            if (dragHandler != null)
            {
                dragHandler.Initialize(inputService);

                // Subscribe to drag handler events
                dragHandler.OnSuccessfulPlacement.AddListener(OnPiecePlaced);
                dragHandler.OnFailedPlacement.AddListener(OnPlaceFailed);
            }

            Debug.Log("[PuzzleInputHandler] Initialized");
        }

        /// <summary>
        /// Called when user begins dragging from a slot
        /// </summary>
        public void OnSlotBeginDrag(int slotIndex)
        {
            if (inventory == null || dragHandler == null)
            {
                Debug.LogWarning("[PuzzleInputHandler] Missing references!");
                return;
            }

            var piece = inventory.GetPiece(slotIndex);
            if (piece == null)
            {
                Debug.LogWarning($"[PuzzleInputHandler] No piece in slot {slotIndex}");
                return;
            }

            selectedSlot = slotIndex;
            dragHandler.OnBeginDrag(slotIndex, piece);
            Debug.Log($"[PuzzleInputHandler] Started dragging piece from slot {slotIndex}");
        }

        /// <summary>
        /// Called during drag
        /// </summary>
        public void OnSlotDrag()
        {
            if (dragHandler != null && dragHandler.IsDragging)
            {
                dragHandler.OnDrag();
            }
        }

        /// <summary>
        /// Called when drag ends
        /// </summary>
        public void OnSlotEndDrag()
        {
            if (dragHandler != null && dragHandler.IsDragging)
            {
                dragHandler.OnEndDrag();
            }
        }

        /// <summary>
        /// Called when piece is successfully placed on board
        /// </summary>
        private void OnPiecePlaced(int slotIndex, Vector2Int gridPos)
        {
            if (board == null || inventory == null)
                return;

            // Get the piece from inventory
            var piece = inventory.GetPiece(slotIndex);
            if (piece == null)
            {
                Debug.LogWarning("[PuzzleInputHandler] Piece not found in inventory!");
                return;
            }

            // Place piece on board
            bool placed = board.PlacePiece(piece, gridPos);

            if (placed)
            {
                // Remove from inventory (will auto-refill)
                inventory.RemovePiece(slotIndex);

                // Play success SFX
                if (audioService != null && placePieceSFX != null)
                {
                    audioService.PlaySFX(placePieceSFX);
                }

                Debug.Log($"[PuzzleInputHandler] Piece placed at {gridPos}");

                // Check win condition
                if (board.IsComplete())
                {
                    Debug.Log("[PuzzleInputHandler] Level Complete!");
                    // TODO: Trigger win event (Phase 8)
                }
            }
            else
            {
                Debug.LogWarning("[PuzzleInputHandler] Failed to place piece on board!");
                OnPlaceFailed();
            }

            selectedSlot = -1;
        }

        /// <summary>
        /// Called when piece placement failed
        /// </summary>
        private void OnPlaceFailed()
        {
            // Play fail SFX
            if (audioService != null && invalidPlaceSFX != null)
            {
                audioService.PlaySFX(invalidPlaceSFX);
            }

            Debug.Log("[PuzzleInputHandler] Invalid placement");
            selectedSlot = -1;
        }

        private void OnDestroy()
        {
            // Unsubscribe events
            if (dragHandler != null)
            {
                dragHandler.OnSuccessfulPlacement.RemoveListener(OnPiecePlaced);
                dragHandler.OnFailedPlacement.RemoveListener(OnPlaceFailed);
            }
        }

        // Public getters
        public int SelectedSlot => selectedSlot;
    }
}