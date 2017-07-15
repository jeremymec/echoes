using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region {

    public static int count = 0;

    public int id; // IS PUBLIC FOR DEBUGGING

    public Region()
    {
        this.id = ++count;
    }
	
    public int getID()
    {
        return this.id;
    }

    public void setID(Region region)
    {
        this.id = region.id;
    }
}
