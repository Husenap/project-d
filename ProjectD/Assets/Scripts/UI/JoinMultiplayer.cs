using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinMultiplayer : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField ip;
   
    public void TriggerJoin() {
        Debug.Log("Join " + ip.text);

    }
}
