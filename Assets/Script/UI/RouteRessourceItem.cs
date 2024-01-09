using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouteRessourceItem : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text qtyText;

    public void Set(int ressourceID, int qty)
    {
        icon.sprite = GameManager.Instance.ressourceTypes[ressourceID].sprite;
        qtyText.text = "" + qty;
    }
}
