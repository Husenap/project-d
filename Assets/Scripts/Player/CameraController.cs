using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Vector3 cameraOffset;

    private void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = gameObject.transform.position + cameraOffset;
            mainCamera.transform.LookAt(gameObject.transform.position);
        }
    }
}
