using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Enemy
{


    PlayerState target;
    const int ATTACK_DISTANCE = 2;
    public override void Start() {
        base.Start();
        InvokeRepeating(nameof(UpdateBrain), 0, 1);
    }


    private void Update() {
        WalkTowards();
    }



    float Dist(PlayerState player) {
        var p1 = new Vector2(player.transform.position.x, player.transform.position.z);
        var p2 = new Vector2(myTransform.position.x, myTransform.position.z);
        return Vector2.Distance(p1, p2);
    }

    PlayerState minPlayer(PlayerState[] players) {
        var min = players[0];
        float minDistance = float.MaxValue;
        foreach(var player in players) {
            var dist = Dist(player);
            if (dist < minDistance) {
                min = player;
                minDistance = dist;
            }
        }
        return min;

    }

    void UpdateBrain() {
        
        var playersInRange = base.TargetsInRange(20);
        if(playersInRange.Length > 0) {
            var closestPlayer = minPlayer(playersInRange);
            var dist = Dist(closestPlayer);
            //Debug.Log(dist);
            var dY = closestPlayer.transform.position.y - myTransform.position.y;
            if(dY > 1) {
                return;
            }
            if (dist < ATTACK_DISTANCE) {
                target = null;
                Attack(closestPlayer);
            } else {
                target = closestPlayer;
            }
        }
    }

    void Attack(PlayerState player) {
        player.changeHealth(-1);
    }

    void WalkTowards() {
        if(target == null) return;
        var source = target.transform.position;
        var destination = myTransform.position;
        source.y = 0;
        destination.y = 0;
        myTransform.position -= (destination-source).normalized *  Time.deltaTime * 2;
    }
}
