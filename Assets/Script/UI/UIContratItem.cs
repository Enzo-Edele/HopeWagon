using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIContratItem : MonoBehaviour
{

    [SerializeField] Image requireImage;
    [SerializeField] TMP_Text requireQtyText;
    [SerializeField] Image rewardImage;
    [SerializeField] TMP_Text rewardQtyText;

    public void Set(Sprite requireIcon, string requireText, Sprite rewardIcon, string rewardText)
    {
        requireImage.sprite = requireIcon;
        requireQtyText.text = requireText;
        rewardImage.sprite = rewardIcon;
        rewardQtyText.text = rewardText;
    }
}
