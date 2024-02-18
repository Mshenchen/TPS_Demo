using System;

public class Player {
	//id
	public string id = "";
	//指向ClientState
	public ClientState state;
	//构造函数
	public Player(ClientState state){
		this.state = state;
	}
	//临时数据，如：坐标
	public float x; 
	public float y; 
	public float z;
	public float ex;
	public float ey;
	public float ez;
	public int roomId = -1;
	public int camp = 1;
	public int hp = 100;
    //动画参数
    public float speedValue = 0f;
    public bool jumpValue = false;
    public bool groundedValue = false;
    public bool freefallValue = false;
    public float motionSpeedValue = 0f;
    //数据库数据
    public PlayerData data;

	//发送信息
	public void Send(MsgBase msgBase){
		NetManager.Send(state, msgBase);
	}

}


