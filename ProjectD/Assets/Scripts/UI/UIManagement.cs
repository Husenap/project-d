using TMPro;
using UnityEngine;

public class UIManagement : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI manaLabel;

    [SerializeField]
    private TextMeshProUGUI healthLabel;

    private void Start() {
        PlayerState.manaChanged += updateMana;
        PlayerState.healthChanged += updateHealth;
    }

    public void updateMana(int current) {
        if (manaLabel) {
            manaLabel.text = $"Mana: {current}";
        }
    }

    public void updateHealth(int current) {
        if (healthLabel) {
            healthLabel.text = $"Health: {current}";
        }
    }
}
