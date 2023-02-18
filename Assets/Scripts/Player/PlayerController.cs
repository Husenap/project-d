using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Camera mainCamera;

    private GameObject cube;
    private Vector3? targetPosition;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                Debug.Log(hit.transform.gameObject.name + ": " + hit.point);

                if (cube == null) {
                    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * 0.1f;
                }
                cube.transform.position = hit.point;
                cube.GetComponent<Renderer>().material.color = Color.red;
                targetPosition = hit.point;
            }
        }

        if (targetPosition.HasValue) {
            var dist = Vector3.Distance(targetPosition.Value, transform.position);
            var speedMultiplier = Mathf.SmoothStep(0, 1, dist);
            transform.position += (targetPosition.Value - transform.position).normalized * speedMultiplier * Time.deltaTime * 5.0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition.Value - transform.position), Time.deltaTime * 5.0f);
        }
    }
}
