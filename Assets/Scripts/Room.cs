using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class Room {

    public GameObject[,] board;

    int sizeX;
    int sizeY;

    public static int count = 0;
    public int id;

    int[] bottomLeft;

    Region region;

    public Room()
    {
        this.id = ++count;
    }

    public void init(GameObject[,] board, int[] bottomLeft, int sizeX, int sizeY)
    {
        this.board = board;
        this.bottomLeft = bottomLeft;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public Region getRegion()
    {
        return this.region;
    }

    public void setRegion(Region region)
    {
        this.region = region;
    }

    public override bool Equals(object obj)
    {
        Room room = obj as Room;

        if (room == null)
        {
            return false;
        }
        
        if (this.id == room.id)
        {
            return true;
        }

        return false;
    }

}
