using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager {

    public ArrayList regions; // COULD BE IMPROVED

    public RegionManager()
    {
        regions = new ArrayList();
    }

    public Region<T> addRegion<T>()
    {
        Region<T> r = new Region<T>();
        regions.Add(r);
        return r;
    }


}
