using UnityEngine;

namespace PuzzleGame.Gameplay.Level
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Puzzle Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level Info")]
        [SerializeField] private string levelName = "Level 1";
        [SerializeField] private int levelIndex = 1;

        [Header("Time Limit (NEW)")]
        [SerializeField] private float timeLimit = 120f; // seconds (default 2 minutes)

        [Header("Target Image")]
        [SerializeField] private Texture2D targetImage; // 15x15 pixel image

        [Header("Target Map")]
        [SerializeField] private bool[,] targetMap = new bool[15, 15];

        /// <summary>
        /// Get pixel color at position from target image
        /// </summary>
        public Color GetPixelColor(int x, int y)
        {
            if (targetImage == null)
                return Color.clear;

            if (x < 0 || x >= targetImage.width || y < 0 || y >= targetImage.height)
                return Color.clear;

            return targetImage.GetPixel(x, y);
        }

        /// <summary>
        /// Check if position is a target tile
        /// </summary>
        public bool IsTargetTile(int x, int y)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            return targetMap[x, y];
        }

        // Getters
        public string LevelName => levelName;
        public int LevelIndex => levelIndex;
        public float TimeLimit => timeLimit; // NEW
        public Texture2D TargetImage => targetImage;
        public bool[,] TargetMap => targetMap;

#if UNITY_EDITOR
        /// <summary>
        /// Import target map from target image
        /// </summary>
        public void ImportFromImage()
        {
            if (targetImage == null)
            {
                Debug.LogWarning("No target image assigned!");
                return;
            }

            if (targetImage.width != 15 || targetImage.height != 15)
            {
                Debug.LogWarning("Target image must be 15x15 pixels!");
                return;
            }

            targetMap = new bool[15, 15];

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    Color pixel = targetImage.GetPixel(x, y);
                    // Consider pixel as target if alpha > 0.1
                    targetMap[x, y] = pixel.a > 0.1f;
                }
            }

            Debug.Log("Target map imported from image!");
        }

        /// <summary>
        /// Set target tile at position
        /// </summary>
        public void SetTargetTile(int x, int y, bool isTarget)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return;

            targetMap[x, y] = isTarget;
        }

        /// <summary>
        /// Fill all tiles as targets
        /// </summary>
        public void FillAll()
        {
            targetMap = new bool[15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    targetMap[x, y] = true;
                }
            }
        }

        /// <summary>
        /// Clear all tiles
        /// </summary>
        public void ClearAll()
        {
            targetMap = new bool[15, 15];
        }

        /// <summary>
        /// Invert all tiles
        /// </summary>
        public void InvertAll()
        {
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    targetMap[x, y] = !targetMap[x, y];
                }
            }
        }
#endif
    }

}
