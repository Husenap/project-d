using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour {
    [SerializeField] private Vector3 focusOffset;
    [SerializeField] private float nearCameraHeight;
    [SerializeField] private float nearCameraDistance;
    [SerializeField] private float farCameraHeight;
    [SerializeField] private float farCameraDistance;

    private Camera cam;

    private float angle = 0.0f;
    private float zoom = 1;
    private float zoomTarget;
    private float angleTarget;
    private float cameraHeight;
    private float cameraDistance;
    private Vector3 positionTarget;
    private Vector3 position;


    private Volume volume;

    private PD.Input input;

    private enum CameraState {
        FollowPlayer,
        FreeFly
    };
    CameraState state = CameraState.FollowPlayer;

    private void OnEnable() {
        input ??= new PD.Input();
        input.CameraController.Enable();
    }
    private void OnDisable() {
        input.CameraController.Disable();
    }

    private void Start() {
        cam = GetComponentInChildren<Camera>();

        volume = FindFirstObjectByType<Volume>();

        angleTarget = angle;
        zoomTarget = zoom;

        cam.transform.parent = null;

        position = positionTarget = transform.position;
    }

    void Update() {
        if (input.CameraController.RotateCamera.IsPressed()) {
            angleTarget += input.CameraController.CameraRotation.ReadValue<float>();
        }

        if (input.CameraController.CameraMovement.WasPerformedThisFrame()) {
            state = CameraState.FreeFly;
        } else if (input.CameraController.CenterCamera.WasPerformedThisFrame()) {
            angle = angle % 360.0f;
            if (angle > 180.0f)
                angle -= 360.0f;
            if (angle < -180.0f)
                angle += 360.0f;

            angleTarget = 0.0f;
            zoomTarget = 1.0f;
            positionTarget = transform.position;
            if (Vector3.Distance(position, transform.position) > 50.0f) {
                position = positionTarget;
                angle = angleTarget;
                zoom = zoomTarget;
            }
            state = CameraState.FollowPlayer;
        }

        switch (state) {
            case CameraState.FollowPlayer:
                FollowPlayer();
                break;
            case CameraState.FreeFly:
                FreeFly();
                break;
        }

        zoomTarget = Mathf.Clamp01(zoomTarget - Mouse.current.scroll.value.y / 1000.0f);
        zoom = Mathf.Lerp(zoom, zoomTarget, Time.deltaTime * 10.0f);
        angle = Mathf.Lerp(angle, angleTarget, Time.deltaTime * 10.0f);
        cameraHeight = Mathf.Lerp(nearCameraHeight, farCameraHeight, zoom);
        cameraDistance = Mathf.Lerp(nearCameraDistance, farCameraDistance, zoom);
        {
            var fastPosition = Vector3.Lerp(position, positionTarget, Time.deltaTime * 15.0f);
            var slowPosition = Vector3.Lerp(position, positionTarget, Time.deltaTime * 7.5f);
            position = new Vector3(fastPosition.x, slowPosition.y, fastPosition.z);
        }


        if (cam) {
            var cameraOffset = new Vector3(cameraDistance * Mathf.Sin(Mathf.Deg2Rad * angle), cameraHeight, cameraDistance * -Mathf.Cos(Mathf.Deg2Rad * angle));
            var focusPoint = focusOffset + position;

            cam.transform.position = focusPoint + cameraOffset;
            cam.transform.LookAt(focusPoint);

            if (volume) {
                var dof = volume.profile.components.Find(c => c is DepthOfField) as DepthOfField;
                if (dof) {
                    dof.focusDistance.Override((focusPoint - cam.transform.position).magnitude);
                }
            }
        }
    }

    private void FollowPlayer() {
        positionTarget = transform.position;
    }
    private void FreeFly() {
        var movement = input.CameraController.CameraMovement.ReadValue<Vector2>() * Time.deltaTime;

        var right = cam.transform.right;
        right.y = 0;
        var forward = new Vector3(-right.z, 0, right.x);

        positionTarget += movement.x * right + movement.y * forward;

        if (NavMesh.SamplePosition(positionTarget, out NavMeshHit hit, float.MaxValue, NavMesh.AllAreas)) {
            var pos = positionTarget;
            pos.y = hit.position.y;

            pos = hit.position + Vector3.ClampMagnitude(pos - hit.position, 10.0f);
            positionTarget = pos;
        }
    }
}
