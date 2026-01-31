using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Visual;
using PuzzleGame.Gameplay.Input;
using Game.Services.Audio;
using Game.Core;

namespace PuzzleGame.UI.Gameplay
{
    /// <summary>
    /// UI slot for inventory piece with drag and drop support
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Visual")]
        [SerializeField] private PieceVisual visual;
        [SerializeField] private Image border;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private float dragAlpha = 0.5f;

        [Header("Audio")]
        [SerializeField] private AudioClip pickupSFX;

        // References
        private PuzzleInputHandler inputHandler;
        private IAudioService audioService;

        // State
        private int slotIndex;
        private PuzzlePiece currentPiece;

        private void Awake()
        {
            if (visual == null)
                visual = GetComponentInChildren<PieceVisual>();

            if (border == null)
                border = GetComponent<Image>();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            // Get audio service
            audioService = ServiceLocator.Instance.Get<IAudioService>();
        }

        /// <summary>
        /// Initialize slot with index and input handler
        /// </summary>
        public void Initialize(int index, PuzzleInputHandler handler)
        {
            slotIndex = index;
            inputHandler = handler;
        }

        /// <summary>
        /// Set piece to display in this slot
        /// </summary>
        public void SetPiece(PuzzlePiece piece)
        {
            currentPiece = piece;

            if (visual != null && piece != null)
            {
                visual.GenerateVisual(piece);
                visual.gameObject.SetActive(true);
            }
            else if (visual != null)
            {
                visual.ClearVisual();
                visual.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Set selected state (highlight border)
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (border != null)
            {
                border.color = selected ? selectedColor : normalColor;
            }
        }

        /// <summary>
        /// Called when drag begins
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (currentPiece == null || inputHandler == null)
                return;

            // Play pickup sound
            if (audioService != null && pickupSFX != null)
            {
                audioService.PlaySFX(pickupSFX);
            }

            // Set alpha to show dragging state
            if (canvasGroup != null)
            {
                canvasGroup.alpha = dragAlpha;
            }

            // Notify input handler
            inputHandler.OnSlotBeginDrag(slotIndex);

            Debug.Log($"[InventorySlotUI] Begin drag slot {slotIndex}");
        }

        /// <summary>
        /// Called during drag
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (inputHandler != null)
            {
                inputHandler.OnSlotDrag();
            }
        }

        /// <summary>
        /// Called when drag ends
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // Reset alpha
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            // Notify input handler
            if (inputHandler != null)
            {
                inputHandler.OnSlotEndDrag();
            }

            Debug.Log($"[InventorySlotUI] End drag slot {slotIndex}");
        }

        // Getters
        public int SlotIndex => slotIndex;
        public PuzzlePiece CurrentPiece => currentPiece;
    }
}