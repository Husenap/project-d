using System;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    private int mana = 100;

    public static Action<int> manaChangedAction;

    public void changeMana(int offset) {
        mana += offset;
        manaChangedAction(mana);
    }
}
