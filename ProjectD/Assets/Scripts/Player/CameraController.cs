using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour, PD.Input.ICameraControllerActions {
    [SerializeField] private Vector3 focusOffset;
    [SerializeField] private float nearCameraHeight;
    [SerializeField] private float nearCameraDistance;
    [SerializeField] private float farCameraHeight;
    [SerializeField] private float farCameraDistance;
    [SerializeField] private float angle = 0.0f;

    private Camera cam;

    private float zoom = 0.5f;
    private float zoomTarget;
    private float angleTarget;

    private float cameraHeight;
    private float cameraDistance;

    private Volume volume;

    private PD.Input input;

    private void OnEnable() {
        input ??= new PD.Input();
        input.CameraController.SetCallbacks(this);
        input.CameraController.Enable();
    }
    private void OnDisable() {
        input.CameraController.Disable();
    }

    private void Start() {
        cam = GetComponentInChildren<Camera>();

        UpdateHeightAndDistance();
        volume = FindFirstObjectByType<Volume>();

        angleTarget = angle;
        zoomTarget = zoom;
    }

    private void UpdateHeightAndDistance() {
        cameraHeight = Mathf.Lerp(nearCameraHeight, farCameraHeight, zoom * zoom);
        cameraDistance = Mathf.Lerp(nearCameraDistance, farCameraDistance, zoom);
    }

    void Update() {
        if (input.CameraController.RotateCamera.IsPressed()) {
            angleTarget += input.CameraController.CameraRotation.ReadValue<float>();
        }
        if (cam) {
            var cameraOffset = new Vector3(cameraDistance * Mathf.Cos(Mathf.Deg2Rad * angle), cameraHeight, cameraDistance * Mathf.Sin(Mathf.Deg2Rad * angle));
            var focusPoint = transform.position + focusOffset;

            cam.transform.position = focusPoint + cameraOffset;
            cam.transform.LookAt(focusPoint);

            if (volume) {
                var dof = volume.profile.components.Find(c => c is DepthOfField) as DepthOfField;
                if (dof) {
                    dof.focusDistance.Override((focusPoint - cam.transform.position).magnitude);
                }
            }
            zoomTarget = Mathf.Clamp01(zoomTarget - Mouse.current.scroll.value.y / 1000.0f);
            zoom = Mathf.Lerp(zoom, zoomTarget, Time.deltaTime * 10.0f);
            angle = Mathf.Lerp(angle, angleTarget, Time.deltaTime * 10.0f);
            UpdateHeightAndDistance();
        }
    }

    public void OnRotateCamera(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }

    public void OnCameraRotation(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }
}
