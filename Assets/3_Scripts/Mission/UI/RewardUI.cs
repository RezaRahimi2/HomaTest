using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    [SerializeField]private Image m_iconImage;
    [SerializeField]private TextMeshProUGUI m_awardText;

    public void Set(Sprite icon,int award)
    {
        m_iconImage.sprite = icon;
        m_awardText.text = award.ToString();
    }
}
