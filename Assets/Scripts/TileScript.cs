using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

    public int[] arrayPos;
    Room room;
    Region<TileScript> region;

	// Use this for initialization
	void Start () {
        this.room = null;
	}

    public Room getRoom()
    {
        return this.room;
    }

    public void setRoom(Room room)
    {
        this.room = room;
    }

    void insert(int[] pos)
    {
        this.arrayPos = pos;
    }

    public Region<TileScript> getRegion()
    {
        return this.region;
    }

    public void setRegion(Region<TileScript> region)
    {
        this.region = region;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
