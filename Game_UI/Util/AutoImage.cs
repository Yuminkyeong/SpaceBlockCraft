using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class AutoImage : MonoBehaviour
{
    const int ScreenX = 1920;
    const int ScreenY = 1080;
    const int JeplineX = 960;
    const int JeplineY = 540;



    public float Startx;
    public float Starty;
    public float Width;
    public float Height;


    public float RealWidth;
    public float RealHeight;

    public Color Color = Color.white;

    public enum UITYPE {NONE,SPRITE,BUTTON,RAWIMAGE,TEXT };
    public UITYPE UIType=UITYPE.SPRITE;


    public Sprite m_kSprite;
    

    [ContextMenu("Do FixPostion")]
    void DoFixPostion()
    {
        Debug.Log("Fix Postion");


        RectTransform tt = GetComponent<RectTransform>();
        float rateX = ScreenX / JeplineX;
        float rateY = ScreenY / JeplineY;
        float dx = Width*rateX;
        float dy = Height * rateY;

        float sx = Startx*rateX+  dx / 2- ScreenX / 2;

        float sy = ScreenY / 2 - dy / 2 - Starty * rateY;

        Transform tParent=transform.parent;
        Canvas cParent = gameObject.GetComponentInParent<Canvas>();
        transform.parent = cParent.transform;
        tt.anchoredPosition3D = new Vector3(sx,sy,0);

        if (RealWidth != 0) dx = RealWidth * rateX; 
        if (RealHeight != 0) dy = RealHeight * rateY ;

        tt.sizeDelta = new Vector2(dx,dy);
        transform.parent = tParent;


        if (UIType==UITYPE.SPRITE|| UIType == UITYPE.BUTTON)
        {
            Image cs = gameObject.GetComponent<Image>();
            if (m_kSprite != null) name = m_kSprite.name;

            if (cs == null)
                 cs = gameObject.AddComponent<Image>();
            if(cs!=null)
            {
                if (m_kSprite != null) cs.sprite = m_kSprite;
                cs.type = Image.Type.Sliced;
                cs.raycastTarget = false;
            }

        }

        if (UIType == UITYPE.BUTTON)
        {
            Image cs = gameObject.GetComponent<Image>();
            if (cs != null)
                cs.raycastTarget = true;
            Button bt = gameObject.GetComponent<Button>();
            if (bt == null)
            {
                bt=gameObject.AddComponent<Button>();
            }
        }
        if (UIType == UITYPE.RAWIMAGE)
        {
            RawImage rr = gameObject.GetComponent<RawImage>();
            if(rr==null)
            {
                rr=gameObject.AddComponent<RawImage>();
            }
            rr.raycastTarget = false;
        }
        if(UIType == UITYPE.TEXT)
        {
            Text txt = gameObject.GetComponent<Text>();
            if(txt==null)
            {
                txt = gameObject.AddComponent<Text>();
            }
            txt.raycastTarget = false;
            txt.resizeTextForBestFit = true;
            txt.color = Color;
            txt.alignment = TextAnchor.MiddleCenter;
        }


    }
    private void Awake()
    {
        Destroy(this);
    }
}
