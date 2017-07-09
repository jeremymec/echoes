using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager {

    public List<Region> regions; // COULD BE IMPROVED

    public RegionManager()
    {
        regions = new List<Region>();
    }


    public Region addRegion()
    {
        Region r = new Region();
        this.regions.Add(r);
        return r;
    }


}
