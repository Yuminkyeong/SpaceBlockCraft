using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBall : MonoBehaviour
{
    public float rotSpeed = 80f;

    float CalAngle(float a,float b)
    {
        float v= (360 + a + b)%360;
        return v;
    }

    float rrrX = 0;
    float rrrY = 0;
    public bool Flag = false;

    Vector3 vdir = new Vector3(1,0,1);
    private void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        Vector3 vv= transform.eulerAngles;
        vv.x = CalAngle(vv.x,rotY);//+= rotY;
        vv.z = CalAngle(vv.z, rotY);//+= rotY; += rotY;
        vv.y = CalAngle(vv.y, -rotX);//+= rotY;-= rotX;
                                     // transform.eulerAngles = vv;

        //rrrX -= rotX;
        //rrrY += rotY;

        rrrX = CalAngle(rrrX, -rotX);

        Quaternion q1 = Quaternion.AngleAxis(rrrX, Vector3.up);

        if (vv.y > 180)
        {
            rrrY = CalAngle(rrrY, -rotY);
        }
        else
        {
            rrrY = CalAngle(rrrY, rotY);
        }


        Quaternion q3 = Quaternion.AngleAxis(rrrY, vdir);


        transform.rotation = q1*q3;

    }

}
