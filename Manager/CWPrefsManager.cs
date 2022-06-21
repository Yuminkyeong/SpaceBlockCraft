using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWPrefsManager : CWManager<CWPrefsManager>
{


    private string _LoginType;
    private string _LoginID;

    private int _Language;
    private float _BGMVolumn;
    private float _FXVolumn;

    


    public void LoadPrefs()
    {
      


        if (PlayerPrefs.HasKey("Language"))
        {
            SetLanguage(PlayerPrefs.GetInt("Language"));
        }
        else
        {
            int num = CWLocalization.Instance.m_LanguageList.FindIndex(Language => Language == Application.systemLanguage);
            if(num==-1)
            {

                num = 1;// 조건에 맞지 않으면 무조건 영어 
            }
            SetLanguage(num);
        }

        if (PlayerPrefs.HasKey("BGM"))
        {
            SetBGM(PlayerPrefs.GetFloat("BGM"));
        }
        else
        {
            SetBGM(1f);
        }
        if (PlayerPrefs.HasKey("FX"))
        {
            SetFX(PlayerPrefs.GetFloat("FX"));
        }
        else
        {
            SetFX(0.2f);
        }
    }


    public void SetLanguage(int _Num)
    {
        _Language = _Num;

        PlayerPrefs.SetInt("Language", _Num);
    }

    public int GetLanguage()
    {
        return _Language;
    }

    public void SetBGM(float _Volumn)
    {
        _BGMVolumn = _Volumn;
        PlayerPrefs.SetFloat("BGM", _Volumn);
    }

    public float GetBGM()
    {
        return _BGMVolumn;
    }
    public void SetFX(float _Volumn)
    {
        _FXVolumn = _Volumn;
        PlayerPrefs.SetFloat("FX", _Volumn);

    }

    public float GetFX()
    {
        return _FXVolumn;
    }

}
