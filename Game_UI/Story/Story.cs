using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story : MonoBehaviour
{


    public void ReSort()
    {
        Test_mk kk = GetComponentInChildren<Test_mk>();
        kk.ReSort();
    }
    public void OrderNumber()
    {
        Test_mk kk = GetComponentInChildren<Test_mk>();
        kk.OrderNumber();

    }
    public void Insert(int num)
    {
        Test_mk kk = GetComponentInChildren<Test_mk>();
        kk.Insert(num);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
