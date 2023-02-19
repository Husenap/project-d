using UnityEngine;

public class PlayerState : MonoBehaviour {
    private int _mana = 100;
    [SerializeField]
    UIManagement management;

    public void changeMana(int offset) {
        _mana += offset;
        management.updateMana(this._mana);
    }

}
