using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class CWText : Text
{
    public enum TEXTTYPE {NORMAL,NUMBER,TRANSTEXT};
    public enum FONTTYPE {NORMAL,NUMBER,HIGHWAY, test_mk };

    public FONTTYPE m_kFontType;
    public TEXTTYPE m_kTextType;

    string m_szLocalText = "";


   
    public void OnChange()
    {
        if (m_kTextType == TEXTTYPE.TRANSTEXT)
        {
            if (CWLocalization.Instance)
            {

                base.text = CWLocalization.Instance.GetLanguage(m_szLocalText);
            }
                
        }

    }
    protected override void OnEnable()
    {
        if(!CWLib.IsString(m_szLocalText))
        {
            m_szLocalText = base.text;
        }
            
        OnChange();

        base.OnEnable();
    }

    public override string text
    {
        get
        {

            if (m_kTextType == TEXTTYPE.NUMBER)
            {
                int num = CWLib.ConvertInt(base.text);
                if (num < 1000)
                {
                    return base.text;
                    //base.text = value;
                }
                else
                {
                    return string.Format("{0:0,0}", CWLib.ConvertInt(base.text));
                }

            }
            if (m_kTextType == TEXTTYPE.TRANSTEXT)
            {
                string str = base.text;
                if (CWLocalization.Instance)
                {
                    str= CWLocalization.Instance.GetLanguage(base.text);
                }
                ChangeNameUI cs = GetComponent<ChangeNameUI>();
                if (cs!=null)
                {
                    return cs.Change(str);

                }
                return str;
            }

            return base.text;

        }
        set
        {
            base.text = value;
            m_szLocalText = base.text;
        }

    }
}
