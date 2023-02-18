using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
    private Camera mainCamera;

    private GameObject cube;
    private NavMeshAgent agent;

    void Start() {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
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
                if (agent) {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}
