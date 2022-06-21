using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayPos : MonoBehaviour
{
    public enum Anchor
    {
        TopLeft = 0,
        Top = 1,
        TopRight = 2,
        Left = 3,
        Center = 4,
        Right = 5,
        BottomLeft = 6,
        Bottom = 7,
        BottomRight = 8
    }


    private float Xanchor = 0.0f, Yanchor = 0.0f;

    public Anchor UIAnchor;
    public float Xoff = 0.0f, Yoff = 0.0f;
    public float Xcor = 0.0f, Ycor = 0.0f, Zcor = 0.0f;
    public float Xrot = 0.0f, Yrot = 0.0f, Zrot = 0.0f;
    public float Xsc = 1.0f, Ysc = 1.0f, Zsc = 1.0f;

    public bool m_bFollow=false;

    public float m_fSpeed = 10f;
    public GameObject m_gTarget;
    void SetAnchor()
    {
        Xanchor = 0.0f;
        Yanchor = 0.0f;
        switch (UIAnchor)
        {
            case Anchor.TopLeft:
                Xanchor = this.GetComponent<RectTransform>().sizeDelta.x / 2;
                Yanchor = -this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            case Anchor.Top:
                Yanchor = -this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            case Anchor.TopRight:
                Xanchor = -this.GetComponent<RectTransform>().sizeDelta.x / 2;
                Yanchor = -this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            case Anchor.Left:
                Xanchor = this.GetComponent<RectTransform>().sizeDelta.x / 2;
                break;
            case Anchor.Center:
                break;
            case Anchor.Right:
                Xanchor = -this.GetComponent<RectTransform>().sizeDelta.x / 2;
                break;
            case Anchor.BottomLeft:
                Xanchor = this.GetComponent<RectTransform>().sizeDelta.x / 2;
                Yanchor = this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            case Anchor.Bottom:
                Yanchor = this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            case Anchor.BottomRight:
                Xanchor = -this.GetComponent<RectTransform>().sizeDelta.x / 2;
                Yanchor = this.GetComponent<RectTransform>().sizeDelta.y / 2;
                break;
            default:
                break;
        }
    }

    Vector2 m_vPos = Vector2.zero;
    void OnAlways()
    {
        if (!m_gTarget) return;
        if (Camera.main == null) return;
        Canvas kCanvas = GetComponentInParent<Canvas>();

        RectTransform CanvasRect = kCanvas.GetComponent<RectTransform>();
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(new Vector3(m_gTarget.transform.position.x + Xcor,
                                                                                m_gTarget.transform.position.y + Ycor,
                                                                                m_gTarget.transform.position.z + Zcor));

        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + Xanchor + Xoff,
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + Yanchor + Yoff);


        //GetComponent<RectTransform>().anchoredPosition =
        m_vPos = new Vector2(WorldObject_ScreenPosition.x,
                                               WorldObject_ScreenPosition.y);


    }
    void Update()
    {
        if (m_gTarget == null) return;
        SetAnchor();
        OnAlways();

        if(m_bFollow)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, m_vPos, 1.0f - Mathf.Exp(-m_fSpeed * Time.deltaTime)); //1.0f - Mathf.Exp(-m_fSpeed * Time.deltaTime)
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = m_vPos;
        }
    }
}
