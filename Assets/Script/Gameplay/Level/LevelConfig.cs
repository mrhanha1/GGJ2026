using UnityEngine;

namespace PuzzleGame.Gameplay.Level
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Puzzle Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level Info")]
        [SerializeField] private string levelName = "Level 1";
        [SerializeField] private int levelIndex = 1;

        [Header("Time Limit")]
        [SerializeField] private float timeLimit = 120f; // seconds (default 2 minutes)

        [Header("Target Image")]
        [SerializeField] private Texture2D targetImage; // 15x15 pixel image

        [Header("Target Map (15x15 = 225 cells)")]
        [SerializeField] private bool[] targetMapFlat = new bool[225]; // 15x15 flattened

        // Runtime 2D map cache
        private bool[,] targetMapCache;

        /// <summary>
        /// Get target map as 2D array
        /// </summary>
        public bool[,] TargetMap
        {
            get
            {
                if (targetMapCache == null)
                {
                    ConvertFlatTo2D();
                }
                return targetMapCache;
            }
        }

        /// <summary>
        /// Convert flat array to 2D map
        /// </summary>
        private void ConvertFlatTo2D()
        {
            targetMapCache = new bool[15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    int index = y * 15 + x;
                    targetMapCache[x, y] = targetMapFlat[index];
                }
            }
        }

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

            int index = y * 15 + x;
            return targetMapFlat[index];
        }

        // Getters
        public string LevelName => levelName;
        public int LevelIndex => levelIndex;
        public float TimeLimit => timeLimit;
        public Texture2D TargetImage => targetImage;

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

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    Color pixel = targetImage.GetPixel(x, y);
                    int index = y * 15 + x;
                    // Consider pixel as target if alpha > 0.1
                    targetMapFlat[index] = pixel.a > 0.1f;
                }
            }

            targetMapCache = null; // Clear cache
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("Target map imported from image!");
        }

        /// <summary>
        /// Set target tile at position
        /// </summary>
        public void SetTargetTile(int x, int y, bool isTarget)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return;

            int index = y * 15 + x;
            targetMapFlat[index] = isTarget;
            targetMapCache = null; // Clear cache
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Fill all tiles as targets
        /// </summary>
        public void FillAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = true;
            }
            targetMapCache = null; // Clear cache
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Clear all tiles
        /// </summary>
        public void ClearAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = false;
            }
            targetMapCache = null; // Clear cache
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Invert all tiles
        /// </summary>
        public void InvertAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = !targetMapFlat[i];
            }
            targetMapCache = null; // Clear cache
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}