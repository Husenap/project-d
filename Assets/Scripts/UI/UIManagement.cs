using TMPro;
using UnityEngine;

public class UIManagement : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI manaLabel;

    private void Start() {
        PlayerState.manaChangedAction += updateMana;
    }

    public void updateMana(int current) {
        if (manaLabel) {
            manaLabel.text = $"Mana: {current}";
        }
    }
}
