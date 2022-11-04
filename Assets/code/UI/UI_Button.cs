using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [HideInInspector]
    public bool is_down = false;
    public UnityEvent on_press;
    public UnityEvent on_click;
    public void OnPointerDown(PointerEventData eventData)
    {
        is_down = true;
        on_press.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        is_down = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        is_down = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        on_click.Invoke();
    }
}