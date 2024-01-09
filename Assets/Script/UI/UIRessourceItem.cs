using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRessourceItem : MonoBehaviour
{
    public TMP_Text qtyRessource;
    public Image icon, input, output;
    [SerializeField] Sprite inputSprite, outputSprite;

    public void SetItem(int ID, int qty, bool outputState, bool inputState)
    {
        icon.sprite = GameManager.Instance.ressourceTypes[ID].sprite;//
        qtyRessource.text = "" + qty;
        if (inputState)
            input.sprite = inputSprite;
        else
            input.gameObject.SetActive(false);
        if(outputState)
            output.sprite = outputSprite;
        else
            output.gameObject.SetActive(false);
    }
}
