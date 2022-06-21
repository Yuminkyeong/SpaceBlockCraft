using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using UnityEngine.UI;
using DG.Tweening;

public class MoveTest : MonoBehaviour
{


    public GameObject m_dummy;
    public float m_Speed = 10f;
    Rigidbody m_Rigidbody;
    Vector3 m_Input = Vector2.zero;
    Vector3 m_vDir = Vector2.zero;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Debug.DrawRay(m_dummy.transform.position, transform.forward, Color.green);

        Vector3 forward = transform.TransformDirection(m_vDir) * 10;
        Debug.DrawRay(m_dummy.transform.position, forward, Color.green);
    }
    bool CheckWall(Vector3 vdir)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_dummy.transform.position, vdir);
        //int layer1 = 1 << LayerMask.NameToLayer("Detect");
        int layer2 = 1 << LayerMask.NameToLayer("BlockMap");
        int nMask = layer2;//| layer2;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {
            float fdist = Vector3.Distance(hit.point, transform.position);
            if(fdist<2f)
            {
                return true;
            }
            // 충돌
            

        }
        return false;

    }
    private void FixedUpdate()
    {
        Vector3 vdir = CWMath.CalYaw(transform.eulerAngles.y, m_Input);
        m_vDir = vdir;
        m_Rigidbody.MovePosition(transform.position + vdir * Time.deltaTime * m_Speed);
        // 개념
        // 가는 방향으로 레이를 발사후 충돌하면 점프

        
        if(CheckWall(vdir))
        {
            Jump();
        }
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

    }
    bool m_bJump = false;
    void Jump()
    {
        if (m_bJump) return;
        m_bJump = true;
        transform.DOMoveY(transform.position.y + 2f, 0.5f).OnComplete(()=> {
            m_bJump = false;
        });
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("c Enter!");
    //    if (m_Input == Vector3.zero) return;
    //    Jump();
    //    //transform.DOMoveY(transform.position.y + 2f, 0.5f);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    //Debug.Log("c Stay!");
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("c Exit!");
    //}
}
