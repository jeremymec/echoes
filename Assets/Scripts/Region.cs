using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region {


    public int id; // IS PUBLIC FOR DEBUGGING

    public Region()
    {
    }
	
    public int getID()
    {
        return this.id;
    }

    public void setID(int id)
    {
        this.id = id;
    }
    
    public static bool compareRegion(GameObject firstTile, GameObject secondTile)
    {
        Region r1 = firstTile.GetComponent<TileScript>().getRegion();
        Region r2 = secondTile.GetComponent<TileScript>().getRegion();

        if (r1.getID() == r2.getID())
        {
            return true;

        } else
        {
            return false;
        }
    }

}
