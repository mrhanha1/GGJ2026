using UnityEngine;
using UnityEditor;
using PuzzleGame.Gameplay.Level;

namespace PuzzleGame.Gameplay.Editor
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        private const int GRID_SIZE = 15;
        private const float CELL_SIZE = 20f;
        private const float PADDING = 10f;

        private Color targetColor = new Color(1f, 0.84f, 0f, 1f); // Yellow
        private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
        private Color gridLineColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Dark gray

        public override void OnInspectorGUI()
        {
            LevelConfig config = (LevelConfig)target;

            // Draw default inspector first
            DrawDefaultInspector();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Target Map Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // Import/Export buttons
            DrawActionButtons(config);

            EditorGUILayout.Space(10);

            // Draw interactive grid
            DrawInteractiveGrid(config);

            EditorGUILayout.Space(10);

            // Statistics
            DrawStatistics(config);
        }

        private void DrawActionButtons(LevelConfig config)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import from Image", GUILayout.Height(30)))
            {
                Undo.RecordObject(config, "Import Target Map");
                config.ImportFromImage();
                EditorUtility.SetDirty(config);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill All"))
            {
                Undo.RecordObject(config, "Fill All Tiles");
                config.FillAll();
                EditorUtility.SetDirty(config);
            }

            if (GUILayout.Button("Clear All"))
            {
                Undo.RecordObject(config, "Clear All Tiles");
                config.ClearAll();
                EditorUtility.SetDirty(config);
            }

            if (GUILayout.Button("Invert"))
            {
                Undo.RecordObject(config, "Invert Tiles");
                config.InvertAll();
                EditorUtility.SetDirty(config);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawInteractiveGrid(LevelConfig config)
        {
            // Get rect for grid
            float gridWidth = GRID_SIZE * CELL_SIZE + PADDING * 2;
            float gridHeight = GRID_SIZE * CELL_SIZE + PADDING * 2;
            Rect gridRect = GUILayoutUtility.GetRect(gridWidth, gridHeight);

            // Draw background
            EditorGUI.DrawRect(gridRect, new Color(0.2f, 0.2f, 0.2f, 1f));

            // Handle mouse input
            Event e = Event.current;
            bool isMouseDown = e.type == EventType.MouseDown || e.type == EventType.MouseDrag;
            bool isLeftClick = e.button == 0;
            bool isRightClick = e.button == 1;

            // Draw cells
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    // Calculate cell position (flip Y for correct display)
                    float cellX = gridRect.x + PADDING + x * CELL_SIZE;
                    float cellY = gridRect.y + PADDING + (GRID_SIZE - 1 - y) * CELL_SIZE;
                    Rect cellRect = new Rect(cellX, cellY, CELL_SIZE - 1, CELL_SIZE - 1);

                    // Get cell state
                    bool isTarget = config.IsTargetTile(x, y);

                    // Draw cell
                    Color cellColor = isTarget ? targetColor : emptyColor;
                    EditorGUI.DrawRect(cellRect, cellColor);

                    // Handle click
                    if (isMouseDown && cellRect.Contains(e.mousePosition))
                    {
                        Undo.RecordObject(config, "Toggle Target Tile");

                        if (isLeftClick)
                        {
                            config.SetTargetTile(x, y, true); // Set target
                        }
                        else if (isRightClick)
                        {
                            config.SetTargetTile(x, y, false); // Clear target
                        }

                        EditorUtility.SetDirty(config);
                        e.Use();
                    }

                    // Draw grid lines
                    Handles.color = gridLineColor;
                    Handles.DrawLine(
                        new Vector3(cellRect.xMin, cellRect.yMin),
                        new Vector3(cellRect.xMax, cellRect.yMin)
                    );
                    Handles.DrawLine(
                        new Vector3(cellRect.xMin, cellRect.yMin),
                        new Vector3(cellRect.xMin, cellRect.yMax)
                    );
                }
            }

            // Draw outer border
            Handles.color = Color.white;
            Handles.DrawLine(
                new Vector3(gridRect.x + PADDING, gridRect.y + PADDING),
                new Vector3(gridRect.x + PADDING + GRID_SIZE * CELL_SIZE, gridRect.y + PADDING)
            );
            Handles.DrawLine(
                new Vector3(gridRect.x + PADDING, gridRect.y + PADDING),
                new Vector3(gridRect.x + PADDING, gridRect.y + PADDING + GRID_SIZE * CELL_SIZE)
            );
            Handles.DrawLine(
                new Vector3(gridRect.x + PADDING + GRID_SIZE * CELL_SIZE, gridRect.y + PADDING),
                new Vector3(gridRect.x + PADDING + GRID_SIZE * CELL_SIZE, gridRect.y + PADDING + GRID_SIZE * CELL_SIZE)
            );
            Handles.DrawLine(
                new Vector3(gridRect.x + PADDING, gridRect.y + PADDING + GRID_SIZE * CELL_SIZE),
                new Vector3(gridRect.x + PADDING + GRID_SIZE * CELL_SIZE, gridRect.y + PADDING + GRID_SIZE * CELL_SIZE)
            );

            // Instructions
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "Left Click: Set target tile (yellow)\n" +
                "Right Click: Clear tile (gray)\n" +
                "Drag: Paint multiple tiles",
                MessageType.Info
            );
        }

        private void DrawStatistics(LevelConfig config)
        {
            int targetCount = 0;
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    if (config.IsTargetTile(x, y))
                        targetCount++;
                }
            }

            int totalTiles = GRID_SIZE * GRID_SIZE;
            float percentage = (targetCount / (float)totalTiles) * 100f;

            EditorGUILayout.LabelField($"Target Tiles: {targetCount} / {totalTiles} ({percentage:F1}%)", EditorStyles.helpBox);
        }
    }
}