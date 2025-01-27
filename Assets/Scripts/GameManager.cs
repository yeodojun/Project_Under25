using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager :MonoBehaviour
{
    [SerializeField]
    private GameObject SettingPanel;
    [SerializeField]
    private TextMeshProUGUI text1;
    public void ShowSettingPanel() {
        SettingPanel.SetActive(true);
    }
    public void HideSettingPanel() {
        SettingPanel.SetActive(false);
    }
    
}
