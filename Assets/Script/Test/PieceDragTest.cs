using UnityEngine;
using PuzzleGame.Gameplay.Pieces;
using PuzzleGame.Gameplay.Input;
using Game.Services.Input;

using Game.Core;

namespace PuzzleGame.Gameplay.Test
{
    public class PieceDragTest : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PieceFactory factory;
        [SerializeField] private PieceDragHandler dragHandler;

        private IInputService inputService;

        private void Start()
        {
            inputService = ServiceLocator.Instance.Get<IInputService>();

            if (inputService == null)
            {
                Debug.LogError("InputService not found!");
                return;
            }

            // Subscribe to Puzzle.Piece input (bạn sẽ tạo sau)
            inputService.Puzzle.Piece.performed += OnPieceInputPerformed;

            // Initialize dragHandler with inputService
            dragHandler.Initialize(inputService);
        }

        private void OnPieceInputPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // Tạo random piece và bắt đầu drag
            var piece = factory.CreateRandomPiece();
            dragHandler.OnBeginDrag(0, piece);
        }

        private void OnDestroy()
        {
            if (inputService != null)
            {
                inputService.Puzzle.Piece.performed -= OnPieceInputPerformed;
            }
        }
    }
}