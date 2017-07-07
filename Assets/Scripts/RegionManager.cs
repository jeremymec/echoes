using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager {

    ArrayList regions; // COULD BE IMPROVED

    public RegionManager()
    {

    }

    public void addRegion<T>(T region)
    {
        regions.Add(new Region<T>());
    }


}
