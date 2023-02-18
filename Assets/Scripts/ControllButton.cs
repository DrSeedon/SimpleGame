using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ControllButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent action;

    public bool isActive = false;
    void Update()
    {
        if(isActive)
            action?.Invoke();
            
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isActive = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isActive = false;
    }
}
