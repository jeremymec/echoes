using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager {

    public List<Region> regions;

    int region_count = 0;

    public RegionManager()
    {
        regions = new List<Region>();
    }

    public Region addRegion()
    {
        Region r = new Region();

        r.setID(++region_count);

        this.regions.Add(r);
        return r;
    }

    public static bool compareRegion(GameObject firstTile, GameObject secondTile)
    {
        Region r1 = firstTile.GetComponent<TileScript>().getRegion();
        Region r2 = secondTile.GetComponent<TileScript>().getRegion();

        if (r1.getID() == r2.getID())
        {
            return true;

        }
        else
        {
            return false;
        }
    }


}
