using UnityEngine;
using PuzzleGame.Gameplay.Board;
using PuzzleGame.Gameplay.Pieces;

public class PuzzleTestManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PuzzleBoard board;
    [SerializeField] private PieceFactory factory;

    [Header("Test Controls")]
    [SerializeField] private bool runTest = false;
    [SerializeField] private int testNumber = 1;

    private void Update()
    {
        if (runTest)
        {
            runTest = false;
            RunTest(testNumber);
        }
    }

    private void RunTest(int testNum)
    {
        Debug.Log($"========== RUNNING TEST {testNum} ==========");

        switch (testNum)
        {
            case 1: Test_Phase1_InitializeBoard(); break;
            case 2: Test_Phase1_SetTileValue(); break;
            case 3: Test_Phase1_GetTileValue(); break;
            case 4: Test_Phase1_TargetMap(); break;
            case 5: Test_Phase1_IsComplete(); break;
            case 6: Test_Phase1_GridWorldConversion(); break;
            case 7: Test_Phase2_CreateRandomPiece(); break;
            case 8: Test_Phase2_PieceRotation(); break;
            case 9: Test_Phase2_GetOccupiedCells(); break;
            case 10: Test_Phase2_MultipleRotations(); break;
            default: Debug.LogError("Invalid test number!"); break;
        }
    }

    // ========== PHASE 1 TESTS ==========

    private void Test_Phase1_InitializeBoard()
    {
        Debug.Log("TEST 1: Initialize Board");
        board.InitializeBoard();
        Debug.Log("? Board initialized - Check scene view for 15x15 gray tiles");
        Debug.Log($"? Board size: {board.BoardWidth}x{board.BoardHeight}");
    }

    private void Test_Phase1_SetTileValue()
    {
        Debug.Log("TEST 2: Set Tile Value");

        // Set some tiles to filled
        board.SetTileValue(new Vector3Int(0, 0, 0), true);
        board.SetTileValue(new Vector3Int(5, 5, 0), true);
        board.SetTileValue(new Vector3Int(10, 10, 0), true);
        board.SetTileValue(new Vector3Int(14, 14, 0), true);

        Debug.Log("? Set tiles (0,0), (5,5), (10,10), (14,14) to FILLED");
        Debug.Log("? Check scene view - these tiles should be BLUE");

        // Set one back to empty
        board.SetTileValue(new Vector3Int(5, 5, 0), false);
        Debug.Log("? Set tile (5,5) back to EMPTY - should be GRAY again");
    }

    private void Test_Phase1_GetTileValue()
    {
        Debug.Log("TEST 3: Get Tile Value");

        bool tile1 = board.GetTileValue(new Vector3Int(0, 0, 0));
        bool tile2 = board.GetTileValue(new Vector3Int(5, 5, 0));
        bool tile3 = board.GetTileValue(new Vector3Int(10, 10, 0));

        Debug.Log($"? Tile (0,0): {tile1} (should be TRUE)");
        Debug.Log($"? Tile (5,5): {tile2} (should be FALSE after reset)");
        Debug.Log($"? Tile (10,10): {tile3} (should be TRUE)");

        // Test invalid position
        bool invalidTile = board.GetTileValue(new Vector3Int(20, 20, 0));
        Debug.Log($"? Tile (20,20) out of bounds: {invalidTile} (should be FALSE)");
    }

    private void Test_Phase1_TargetMap()
    {
        Debug.Log("TEST 4: Target Map");

        // Create test target map
        bool[,] testTarget = new bool[15, 15];
        testTarget[0, 0] = true;
        testTarget[1, 1] = true;
        testTarget[2, 2] = true;
        testTarget[7, 7] = true; // center

        board.SetTargetMap(testTarget);

        Debug.Log($"? IsTarget (0,0): {board.IsTargetTile(new Vector3Int(0, 0, 0))} (should be TRUE)");
        Debug.Log($"? IsTarget (1,1): {board.IsTargetTile(new Vector3Int(1, 1, 0))} (should be TRUE)");
        Debug.Log($"? IsTarget (7,7): {board.IsTargetTile(new Vector3Int(7, 7, 0))} (should be TRUE)");
        Debug.Log($"? IsTarget (5,5): {board.IsTargetTile(new Vector3Int(5, 5, 0))} (should be FALSE)");
    }

    private void Test_Phase1_IsComplete()
    {
        Debug.Log("TEST 5: Is Complete");

        board.InitializeBoard(); // Reset

        bool complete1 = board.IsComplete();
        Debug.Log($"? Empty board complete: {complete1} (should be FALSE)");

        // Fill all tiles
        for (int x = 0; x < board.BoardWidth; x++)
        {
            for (int y = 0; y < board.BoardHeight; y++)
            {
                board.SetTileValue(new Vector3Int(x, y, 0), true);
            }
        }

        bool complete2 = board.IsComplete();
        Debug.Log($"? Full board complete: {complete2} (should be TRUE)");
        Debug.Log("? Check scene view - all tiles should be BLUE");
    }

    private void Test_Phase1_GridWorldConversion()
    {
        Debug.Log("TEST 6: Grid <-> World Conversion");

        Vector3Int gridPos = new Vector3Int(5, 5, 0);
        Vector3 worldPos = board.GridToWorldPosition(gridPos);
        Vector3Int backToGrid = board.WorldToGridPosition(worldPos);

        Debug.Log($"? Grid (5,5) ? World {worldPos}");
        Debug.Log($"? World {worldPos} ? Grid {backToGrid}");
        Debug.Log($"? Conversion correct: {gridPos == backToGrid} (should be TRUE)");

        // Test corner positions
        Vector3 corner1 = board.GridToWorldPosition(new Vector3Int(0, 0, 0));
        Vector3 corner2 = board.GridToWorldPosition(new Vector3Int(14, 14, 0));
        Debug.Log($"? Corner (0,0) world pos: {corner1}");
        Debug.Log($"? Corner (14,14) world pos: {corner2}");
    }

    // ========== PHASE 2 TESTS ==========

    private void Test_Phase2_CreateRandomPiece()
    {
        Debug.Log("TEST 7: Create Random Piece");

        for (int i = 0; i < 5; i++)
        {
            PuzzlePiece piece = factory.CreateRandomPiece();
            Debug.Log($"? Piece {i + 1}: Shape={piece.Shape.ShapeName}, Type={piece.Type}, Size={piece.Width}x{piece.Height}");

            var cells = piece.GetOccupiedCells();
            Debug.Log($"  ? Occupied cells: {cells.Count}");
        }
    }

    private void Test_Phase2_PieceRotation()
    {
        Debug.Log("TEST 8: Piece Rotation");

        PuzzlePiece piece = factory.CreateRandomPiece();
        Debug.Log($"? Initial: Rotation={piece.Rotation}, Size={piece.Width}x{piece.Height}");

        var cellsBefore = piece.GetOccupiedCells();
        Debug.Log($"? Cells before rotate: {string.Join(", ", cellsBefore)}");

        piece.Rotate();
        Debug.Log($"? After 1 rotate: Rotation={piece.Rotation}, Size={piece.Width}x{piece.Height}");

        var cellsAfter = piece.GetOccupiedCells();
        Debug.Log($"? Cells after rotate: {string.Join(", ", cellsAfter)}");
        Debug.Log($"? Cell positions changed: {!AreCellsSame(cellsBefore, cellsAfter)} (should be TRUE if not square)");
    }

    private void Test_Phase2_GetOccupiedCells()
    {
        Debug.Log("TEST 9: Get Occupied Cells");

        PuzzlePiece piece = factory.CreateRandomPiece();
        var cells = piece.GetOccupiedCells();

        Debug.Log($"? Piece: {piece.Shape.ShapeName}");
        Debug.Log($"? Total occupied cells: {cells.Count}");
        Debug.Log($"? Cell positions:");
        foreach (var cell in cells)
        {
            Debug.Log($"  ? ({cell.x}, {cell.y})");
        }

        // Test with offset
        Vector2Int offset = new Vector2Int(5, 5);
        var offsetCells = piece.GetOccupiedCells(offset);
        Debug.Log($"? Cells with offset (5,5):");
        foreach (var cell in offsetCells)
        {
            Debug.Log($"  ? ({cell.x}, {cell.y})");
        }
    }

    private void Test_Phase2_MultipleRotations()
    {
        Debug.Log("TEST 10: Multiple Rotations (Full 360°)");

        PuzzlePiece piece = factory.CreateRandomPiece();
        var initialCells = piece.GetOccupiedCells();

        Debug.Log($"? Initial rotation: {piece.Rotation}");
        Debug.Log($"? Initial cells: {string.Join(", ", initialCells)}");

        for (int i = 0; i < 4; i++)
        {
            piece.Rotate();
            var cells = piece.GetOccupiedCells();
            Debug.Log($"? Rotation {piece.Rotation}: {string.Join(", ", cells)}");
        }

        var finalCells = piece.GetOccupiedCells();
        bool backToStart = AreCellsSame(initialCells, finalCells);
        Debug.Log($"? After 4 rotations (360°): rotation={piece.Rotation} (should be 0)");
        Debug.Log($"? Cells back to initial: {backToStart} (should be TRUE)");
    }

    private bool AreCellsSame(System.Collections.Generic.List<Vector2Int> cells1,
                             System.Collections.Generic.List<Vector2Int> cells2)
    {
        if (cells1.Count != cells2.Count) return false;

        foreach (var cell in cells1)
        {
            if (!cells2.Contains(cell)) return false;
        }
        return true;
    }
}