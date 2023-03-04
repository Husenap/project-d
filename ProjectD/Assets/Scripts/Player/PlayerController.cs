using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PD.Input.IPlayerControllerActions {
    private Camera cam;

    private NavMeshAgent agent;
    private PlayerState state;

    private GameObject debugCube;

    private PD.Input input;

    private void OnEnable() {
        input ??= new PD.Input();
        input.PlayerController.SetCallbacks(this);
        input.PlayerController.Enable();
    }

    private void OnDisable() {
        input?.PlayerController.Disable();
    }

    public void OnAbility1(InputAction.CallbackContext context) {
        state.changeMana(-1);
    }

    public void OnAbility2(InputAction.CallbackContext context) {
        state.changeHealth(-1);
    }

    public void OnMove(InputAction.CallbackContext context) {
        throw new System.NotImplementedException();
    }

    void Start() {
        cam = GetComponentInChildren<Camera>();

        agent = GetComponent<NavMeshAgent>();
        state = GetComponent<PlayerState>();
    }

    private void Update() {
        if (input.PlayerController.Move.WasPerformedThisFrame() && !EventSystem.current.IsPointerOverGameObject()) {
            var ray = cam.ScreenPointToRay(Mouse.current.position.value);
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
