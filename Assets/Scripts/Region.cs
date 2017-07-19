using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region {


    public int id; // IS PUBLIC FOR DEBUGGING

    public List<GameObject> tiles;

    public Region()
    {
        this.tiles = new List<GameObject>();
    }
	
    public int getID()
    {
        return this.id;
    }

    public void setID(int id)
    {
        this.id = id;
    }
    
    public List<GameObject> getTiles()
    {
        return this.tiles;
    }


}
