// Assets/_Project/Scripts/Services/Camera/ICameraService.cs

using UnityEngine;

namespace Game.Services.Camera
{
    /// <summary>
    /// Interface for camera management service.
    /// Handles Cinemachine camera control, shake, follow target, and zoom.
    /// </summary>
    public interface ICameraService : Game.Core.IService
    {
        /// <summary>
        /// Shake camera using a profile.
        /// </summary>
        void ShakeCamera(CameraShakeProfile profile);

        /// <summary>
        /// Shake camera with custom parameters.
        /// </summary>
        void ShakeCamera(float force, float duration);

        /// <summary>
        /// Set the camera follow target.
        /// </summary>
        void SetFollowTarget(Transform target);

        /// <summary>
        /// Set the camera look-at target.
        /// </summary>
        void SetLookAtTarget(Transform target);

        /// <summary>
        /// Set orthographic camera size (zoom).
        /// </summary>
        void SetZoom(float size, float duration = 0f);

        /// <summary>
        /// Get current orthographic size.
        /// </summary>
        float GetZoom();
    }
}