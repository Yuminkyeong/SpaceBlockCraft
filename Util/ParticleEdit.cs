using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEdit : MonoBehaviour
{

    [HideInInspector]
    public float scaleFactor = 1.0f; // 파티클 크기

    [HideInInspector]
    public float RotateFactor = 1.0f; //속도 

    [HideInInspector]
    public float SimulFactor = 1.0f; //속도 


    public void ModeChange()
    {

        ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);

        for(int i=0;i<pp.Length;i++)
        {
            ParticleSystem.MainModule pm= pp[i].main;
            pm.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }
    public void ParticleScaleChange()
    {
        ParticleSystem[] particleSystemList = GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < particleSystemList.Length; i++)
        {
            particleSystemList[i].gameObject.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    public void ParticleSpeedChange()
    {

        {
            ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < pp.Length; i++)
            {
                ParticleSystem.MainModule pm = pp[i].main;
                ParticleSystem.MinMaxCurve vvv = pm.startSpeed;
                vvv.constantMin *= scaleFactor;
                vvv.constantMax *= scaleFactor;
                vvv.constant *= scaleFactor;
                pm.startSpeed = vvv;
            }

        }

        {
            ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < pp.Length; i++)
            {
                ParticleSystem.VelocityOverLifetimeModule pm = pp[i].velocityOverLifetime;

                ParticleSystem.MinMaxCurve vvv = pm.speedModifier;
                vvv.constantMin *= scaleFactor;
                vvv.constantMax *= scaleFactor;
                vvv.constant *= scaleFactor;
                pm.speedModifier = vvv;

            }
        }
        {
            ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < pp.Length; i++)
            {
                ParticleSystem.MainModule pm = pp[i].main;
                pm.simulationSpeed *= scaleFactor;
            }
        }

    }



    public void ParticleRotateSpeed()
    {
        ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < pp.Length; i++)
        {
            ParticleSystem.VelocityOverLifetimeModule pm = pp[i].velocityOverLifetime;
            pm.speedModifier = RotateFactor;

        }

    }
    public void ParticleSimulSpeed()
    {
        ParticleSystem[] pp = GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < pp.Length; i++)
        {
            ParticleSystem.MainModule pm = pp[i].main;
            pm.simulationSpeed = SimulFactor;
        }
    }

}
