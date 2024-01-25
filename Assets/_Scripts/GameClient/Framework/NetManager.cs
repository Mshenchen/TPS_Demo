using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
	private static Socket socket;
	private static ByteArray readBuff;
	//写入队列
	private static Queue<ByteArray> writeQueue;
	private static bool isConnecting = false;
	private static bool isCloseing = false;
	//是否启用心跳
	public static bool isUsePing = true;
	//心跳间隔时间
	public static int pingInterval = 30;
	//上一次发送PING时间
	private static float lastPingTime = 0;
	//上一次收到PONG时间
	private static float lastPongTime = 0;
	public enum NetEvent
	{
		ConnectSucc = 1,
		ConnectFail = 2,
		Close = 3,
	}
	//事件委托类型
	public delegate void EventListener(string err);
	//事件监听列表
	private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
	public delegate void MsgListener(MsgBase msgBase);
	private static Dictionary<string ,MsgListener> msgListeners = new Dictionary<string , MsgListener>();
	//消息列表
	private static List<MsgBase> msgList = new List<MsgBase>();
	private static int msgCount = 0;
	private readonly static int MAX_MESSAGE_FIRE = 10;
	/// <summary>
	/// 添加事件监听
	/// </summary>
	/// <param name="netEvent">事件</param>
	/// <param name="listener">监听函数</param>
	public static void AddEventListener(NetEvent netEvent, EventListener listener)
	{
		if(eventListeners.ContainsKey(netEvent))
		{
			eventListeners[netEvent] += listener;
		}
		else
		{
			eventListeners[netEvent] = listener;
		}
    }
	/// <summary>
	/// 删除事件监听
	/// </summary>
	/// <param name="netEvent"></param>
	/// <param name="listener"></param>
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
			if (eventListeners[netEvent] == null)
			{
				eventListeners.Remove(netEvent);
			}
        }
        
    }
	//分发事件
	private static void FireEvent(NetEvent netEvent,string err)
	{
		if (eventListeners.ContainsKey(netEvent))
		{
			eventListeners[netEvent](err);
		}
	}
	public static void AddMsgListener(string msgName, MsgListener listener)
	{
		if (msgListeners.ContainsKey(msgName))
		{
			msgListeners[msgName] += listener;
			Debug.Log("msgName" + msgName);
		}
		else
		{
			msgListeners[msgName] = listener;
            Debug.Log("msgName" + msgName);
        }
	}
	public static void RemoveMsgListener(string msgName,MsgListener listener)
	{
		if (msgListeners.ContainsKey(msgName))
		{
			msgListeners[msgName] -= listener;
			if (msgListeners[msgName] == null)
			{
				msgListeners.Remove(msgName);
			}
		}
	}
	//分发消息
	private static void FireMsg(string msgName,MsgBase msgBase)
	{
		//Debug.Log("FireMsg===FireMsg");
		if (msgListeners.ContainsKey(msgName))
		{
			msgListeners[msgName](msgBase);
		}
	}
    public static void Connect(string ip,int port)
	{
		if(socket != null&&socket.Connected)
		{
			Debug.Log("Connect fail,already connected!");
			return;
		}
		if(isConnecting)
		{
			Debug.Log("Connect fail,isConnecting");
			return;
		}
		InitState();
		socket.NoDelay = true;
		isConnecting = true;
		socket.BeginConnect(ip, port,ConnectCallback,socket);
	}
	public static void Update()
	{
		MsgUpdate();
		PingUpdate();
    }
	public static void MsgUpdate()
	{
		if(msgCount == 0)
		{
			return;
		}
		for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
		{
			MsgBase msgBase = null;
			lock (msgList)
			{
				if (msgList.Count > 0)
				{
					msgBase = msgList[0];
					msgList.RemoveAt(0);
					msgCount--;
				}
			}
			//分发消息
			if(msgBase != null)
			{
				FireMsg(msgBase.protoName, msgBase);
				
			}
			else
			{
				break;
			}
		}
	}
	private static void InitState()
	{
		socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
		//接收缓冲区
		readBuff = new ByteArray();
		writeQueue = new Queue<ByteArray>();
		isConnecting = false;
		isCloseing = false;
		msgList = new List<MsgBase>();
		msgCount = 0;
		//上一次心跳的时间
		lastPingTime = Time.time;
		lastPongTime = Time.time;
		if (!msgListeners.ContainsKey("MsgPong"))
		{
			AddMsgListener("MsgPong", OnMsgPong);
		}
	}

	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			Debug.Log("Socket Connect Succ ");
			FireEvent(NetEvent.ConnectSucc, "");
			isConnecting = true;
			//开始接收
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
			Debug.Log("ReceiveCallback=====End");
		}
		catch (SocketException ex)
		{

			Debug.Log("Socket Connect fail "+ex.ToString());
			FireEvent(NetEvent.ConnectFail, ex.ToString());
			isConnecting = false;
		}
	}

    private static void ReceiveCallback(IAsyncResult ar)
    {
	    
		try
		{
			//Debug.Log("ReceiveCallback===ReceiveCallback");
			Socket socket = (Socket)ar.AsyncState;
			int count = socket.EndReceive(ar);
			if (count == 0)
			{
				Close();
				return;
			}
			readBuff.writeIdx += count;
			//处理二进制消息
			OnReceiveData();
			if (readBuff.remain < 8)
			{
				readBuff.MoveBytes();
				readBuff.ReSize(readBuff.length * 2);
			}
			socket.BeginReceive(readBuff.bytes,readBuff.writeIdx,readBuff.remain,0, ReceiveCallback, socket);
		}
		catch (SocketException ex)
		{
			Debug.Log("Socket Receive fail" + ex.ToString());
		}
    }

    private static void OnReceiveData()
    {
        if(readBuff.length<=2)
		{
			return;
		}
		//获取消息体长度
		int readIdx  = readBuff.readIdx;
		byte[] bytes = readBuff.bytes;
		short bodyLength = (short)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
		if (readBuff.length < bodyLength + 2)
		{
			return;
		}
		readBuff.readIdx += 2;
		//解析协议名
		int nameCount = 0;
		string protoName = MsgBase.DecodeName(readBuff.bytes,readBuff.readIdx,out nameCount);
		if(protoName == "")
		{
			Debug.Log("OnReceiveData MsgBase.DecodeName fail");
			return;
		}
		readBuff.readIdx += nameCount;
		//解析协议体
		int bodyCount = bodyLength - nameCount;
		MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
		readBuff.readIdx += bodyCount;
		readBuff.CheckAndMoveBytes();
		//添加到消息队列
		lock (msgList)
		{
			msgList.Add(msgBase);
		}
		msgCount++;
		//继续读取消息
		if(readBuff.length>2)
		{
			OnReceiveData();
		}
    }

    //关闭连接
    public static void Close()
	{
		if(socket == null || !socket.Connected)
		{
			return;
		}
		if(isConnecting)
		{
			return;
		}
		//如果还有数据在发送
		if(writeQueue.Count > 0)
		{
			isCloseing = true;
		}
		else
		{
			socket.Close();
			FireEvent(NetEvent.Close, "");
		}
	}
    //发送数据
    public static void Send(MsgBase msg)
    {
	    //Debug.Log("isConnecting："+isConnecting);
	    //Debug.Log("isCloseing："+isCloseing);
        if (socket == null || !socket.Connected)
		{
			return;
		}
		if(!isConnecting) { return; }
		if(isCloseing) { return; }
		//数据编码
		byte[] nameBytes = MsgBase.EncodeName(msg);
		byte[] bodyBytes = MsgBase.Encode(msg);
		int len = nameBytes.Length + bodyBytes.Length;
		byte[] sendBytes = new byte[2+len];
		sendBytes[0] = (byte)(len % 256);
		sendBytes[1] = (byte)(len / 256);
		//组装名字
		Array.Copy(nameBytes,0, sendBytes, 2,nameBytes.Length);
		Array.Copy(bodyBytes,0,sendBytes,2+nameBytes.Length,bodyBytes.Length);
		//Debug.Log("ByteArray.send");
		//写入队列
		ByteArray ba = new ByteArray(sendBytes);
		
		int count = 0;
		lock (writeQueue)
		{
			writeQueue.Enqueue(ba);
			count = writeQueue.Count;
		}
		if(count == 1)
		{
			
			socket.BeginSend(sendBytes,0, sendBytes.Length,0,SendCallback,socket);
		}
		
    }
	public static void SendCallback(IAsyncResult ar)
	{
		Socket socket = (Socket)ar.AsyncState;
		if(socket == null || !socket.Connected)
		{
			return;
		}
		int count = socket.EndSend(ar);
		//获取写入队列第一条数据
		ByteArray ba;
		lock (writeQueue)
		{
			ba = writeQueue.First();
		}
		ba.readIdx += count;
		if(ba.length == 0)
		{
			lock (writeQueue)
			{
				writeQueue.Dequeue();
				ba = writeQueue.First();
			}
		}
		if (ba!= null)
		{
			socket.BeginSend(ba.bytes,ba.readIdx,ba.length,0,SendCallback,socket);
		}else if (isCloseing)
		{
			socket.Close();
		}
	}
	private static void PingUpdate()
	{
		if (!isUsePing)
		{
			return;
		}
		if (Time.time - lastPingTime > pingInterval)
		{
			MsgPing msgPing = new MsgPing();
			Send(msgPing);
			lastPingTime = Time.time;
		}
		if(Time.time - lastPongTime > pingInterval * 4)
		{
			Close();
		}
	}
	private static void OnMsgPong(MsgBase msgBase)
	{
		lastPongTime = Time.time;
	}
}