using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 이미지를 부채꼴로 만들어 준다 
public class CircleValue : MonoBehaviour
{

    // 12 방향으로 시작한다
    // 12 방향에서 시계 방향으로 돈다

    public int m_Count; // 몇조각인가?
    public int m_Number;// 몇번째 도형인가?

    void Make()
    {
        Image kImage = GetComponent<Image>();
        kImage.type = Image.Type.Filled;
        float roll = 0;
        float f1 = 1f/(float)m_Count;
        kImage.fillAmount = f1;
        float f2 = 360/ (float)m_Count;
        roll = 180 - m_Number * f2;
        transform.eulerAngles = new Vector3(0, 0, roll);
    }
    private void OnEnable()
    {
        Make();
    }

}
