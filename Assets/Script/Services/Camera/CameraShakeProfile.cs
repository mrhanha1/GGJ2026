// Assets/_Project/Scripts/Services/Camera/CameraShakeProfile.cs

using UnityEngine;

namespace Game.Services.Camera
{
    /// <summary>
    /// ScriptableObject defining camera shake parameters.
    /// Create via: Assets → Create → Game → Camera Shake Profile
    /// </summary>
    [CreateAssetMenu(fileName = "CameraShake", menuName = "Game/Camera Shake Profile", order = 2)]
    public class CameraShakeProfile : ScriptableObject
    {
        [Header("Impulse Settings")]
        [Tooltip("Shake force/amplitude")]
        [Range(0.1f, 10f)] public float force = 1f;

        [Tooltip("Shake duration in seconds")]
        [Range(0.1f, 2f)] public float duration = 0.5f;

        [Tooltip("Shake frequency (oscillations per second)")]
        [Range(1f, 30f)] public float frequency = 10f;

        [Header("Advanced")]
        [Tooltip("Custom velocity for directional shake")]
        public Vector3 customVelocity = Vector3.zero;
    }
}