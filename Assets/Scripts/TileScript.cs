using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

    public int[] arrayPos;
    Room room;
    Region region;

    // FOR DEBUGGING USE
    public static bool debug = true;

	// Use this for initialization
	void Start () {
        this.room = null;
	}

    public void clone(TileScript original)
    {
        this.arrayPos = original.arrayPos;
        this.room = original.room;
        this.region = original.region;
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

    public Region getRegion()
    {
        return this.region;
    }

    public void setRegion(Region region)
    {   
        if (debug && region == null)
        {
            throw new MissingReferenceException();
        }

        this.region = region;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
