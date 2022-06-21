using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;
using CWEnum;


public class EquipInvenSlot : SlotItemUI
{

    public override void OnSelectActive()
    {
        CWResourceManager.Instance.PlaySound("button1");
        base.OnSelectActive();
    }

}
