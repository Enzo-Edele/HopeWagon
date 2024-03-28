using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;
    [Multiline()]
    public string header;
    /*
    //fancy delay
    bool isHover = false;
    bool show = false;
    float hoverTimer;

    private void Update()
    {
        if(isHover)
            hoverTimer += Time.deltaTime;
        if (hoverTimer > 1.0f && !show)
        {
            TooltipSystem.Show(content, header);
            show = true;
        }
    }*/

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(content, header);
        //isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
        //isHover = false;
        //show = false;
        //hoverTimer = 0.0f;
    }
}
