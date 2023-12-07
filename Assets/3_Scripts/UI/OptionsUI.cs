using System.Collections;
using System.Collections.Generic;
using Events;
using Mission;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    CustomToggle colorblindToggle;
    [SerializeField]
    CustomToggle vibrationToggle;
    [SerializeField]
    private GameObject m_missionButton;

    Animator animator;
    bool isOpen = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        if (!RemoteConfig.BOOL_MISSIONS_ENABLED)
        {
            m_missionButton.SetActive(false);
        }
        
        animator.speed = 1.0f / Time.timeScale;
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
        colorblindToggle.SetEnabled(SaveData.CurrentColorList == 1);
        vibrationToggle.SetEnabled(SaveData.VibrationEnabled == 1);
    }
    
    public void Toggle()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }

    public void OnColorblindClick(bool value)
    {
        if (SaveData.CurrentColorList == 1 != value) {
            SaveData.CurrentColorList = value ? 1 : 0;
            TileColorManager.Instance.SetColorList(SaveData.CurrentColorList);
        }
    }

    public void OnVibrationClick(bool value)
    {
        if (SaveData.VibrationEnabled == 1 != value) {
            SaveData.VibrationEnabled = value ? 1 : 0;
            if (value)
                Handheld.Vibrate();
        }
    }
    
    public void OnMissionButtonClick(bool value)
    {
        MissionUI.Instance.OnOpen();
        Toggle();
    }
}
