using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelect : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private Selectable selectable = null;

    //Select element whenever it is hovered (avoids having one button selected and a different one hovered over)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable.transform.parent.GetComponent<VerticalLayoutGroup>() != null)
        {
            return;
        }
        selectable.Select();
    }
}

