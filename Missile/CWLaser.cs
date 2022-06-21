using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWLaser : MonoBehaviour
{

    public GameObject beamLineRendererPrefab;
    public GameObject beamStartPrefab;
    public GameObject beamEndPrefab;

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;

    [Header("Adjustable Variables")]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
    public float textureLengthScale = 3; //Length of the beam texture

    Transform parentForm;
    CWObject parentObject;

    Vector3 startTagetPos = Vector3.zero;


    public bool m_bBlockDigg=false;// 블록캐기 용도 

    int damage;// 1초 4번씩 데미지를 준다 
    // 데미지 개념
    IEnumerator IDamagePlay()
    {
        while(true)
        {
            CWAirObject targetObject;
            targetObject = FindTarget();

            if (targetObject)
                targetObject.Hit(parentObject, damage);
            yield return new WaitForSeconds(0.25f);
        }
    }


    CWAirObject FindTarget()
    {
        Vector3 dir = parentObject.transform.forward;
        RaycastHit hit;
        int nMask ;//10만 디텍트 
        if (parentObject==CWHero.Instance)
        {
            nMask = 1 << LayerMask.NameToLayer("Detect");
        }
        else
        {
            nMask = 1 << LayerMask.NameToLayer("Hero");
        }
        if (Physics.Raycast(parentForm.position, dir, out hit, Mathf.Infinity, nMask))
        {
            CWAirObject Aobject=hit.collider.GetComponent<CWAirObject>();
            return Aobject;
        }
        return null;

    }
    public void BlockBegin(Transform tparent,Vector3 vTargetpos)
    {
        m_bBlockDigg = true;
        // 개염 1초 동안
        parentObject = null;
        parentForm = tparent;
        transform.position = tparent.position;

        beamStart = Instantiate(beamStartPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        beamEnd = Instantiate(beamEndPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        beam = Instantiate(beamLineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        line = beam.GetComponent<LineRenderer>();

        startTagetPos = vTargetpos;
        startTagetPos.x += 0.5f;
        startTagetPos.y += 0.5f;
        startTagetPos.z += 0.5f;
        

    }
    public void Begin(Transform tparent, CWObject parent,int tdamage,bool bBlockdigg=false)
    {

        m_bBlockDigg = bBlockdigg;
        // 개염 1초 동안
        damage = tdamage;
        parentObject = parent;
        parentForm = tparent;
        transform.position = tparent.position;

        beamStart = Instantiate(beamStartPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        beamEnd = Instantiate(beamEndPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        beam = Instantiate(beamLineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        line = beam.GetComponent<LineRenderer>();

       
        if (parentObject == null) return;
        CWAirObject targetObject = null;
        targetObject = FindTarget();
        StartCoroutine("IDamagePlay");
        if(targetObject)
        {
            startTagetPos = targetObject.transform.position;
        }
        else
        {
            
            startTagetPos = tparent.position + parentObject.transform.forward * 100f;
        }
        

    }
    private void OnDisable()
    {
        Stop();
    }
    public void Stop()
    {
        // 스톱 조건 : 유저가 움직이면 스톱이 된다 
        // 혹은 타겟이 순간적으로 많은 이동이 생겨도 취소가 된다

        if (beamStart == null) return;
        StopAllCoroutines();
        Destroy(beamStart);
        Destroy(beamEnd);
        Destroy(beam);
        beamStart = null;
        beamEnd = null;
        beam = null;
    }
    void ShootBeamInDir(Vector3 start, Vector3 dir,Vector3 end)
    {
#if UNITY_5_5_OR_NEWER
        line.positionCount = 2;
#else
		line.SetVertexCount(2); 
#endif
        line.SetPosition(0, start);
        beamStart.transform.position = start;


        beamEnd.transform.position = end;
        line.SetPosition(1, end);

        beamStart.transform.LookAt(beamEnd.transform.position);
        beamEnd.transform.LookAt(beamStart.transform.position);

        float distance = Vector3.Distance(start, end);
        line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
        line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
    }

    void BlockUpdate()
    {
        Vector3 tdir = startTagetPos - parentForm.position;
        tdir.Normalize();
        ShootBeamInDir(parentForm.position, tdir, startTagetPos);


    }
    
    void Update()
    {
        if (beamStart == null) return;
        if (m_bBlockDigg)
        {
            BlockUpdate();
            return;
        }
        
        RaycastHit hit;
        Vector3 dir = parentForm.forward;

        CWAirObject targetObject=null;
        if (!m_bBlockDigg)
        {
            targetObject = FindTarget();
            
            if (targetObject != null)
            {
                dir = targetObject.GetHitPos() - parentForm.position;
            }
        }
        else
        {
            dir = startTagetPos - parentForm.position;
        }

        dir.Normalize();

        int layer1 = 1 << LayerMask.NameToLayer("Detect");
        int layer2 = 1 << LayerMask.NameToLayer("BlockMap");
        int nMask = layer1| layer2;
        if(m_bBlockDigg)
        {
            nMask =  layer2;
        }
        

        Vector3 end = Vector3.zero;

        if (Physics.Raycast(parentForm.position, dir, out hit, Mathf.Infinity, nMask))
        {
            Vector3 tdir = hit.point - parentForm.position;
            end = hit.point - (dir.normalized * beamEndOffset);
            ShootBeamInDir(parentForm.position, tdir,end);
        }
        else
        {
            end = parentForm.position + (dir * 100);
            ShootBeamInDir(parentForm.position, dir,end);
        }
        if (targetObject != null)
        {
            float fdist = Vector3.Distance(startTagetPos, targetObject.GetPosition());
            if (fdist > 10)
            {
                Stop();// 거리가 바뀌면 정지
                return;
            }
        }

    }
}
