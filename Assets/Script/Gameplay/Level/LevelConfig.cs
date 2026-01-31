using UnityEngine;

namespace PuzzleGame.Gameplay.Level
{
    /// <summary>
    /// Configuration for a puzzle level with target map
    /// </summary>
    [CreateAssetMenu(fileName = "Level_01", menuName = "Puzzle Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level Info")]
        [SerializeField] private int levelIndex = 1;
        [SerializeField] private string levelName = "Level 1";

        [Header("Target Image")]
        [SerializeField] private Texture2D targetImage; // 15x15 pixel image

        [Header("Target Map (15x15)")]
        [SerializeField] private bool[] targetMapFlat = new bool[225]; // 15x15 = 225

        // Runtime target map
        private bool[,] targetMap;

        /// <summary>
        /// Get target map as 2D array
        /// </summary>
        public bool[,] GetTargetMap()
        {
            if (targetMap == null)
            {
                ConvertFlatToMap();
            }
            return targetMap;
        }

        /// <summary>
        /// Convert flat array to 2D map
        /// </summary>
        private void ConvertFlatToMap()
        {
            targetMap = new bool[15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    int index = y * 15 + x;
                    targetMap[x, y] = targetMapFlat[index];
                }
            }
        }

        /// <summary>
        /// Set target tile at position (for editor)
        /// </summary>
        public void SetTargetTile(int x, int y, bool value)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return;

            int index = y * 15 + x;
            targetMapFlat[index] = value;

            // Update runtime map if exists
            if (targetMap != null)
            {
                targetMap[x, y] = value;
            }
        }

        /// <summary>
        /// Get target tile at position
        /// </summary>
        public bool IsTargetTile(int x, int y)
        {
            if (x < 0 || x >= 15 || y < 0 || y >= 15)
                return false;

            int index = y * 15 + x;
            return targetMapFlat[index];
        }

        /// <summary>
        /// Get pixel color from target image
        /// </summary>
        public Color GetPixelColor(int x, int y)
        {
            if (targetImage == null || x < 0 || x >= 15 || y < 0 || y >= 15)
                return Color.clear;

            // Ensure texture is readable
            if (!targetImage.isReadable)
            {
                Debug.LogWarning($"[LevelConfig] Target image '{targetImage.name}' is not readable!");
                return Color.clear;
            }

            return targetImage.GetPixel(x, y);
        }

        /// <summary>
        /// Import target map from image (for editor)
        /// </summary>
        public void ImportFromImage()
        {
            if (targetImage == null)
            {
                Debug.LogWarning("[LevelConfig] No target image assigned!");
                return;
            }

            if (!targetImage.isReadable)
            {
                Debug.LogError("[LevelConfig] Target image must be readable! Enable Read/Write in import settings.");
                return;
            }

            if (targetImage.width != 15 || targetImage.height != 15)
            {
                Debug.LogError("[LevelConfig] Target image must be exactly 15x15 pixels!");
                return;
            }

            // Import: non-transparent pixels = target tiles
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    Color pixel = targetImage.GetPixel(x, y);
                    bool isTarget = pixel.a > 0.5f; // Alpha > 0.5 = target
                    SetTargetTile(x, y, isTarget);
                }
            }

            Debug.Log($"[LevelConfig] Imported target map from image '{targetImage.name}'");
        }

        /// <summary>
        /// Fill all tiles as target
        /// </summary>
        public void FillAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = true;
            }
            targetMap = null; // Force rebuild
        }

        /// <summary>
        /// Clear all target tiles
        /// </summary>
        public void ClearAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = false;
            }
            targetMap = null; // Force rebuild
        }

        /// <summary>
        /// Invert all target tiles
        /// </summary>
        public void InvertAll()
        {
            for (int i = 0; i < targetMapFlat.Length; i++)
            {
                targetMapFlat[i] = !targetMapFlat[i];
            }
            targetMap = null; // Force rebuild
        }

        // Getters
        public int LevelIndex => levelIndex;
        public string LevelName => levelName;
        public Texture2D TargetImage => targetImage;
    }
}