using System;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    private int mana = 100;
    private int health = 100;

    public static Action<int> manaChanged;
    public static Action<int> healthChanged;

    public void changeMana(int offset) {
        mana += offset;
        manaChanged(mana);
    }

    public void changeHealth(int offset) {
        health += offset;
        healthChanged(health);
    }
}
