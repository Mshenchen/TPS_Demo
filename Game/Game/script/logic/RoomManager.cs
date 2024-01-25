﻿using System;
using System.Collections.Generic;

public class RoomManager
{
    private static int maxId = 1;
    public static Dictionary<int,Room> rooms = new Dictionary<int,Room>();
    public static void Update()
    {
        foreach (Room room in rooms.Values)
        {
            room.Update();
        }
    }
    public static Room GetRoom(int id)
    {
        if (rooms.ContainsKey(id)) { return rooms[id]; }
        return null;
    }
    public static Room AddRoom()
    {
        maxId++;
        Room room = new Room();
        room.id = maxId;
        rooms.Add(room.id, room);
        return room;
    }
    public static bool RemoveRoom(int id)
    {
        rooms.Remove(id);
        return true;
    }
    public static MsgBase ToMsg()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = rooms.Count;
        msg.rooms = new RoomInfo[count];
        int i = 0;
        foreach (Room room in rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.id = room.id;
            roomInfo.count = room.playerIds.Count;
            roomInfo.status = (int)room.status;
            msg.rooms[i] = roomInfo;
            i++;
        }
        return msg;
    }
}