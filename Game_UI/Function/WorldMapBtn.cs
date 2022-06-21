using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class WorldMapBtn : MonoBehaviour, IPointerDownHandler
{
    public int m_nPlanet;
    public void OnPointerDown(PointerEventData data)
    {
        Space_Map.Instance.MoveStage((m_nPlanet-1)*6);
        WorldMapDlg.Instance.Close();
    }
}
