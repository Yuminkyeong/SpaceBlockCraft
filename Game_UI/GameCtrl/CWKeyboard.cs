using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWKeyboard : MonoBehaviour {

    public CWJoystickCtrl m_kJoy;

    private void Update()
    {
        if(Input.GetKeyDown( KeyCode.W))
        {
            m_kJoy.DirMove(new Vector2(0,1));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_kJoy.DirMove(new Vector2(0, -1));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_kJoy.DirMove(new Vector2(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_kJoy.DirMove(new Vector2(1, 0));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_kJoy.DirMove(new Vector2(-1, 1));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_kJoy.DirMove(new Vector2(1, 1));
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            m_kJoy.JoyStop();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            m_kJoy.JoyStop();
        }



        if (Input.GetKeyUp(KeyCode.W))
        {
            m_kJoy.JoyStop();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            m_kJoy.JoyStop();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            m_kJoy.JoyStop();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            m_kJoy.JoyStop();
        }

    }

}
