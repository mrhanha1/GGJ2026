// Assets/_Project/Scripts/Services/Camera/CameraService.cs

using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

namespace Game.Services.Camera
{
    /// <summary>
    /// Camera service implementation using Cinemachine 3.x.
    /// Manages camera shake (queue-based), follow/look-at targets, and zoom.
    /// </summary>
    public class CameraService : MonoBehaviour, ICameraService
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        private bool _isShaking = false;

        public void Initialize()
        {
            if (cinemachineCamera == null)
            {
                Debug.LogError("[CameraService] CinemachineCamera reference is missing!");
            }

            if (impulseSource == null)
            {
                Debug.LogError("[CameraService] CinemachineImpulseSource reference is missing!");
            }

            Debug.Log("[CameraService] Initialized.");
        }

        // ===== SHAKE =====
        public void ShakeCamera(CameraShakeProfile profile)
        {
            if (profile == null)
            {
                Debug.LogWarning("[CameraService] Shake profile is null.");
                return;
            }

            ShakeCamera(profile.force, profile.duration);
        }

        public void ShakeCamera(float force, float duration)
        {
            if (impulseSource == null)
            {
                Debug.LogWarning("[CameraService] ImpulseSource is missing, cannot shake.");
                return;
            }

            // Queue-based: wait for current shake to finish
            if (_isShaking)
            {
                Debug.Log("[CameraService] Shake queued (waiting for current shake to finish).");
                StartCoroutine(QueueShake(force, duration));
            }
            else
            {
                ExecuteShake(force, duration);
            }
        }

        private IEnumerator QueueShake(float force, float duration)
        {
            yield return new WaitWhile(() => _isShaking);
            ExecuteShake(force, duration);
        }

        private void ExecuteShake(float force, float duration)
        {
            _isShaking = true;

            Vector3 velocity = Random.insideUnitSphere * force;
            impulseSource.GenerateImpulseWithVelocity(velocity);

            Debug.Log($"[CameraService] Shake executed - Force: {force}, Duration: {duration}s");
            StartCoroutine(ShakeDuration(duration));
        }

        private IEnumerator ShakeDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            _isShaking = false;
        }

        // ===== FOLLOW/LOOK AT =====
        public void SetFollowTarget(Transform target)
        {
            if (cinemachineCamera == null) return;

            cinemachineCamera.Follow = target;
            Debug.Log($"[CameraService] Follow target set to: {(target != null ? target.name : "null")}");
        }

        public void SetLookAtTarget(Transform target)
        {
            if (cinemachineCamera == null) return;

            cinemachineCamera.LookAt = target;
            Debug.Log($"[CameraService] LookAt target set to: {(target != null ? target.name : "null")}");
        }

        // ===== ZOOM =====
        public void SetZoom(float size, float duration = 0f)
        {
            if (cinemachineCamera == null) return;

            if (duration <= 0f)
            {
                cinemachineCamera.Lens.OrthographicSize = size;
                Debug.Log($"[CameraService] Zoom set instantly to: {size}");
            }
            else
            {
                StartCoroutine(ZoomCoroutine(size, duration));
            }
        }

        private IEnumerator ZoomCoroutine(float targetSize, float duration)
        {
            float startSize = cinemachineCamera.Lens.OrthographicSize;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t);
                yield return null;
            }

            cinemachineCamera.Lens.OrthographicSize = targetSize;
            Debug.Log($"[CameraService] Zoom completed to: {targetSize}");
        }

        public float GetZoom()
        {
            return cinemachineCamera != null ? cinemachineCamera.Lens.OrthographicSize : 0f;
        }
    }
}