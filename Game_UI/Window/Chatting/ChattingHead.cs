using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChattingHead : MonoBehaviour
{

    public Text m_kName1;
    public Text m_kName2;
    public Text m_kMessage1;
    public Text m_kMessage2;

    private void OnEnable()
    {
        if (CWChattingManager.Instance == null) return;
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        if(m_kName1) m_kName1.text = "";
       if (m_kName2) m_kName2.text = "";
        if (m_kMessage1) m_kMessage1.text = "";
        if (m_kMessage2) m_kMessage2.text = "";
        while (true)
        {

            int tcnt= CWChattingManager.Instance.m_kList.Count;
            if(tcnt>0)
            {
                if (m_kName1) m_kName1.text = CWChattingManager.Instance.m_kList[tcnt - 1].Name;

                if (m_kMessage1) m_kMessage1.text = CWChattingManager.Instance.m_kList[tcnt - 1].ChattingMsg;
                if(tcnt>1)
                {
                    if (m_kName1) m_kName2.text = CWChattingManager.Instance.m_kList[tcnt - 2].Name;
                    if (m_kMessage1) m_kMessage2.text = CWChattingManager.Instance.m_kList[tcnt - 2].ChattingMsg;
                }

            }


            yield return new WaitForSeconds(1f);
        }
    }
}
