using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWGyrosope : MonoBehaviour {


    public UILabel mText;

    private int initialOrientationX;
    private int initialOrientationY;
    private int initialOrientationZ;

    public float m_fRotSpeed = 500f;

    
    

    public UISlider m_kSlider;


    void Start () {

        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.01f;

        
        InitCtrl();
    }
    
    string GetHelp(float ff)
    {
        if (ff == 0) return "Center";
        if (ff < 0) return "Left";
        return "Right";
    }

    
    void Update () {


        mText.text = string.Format("{0:F2} {1:F2} {2:F2}  ", m_fFirstDelta-Input.gyro.attitude.x, m_fFirstDelta, Input.gyro.attitude.x);
        CtrlRun();
    }

    #region 이벤트 개념 


    // 컨트롤 조작 
    
    float m_fFirstDelta = 0;
    const float ZERODELTA = 0.016f; // 0.026f;
    void InitCtrl()
    {
        m_fFirstDelta = Input.gyro.attitude.x;
    }
    bool GetKeyRightDown()
    {
        float fdelta = m_fFirstDelta - Input.gyro.attitude.x;
        if (fdelta > ZERODELTA) return true;
        return false;
    }
    bool GetKeyLeftDown()
    {
        float fdelta = m_fFirstDelta - Input.gyro.attitude.x;
        if (fdelta < -ZERODELTA) return true;
        return false;
    }
    void CtrlRun()
    {
        //if(GetKeyRightDown())
        //{
        //    CWObject kObject = CWHeroManager.Instance.GetHero();
        //    if (kObject == null) return;
        //    CWAction kAction;
        //    kAction = kObject.GetComponent<CWAction>();
        //    kAction.RotateYaw(m_fRotSpeed);
        //}
        //if (GetKeyLeftDown())
        //{
        //    CWObject kObject = CWHeroManager.Instance.GetHero();
        //    if (kObject == null) return;
        //    CWAction kAction;
        //    kAction = kObject.GetComponent<CWAction>();
        //    kAction.RotateYaw(-m_fRotSpeed);

        //}
        //m_fFirstDelta = Input.gyro.attitude.x;

    }


    #endregion



}
