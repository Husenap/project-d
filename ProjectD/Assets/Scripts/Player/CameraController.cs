using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour {
    private Camera mainCamera;

    [SerializeField] private Vector3 focusOffset;
    [SerializeField] private float nearCameraHeight;
    [SerializeField] private float nearCameraDistance;
    [SerializeField] private float farCameraHeight;
    [SerializeField] private float farCameraDistance;
    [SerializeField] private float angle = 0.0f;

    private float zoomTarget = 0.5f;
    private float zoom;

    private float cameraHeight;
    private float cameraDistance;

    private Volume volume;

    private void Start() {
        mainCamera = Camera.main;
        UpdateHeightAndDistance();
        volume = FindFirstObjectByType<Volume>();
        zoom = zoomTarget;
    }

    private void UpdateHeightAndDistance() {
        cameraHeight = Mathf.Lerp(nearCameraHeight, farCameraHeight, zoom * zoom);
        cameraDistance = Mathf.Lerp(nearCameraDistance, farCameraDistance, zoom);
    }

    void Update() {
        if (mainCamera) {
            var cameraOffset = new Vector3(cameraDistance * Mathf.Cos(Mathf.Deg2Rad * angle), cameraHeight, cameraDistance * Mathf.Sin(Mathf.Deg2Rad * angle));
            var focusPoint = transform.position + focusOffset;

            mainCamera.transform.position = focusPoint + cameraOffset;
            mainCamera.transform.LookAt(focusPoint);

            if (volume) {
                var dof = volume.profile.components.Find(c => c is DepthOfField) as DepthOfField;
                if (dof) {
                    dof.focusDistance.Override((focusPoint - mainCamera.transform.position).magnitude);
                }
            }
            zoomTarget = Mathf.Clamp01(zoomTarget - Mouse.current.scroll.value.y / 1000.0f);
            zoom = Mathf.Lerp(zoom, zoomTarget, Time.deltaTime * 10.0f);
            UpdateHeightAndDistance();


        }
    }
}
