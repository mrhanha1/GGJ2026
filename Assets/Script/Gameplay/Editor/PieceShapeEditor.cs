using UnityEngine;
using UnityEditor;

namespace PuzzleGame.Gameplay.Pieces.Editor
{
    [CustomEditor(typeof(PieceShape))]
    public class PieceShapeEditor : UnityEditor.Editor
    {
        private const float CELL_SIZE = 25f;
        private const float PADDING = 10f;

        private Color occupiedColor = new Color(0.3f, 0.7f, 1f, 1f); // Blue
        private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray

        public override void OnInspectorGUI()
        {
            PieceShape shape = (PieceShape)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Piece Shape Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Draw grid
            DrawShapeGrid(shape);

            EditorGUILayout.Space(10);

            // Control buttons
            DrawControlButtons(shape);

            EditorGUILayout.Space(10);

            // Preview info
            DrawPreviewInfo(shape);

            EditorGUILayout.Space(10);

            // Default inspector for other fields
            DrawDefaultInspector();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawShapeGrid(PieceShape shape)
        {
            int width = shape.Width;
            int height = shape.Height;

            // Calculate total size
            float totalWidth = width * CELL_SIZE + PADDING * 2;
            float totalHeight = height * CELL_SIZE + PADDING * 2;

            Rect gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);

            // Draw background
            EditorGUI.DrawRect(gridRect, new Color(0.15f, 0.15f, 0.15f, 1f));

            // Draw cells
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + PADDING + x * CELL_SIZE,
                        gridRect.y + PADDING + y * CELL_SIZE,
                        CELL_SIZE - 2,
                        CELL_SIZE - 2
                    );

                    bool isOccupied = shape.GetCell(x, y);
                    Color cellColor = isOccupied ? occupiedColor : emptyColor;

                    EditorGUI.DrawRect(cellRect, cellColor);

                    // Handle click
                    Event e = Event.current;
                    if (e.type == EventType.MouseDown && cellRect.Contains(e.mousePosition))
                    {
                        Undo.RecordObject(shape, "Toggle Cell");
                        shape.SetCell(x, y, !isOccupied);
                        GUI.changed = true;
                        e.Use();
                    }
                }
            }
        }

        private void DrawControlButtons(PieceShape shape)
        {
            EditorGUILayout.BeginHorizontal();

            // Clear button
            if (GUILayout.Button("Clear All", GUILayout.Height(30)))
            {
                Undo.RecordObject(shape, "Clear Shape");
                shape.Clear();
                GUI.changed = true;
            }

            // Fill button
            if (GUILayout.Button("Fill All", GUILayout.Height(30)))
            {
                Undo.RecordObject(shape, "Fill Shape");
                shape.Fill();
                GUI.changed = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPreviewInfo(PieceShape shape)
        {
            EditorGUILayout.LabelField("Preview Info", EditorStyles.boldLabel);

            var cells = shape.GetOccupiedCells();
            EditorGUILayout.LabelField($"Occupied Cells: {cells.Count}");
            EditorGUILayout.LabelField($"Size: {shape.Width} x {shape.Height}");

            // Draw occupied cells list
            if (cells.Count > 0)
            {
                EditorGUILayout.LabelField("Cells: " + string.Join(", ", cells));
            }
        }
    }
}