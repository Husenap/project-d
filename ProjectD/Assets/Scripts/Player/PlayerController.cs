using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    private Camera mainCamera;
    private NavMeshAgent agent;
    private PlayerState state;
    private PlayerInput playerInput;

    private InputAction moveAction;

    private GameObject debugCube;

    void Start() {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        state = GetComponent<PlayerState>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Player/Move"];

        playerInput.actions["Player/Ability1"].performed += _ => state.changeMana(-1);
        playerInput.actions["Player/Ability2"].performed += _ => state.changeHealth(-1);
    }

    private void Update() {
        if (moveAction.WasPerformedThisFrame() && !EventSystem.current.IsPointerOverGameObject()) {
            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.value);
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
}
