using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spells : MonoBehaviour
{
    enum COOLDOWN {
        FIREBALL,
        METEOR,
        TELEPORT
    }

    private GameObject meteor;
    private GameObject fireball;
    private readonly List<float> cooldowns = new() { 0f, 0f, 0f };
    private readonly List<float> maxCooldowns = new() { 1f, 5f, 1f };
    /*
     * Spell class, containing:
     * currentCooldown
     * maxCooldown
     * spell functioning
     * 
     * List<Spell>
     */

    [SerializeField]
    private GameObject player;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            FireBall();
        }
        if (Input.GetKey(KeyCode.Q)) {
            Meteor();
        }
        if (Input.GetKey(KeyCode.E)) {
            Teleport();
        }
        UpdateCooldowns(Time.deltaTime);
        UpdateSpells(Time.deltaTime);
    }

    private void UpdateCooldowns(float deltaTime) {
        for (int i = 0; i < cooldowns.Count; i++) {
            cooldowns[i] -= deltaTime;
        }
    }

    private void UpdateSpells(float deltaTime) {
        if (fireball != null) {
            fireball.transform.position += 20 * deltaTime * fireball.transform.forward;
        }
    }

    void FireBall() {
        if (cooldowns[(int)COOLDOWN.FIREBALL] <= 0f) {
            fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var point = player.transform.position;
            fireball.transform.SetPositionAndRotation(new Vector3(point.x, point.y + 2f, point.z), player.transform.rotation);
            fireball.GetComponent<Renderer>().material.color = Color.red;
            cooldowns[(int)COOLDOWN.FIREBALL] = maxCooldowns[(int)COOLDOWN.FIREBALL];
            Destroy(fireball, 3f);
        }
    }

    void Meteor() {
        if (cooldowns[(int)COOLDOWN.METEOR] <= 0f) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                meteor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                meteor.transform.position = new Vector3(hit.point.x, hit.point.y + 10f, hit.point.z);
                meteor.transform.localScale = Vector3.one * 5f;
                meteor.AddComponent<Rigidbody>();
                meteor.GetComponent<Renderer>().material.color = Color.red;
            }
            cooldowns[(int)COOLDOWN.METEOR] = maxCooldowns[(int)COOLDOWN.METEOR];
        }
    }

    void Teleport() {
        if (cooldowns[(int)COOLDOWN.TELEPORT] <= 0f) {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                player.transform.position = hit.transform.position;
                //player.transform.position += player.transform.forward * 1f;
            }
        }
    }
}
