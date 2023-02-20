using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerState))]
public class PlayerController : MonoBehaviour {
    private Camera mainCamera;
    private NavMeshAgent agent;
    private PlayerState state;

    private GameObject debugCube;

    void Start() {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        state = GetComponent<PlayerState>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (!debugCube) {
                    debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    debugCube.transform.localScale = Vector3.one * 0.1f;
                }
                debugCube.transform.position = hit.point;
                debugCube.GetComponent<Renderer>().material.color = Color.red;
                if (agent) {
                    agent.SetDestination(hit.point);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            state.changeMana(-10);
        }
    }
}
