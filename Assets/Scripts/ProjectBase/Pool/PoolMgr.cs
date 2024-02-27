using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() { };
        PushObj(obj);
    }
    //将对象放入对象池
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = fatherObj.transform;
    }
    //拿出对象
    public GameObject GetObj()
    {
        GameObject obj = null;
        obj = poolList[0];
        obj.SetActive(true);
        //断开父子关系
        obj.transform.parent = null;
        return obj;
    }
}
/// <summary>
/// 缓存池模块
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;
    //拿出对象
    public void GetObj(string name,UnityAction<GameObject> callBack)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        else
        {
            ResMgr.Instance.LoadAsync<GameObject>(name,(o)=>
            {
                o.name = name;
                callBack(o);
            });
            
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //obj.name = name;
        }
    }
    //将对象放入对象池
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name,new PoolData(obj,poolObj));
        }
    }
    /// <summary>
    /// 清空缓存池
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
