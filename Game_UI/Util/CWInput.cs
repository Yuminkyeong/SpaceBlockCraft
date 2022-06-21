using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CWInput : InputField
{
    public static bool IsFocus=false;
    public override void OnSelect(BaseEventData eventData)
    {
        IsFocus = true;
        base.OnSelect(eventData);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        IsFocus = false;
        base.OnDeselect(eventData);
    }

}
