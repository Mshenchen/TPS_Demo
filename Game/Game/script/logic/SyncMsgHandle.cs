using System;


public partial class MsgHandler {
	public static void MsgSyncPlayer(ClientState c,MsgBase msgBase)
	{
		MsgSyncPlayer msg = (MsgSyncPlayer)msgBase;
		Player player = c.player;
		if(player == null) { return; }
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null) { return; }
		if(room.status != Room.Status.FIGHT) { return; }
		//TODO:作弊检测
		//if(Math.Abs(player.x-msg.x)>5||)
		player.x = msg.x;
		player.y = msg.y;
		player.z = msg.z;
		player.ex = msg.ex;
		player.ey = msg.ey;
		player.ez = msg.ez;
		msg.id = player.id;
		room.Broadcast(msg);
	}
    public static void MsgSyncAnim(ClientState c, MsgBase msgBase)
    {
        MsgSyncAnim msg = (MsgSyncAnim)msgBase;
        Player player = c.player;
        if (player == null) { return; }
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) { return; }
        if (room.status != Room.Status.FIGHT) { return; }
        //TODO:作弊检测
        player.speedValue = msg.speedValue;
        player.jumpValue = msg.jumpValue;
        player.groundedValue = msg.groundedValue;
        player.freefallValue = msg.freefallValue;
        player.motionSpeedValue = msg.motionSpeedValue;
        msg.id = player.id;
        room.Broadcast(msg);
    }
    public static void MsgHit(ClientState c,MsgBase msgBase)
	{
		MsgHit msg = (MsgHit)msgBase;
		Player player = c.player;
		if(player == null) { return; }
		Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
		if(targetPlayer == null) { return; };
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null) { return; }
		if(room.status != Room.Status.FIGHT) { return; }
		if(player.id != msg.id) { return; }
		int damage = 35;
		targetPlayer.hp -= damage;
		msg.id = player.id;
		msg.hp = player.hp;
		msg.damage = damage;
		room.Broadcast(msg);
	}
}


