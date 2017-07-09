using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region<T> {

    public static int count = 0;

    private int id;

    public Region()
    {
        this.id = ++count;
    }
	
    public int getID()
    {
        return this.id;
    }

}
