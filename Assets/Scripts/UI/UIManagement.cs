using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI mana;

    public void updateMana(int current) {
        if(this.mana != null) {
            this.mana.text = $"Mana: {current}";
        }
        
    }
}
