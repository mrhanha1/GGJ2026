using UnityEngine;
using PuzzleGame.Gameplay.Inventory;
using PuzzleGame.Gameplay.Input;

namespace PuzzleGame.UI.Gameplay
{
    /// <summary>
    /// Manages inventory UI display with 3 slots
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventorySlotUI[] slotUIs;
        [SerializeField] private PieceInventory inventory;
        [SerializeField] private PuzzleInputHandler inputHandler;

        private void Start()
        {
            if (inventory == null)
            {
                Debug.LogError("[InventoryUI] PieceInventory not assigned!");
                return;
            }

            if (inputHandler == null)
            {
                Debug.LogError("[InventoryUI] PuzzleInputHandler not assigned!");
                return;
            }

            // Initialize slots
            if (slotUIs != null)
            {
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    if (slotUIs[i] != null)
                    {
                        slotUIs[i].Initialize(i, inputHandler);
                    }
                }
            }

            // Subscribe to inventory changes
            inventory.onInventoryChanged.AddListener(UpdateDisplay);

            // Initial display update
            UpdateDisplay();

            Debug.Log("[InventoryUI] Initialized with " + slotUIs.Length + " slots");
        }

        /// <summary>
        /// Update all slot displays
        /// </summary>
        public void UpdateDisplay()
        {
            if (inventory == null || slotUIs == null)
                return;

            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null)
                {
                    var piece = inventory.GetPiece(i);
                    slotUIs[i].SetPiece(piece);
                }
            }

            Debug.Log("[InventoryUI] Display updated");
        }

        /// <summary>
        /// Highlight selected slot
        /// </summary>
        public void SetSelectedSlot(int index)
        {
            if (slotUIs == null)
                return;

            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null)
                {
                    slotUIs[i].SetSelected(i == index);
                }
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (inventory != null)
            {
                inventory.onInventoryChanged.RemoveListener(UpdateDisplay);
            }
        }
    }
}