//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using Random = UnityEngine.Random;

//public class NetDemo : MonoBehaviour
//{
//    public GameObject humanPrefab;
//    public BaseHuman myHuman;
//    public Dictionary<string, BaseHuman> otherHumans;
//    private void Start()
//    {
//        //NetManager.AddListener("Enter", OnEnter);
//        //NetManager.AddListener("Move", OnMove);
//        //NetManager.AddListener("Leave", OnLeave);
//        //NetManager.AddListener("List", OnList);
//        //NetManager.Connect("127.0.0.1", 8888);
//        ////Ìí¼Ó½ÇÉ«
//        //GameObject obj = (GameObject)Instantiate(humanPrefab);
//        //float x = Random.Range(-5, 5);
//        //float z = Random.Range(-5, 5);
//        //obj.transform.position = new Vector3(x,0,z);
        
//    }


//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            NetManager.Send("Enter|127.0.0.1,100,200,300,45");
//        }
//        NetManager.Update();
//    }
//    private void OnList(string msgArgs)
//    {
//        Debug.Log("OnList " + msgArgs);
//        string[] split = msgArgs.Split(',');
//        int count = (split.Length - 1)/6;
//        for (int i = 0; i < count; i++)
//        {
//            string desc = split[i * 6 + 0];
//            float x = float.Parse(split[i * 6 + 1]);
//            float y = float.Parse(split[i*6 + 2]);
//            float z = float.Parse(split[i*6 + 3]);
//            float eulY = float.Parse(split[i * 6 + 4]);
//            int hp = int.Parse(split[i * 6 + 5]);
//            if (desc == NetManager.GetDesc())
//            {
//                continue;
//            }
//            GameObject obj = (GameObject)Instantiate(humanPrefab);
//            obj.transform.position = new Vector3(x,y,z);
//            obj.transform.eulerAngles = new Vector3(0,eulY,0);
            
//        }
//    }
//    private void OnEnter(string str)
//    {
//        Debug.Log("OnEnter" + str);
//    }
//    private void OnMove(string str)
//    {
//        Debug.Log("OnMove" + str);
//    }
//    private void OnLeave(string str)
//    {
//        Debug.Log("OnLeave" + str);
//    }
//}
