using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_Directing : MonoBehaviour
{
    [Header("연출중인가?")]
    public bool Starting = false;// 연출 중
    // Start is called before the first frame update
    void Start()
    {
        Game_App.Instance.g_bDirecting = Starting;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
