using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.ExpressionParser;
public class CWParse : CWSingleton<CWParse>
{
    ExpressionParser m_kParse = new ExpressionParser();
    public void AddData(string szVal,string szResult)
    {
        m_kParse.AddConst(szVal, () => double.Parse(szResult));
    }
    public bool IsValue(string szParam)
    {
        Expression exp = m_kParse.EvaluateExpression(szParam);
        if (exp.Value == 0) return false;
        return true;
    }
    
}
