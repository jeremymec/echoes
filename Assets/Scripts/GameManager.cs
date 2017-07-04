using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public BoardManager boardScript;

	// Use this for initialization
	void Start () {
        boardScript = GetComponent<BoardManager>();
        boardScript.setupScene(10, 10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
