using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayer : MonoBehaviour
{
	private void Start()
	{
		GameObject playerObj = new GameObject("MyPlayer");
		BasePlayer basePlayer = playerObj.AddComponent<BasePlayer>();
		basePlayer.Init("Player");	
	}
}
