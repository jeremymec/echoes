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

    // Merge makes the SECOND region object obtain the ID of the first, and then removes the duplicate first region object from the list of regions
    public void mergeRegions(Region first, Region second)
    {
        second.setID(first);
        regions.Remove(first);
    }

    public Region addRegion()
    {
        Region r = new Region();
        this.regions.Add(r);
        return r;
    }


}
