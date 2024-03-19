using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDestDropdownItem : MonoBehaviour
{
    public void ChangeDestination(TMP_Dropdown dropdown)
    {
        GameManager.Instance.gameUI.ChangeDestination(dropdown);
    }
}
