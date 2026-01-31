using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using PuzzleGame.Gameplay.Pieces;

namespace PuzzleGame.Gameplay.Inventory
{
    /// <summary>
    /// Manages inventory of 3 puzzle pieces with auto-refill
    /// </summary>
    public class PieceInventory : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxSlots = 3;

        [Header("References")]
        [SerializeField] private PieceFactory factory;

        // Events
        public UnityEvent onInventoryChanged;

        // State
        private List<PuzzlePiece> slots = new List<PuzzlePiece>();

        private void Awake()
        {
            // Initialize event
            if (onInventoryChanged == null)
                onInventoryChanged = new UnityEvent();
        }

        private void Start()
        {
            RefillInventory();
        }

        /// <summary>
        /// Fill all empty slots with random pieces
        /// </summary>
        public void RefillInventory()
        {
            if (factory == null)
            {
                Debug.LogError("PieceFactory not assigned!");
                return;
            }

            // Fill up to maxSlots
            while (slots.Count < maxSlots)
            {
                PuzzlePiece newPiece = factory.CreateRandomPiece();
                slots.Add(newPiece);
            }

            onInventoryChanged?.Invoke();
            Debug.Log($"[PieceInventory] Refilled inventory. Current slots: {slots.Count}");
        }

        /// <summary>
        /// Get piece at slot index
        /// </summary>
        public PuzzlePiece GetPiece(int index)
        {
            if (index < 0 || index >= slots.Count)
            {
                Debug.LogWarning($"Invalid slot index: {index}");
                return null;
            }

            return slots[index];
        }

        /// <summary>
        /// Remove piece at slot index and refill
        /// </summary>
        public void RemovePiece(int index)
        {
            if (index < 0 || index >= slots.Count)
            {
                Debug.LogWarning($"Invalid slot index: {index}");
                return;
            }

            slots.RemoveAt(index);
            RefillInventory();
        }

        /// <summary>
        /// Check if slot has a piece
        /// </summary>
        public bool HasPiece(int index)
        {
            return index >= 0 && index < slots.Count && slots[index] != null;
        }

        /// <summary>
        /// Get current number of pieces
        /// </summary>
        public int GetPieceCount()
        {
            return slots.Count;
        }

        /// <summary>
        /// Clear all pieces
        /// </summary>
        public void ClearInventory()
        {
            slots.Clear();
            onInventoryChanged?.Invoke();
        }

        // Getters
        public int MaxSlots => maxSlots;
        public List<PuzzlePiece> Slots => slots;
    }
}