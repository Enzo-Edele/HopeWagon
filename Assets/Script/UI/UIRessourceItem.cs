using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIRessourceItem : MonoBehaviour
{
    public TMP_Text nameRessource;
    public TMP_Text qtyRessource;

    public void SetItem(string name, int qty)
    {
        nameRessource.text = name;
        qtyRessource.text = "" + qty;
    }
}
