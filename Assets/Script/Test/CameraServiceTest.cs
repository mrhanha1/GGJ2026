using Game.Core;
using Game.Services.Camera;
using Game.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraServiceTest : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CameraShakeProfile smallShake;
    [SerializeField] private CameraShakeProfile explosionShake;
    [SerializeField] private float moveSpeed = 5f;

    private ICameraService _cameraService;
    private IInputService _inputService;
    private Vector2 _moveInput;

    void Start()
    {
        _cameraService = ServiceLocator.Instance.Get<ICameraService>();
        _inputService = ServiceLocator.Instance.Get<IInputService>();

        // Subscribe to Player actions
        _inputService.Player.Move.performed += OnMove;
        _inputService.Player.Move.canceled += OnMove;
        _inputService.Player.TestShakeSmall.performed += OnTestShakeSmall;
        _inputService.Player.TestShakeExplosion.performed += OnTestShakeExplosion;
        _inputService.Player.TestShakeMega.performed += OnTestShakeMega;
        _inputService.Player.TestZoomIn.performed += OnTestZoomIn;
        _inputService.Player.TestZoomOut.performed += OnTestZoomOut;

        if (playerTransform != null)
        {
            _cameraService.SetFollowTarget(playerTransform);
        }
    }

    void Update()
    {
        // Move player with stored input
        if (playerTransform != null)
        {
            Vector3 movement = new Vector3(_moveInput.x, _moveInput.y, 0) * moveSpeed * Time.deltaTime;
            playerTransform.position += movement;
        }
    }

    // Input callbacks
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnTestShakeSmall(InputAction.CallbackContext context)
    {
        Debug.Log("Testing SMALL shake (Space)");
        _cameraService.ShakeCamera(smallShake);
    }

    private void OnTestShakeExplosion(InputAction.CallbackContext context)
    {
        Debug.Log("Testing EXPLOSION shake (E)");
        _cameraService.ShakeCamera(explosionShake);
    }

    private void OnTestShakeMega(InputAction.CallbackContext context)
    {
        Debug.Log("Testing MEGA shake (T)");
        _cameraService.ShakeCamera(50f, 1f);
    }

    private void OnTestZoomIn(InputAction.CallbackContext context)
    {
        Debug.Log("Zoom IN (Z)");
        _cameraService.SetZoom(3f, 1f);
    }

    private void OnTestZoomOut(InputAction.CallbackContext context)
    {
        Debug.Log("Zoom OUT (X)");
        _cameraService.SetZoom(5f, 1f);
    }

    void OnDestroy()
    {
        if (_inputService != null)
        {
            _inputService.Player.Move.performed -= OnMove;
            _inputService.Player.Move.canceled -= OnMove;
            _inputService.Player.TestShakeSmall.performed -= OnTestShakeSmall;
            _inputService.Player.TestShakeExplosion.performed -= OnTestShakeExplosion;
            _inputService.Player.TestShakeMega.performed -= OnTestShakeMega;
            _inputService.Player.TestZoomIn.performed -= OnTestZoomIn;
            _inputService.Player.TestZoomOut.performed -= OnTestZoomOut;
        }
    }
}