using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected Transform myTransform;

    int health = 100;



    public virtual void Start() {
        myTransform = GetComponent<Transform>();
    }





    protected PlayerState[] TargetsInRange(float radius) { // TODO: Change to some different entity to be able to attack summoned creatures or whatever
        var center = myTransform.position;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        List<PlayerState> targets = new List<PlayerState>();
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.gameObject.GetComponent<PlayerState>() != null) { //errors here

                targets.Add(hitCollider.gameObject.GetComponent<PlayerState>());
            }
        }
        return targets.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
