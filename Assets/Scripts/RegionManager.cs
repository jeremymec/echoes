using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager {

    public List<Region> regions;

    public RegionManager()
    {
        regions = new List<Region>();
    }

    public void mergeRegions(Region first, Region second)
    {
        second.setID(first);        
    }

    public Region addRegion()
    {
        Region r = new Region();
        this.regions.Add(r);
        return r;
    }


}
