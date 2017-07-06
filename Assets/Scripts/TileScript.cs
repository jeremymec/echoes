using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

    public int[] arrayPos;
    bool visited;

	// Use this for initialization
	void Start () {
		
	}

    void insert(int[] pos)
    {
        this.arrayPos = pos;
    }

    public bool isVisited()
    {
        return this.visited;
    }

    public void vist()
    {
        this.visited = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
