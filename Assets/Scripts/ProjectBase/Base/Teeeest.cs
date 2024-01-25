using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestTest
{
    public void Update()
    {
        Debug.Log("TestTest");
    }
}
public class Test02 : MonoBehaviour
{
    private void Start()
    {
        TestTest t = new TestTest();
        MonoMgr.Instance.AddUpdateListener(t.Update);
    }
    
}
