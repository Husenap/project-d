using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerState))]
public class PlayerController : MonoBehaviour {
    private Camera mainCamera;
    private NavMeshAgent agent;
    private PlayerState state;

    private GameObject debugCube;

    [SerializeField]
    private InputActionAsset inputActions;

    void Start() {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        state = GetComponent<PlayerState>();

        inputActions.FindAction("Move").performed += _ => Move();
        inputActions.FindAction("Ability1").performed += _ => state.changeMana(-1);
        inputActions.FindAction("Ability2").performed += _ => state.changeHealth(-1);
    }

    private void Move() {
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
}
