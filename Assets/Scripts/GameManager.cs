using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameManager is instantiated at game startup, and controls the overall flow of the game.
/// </summary>
public class GameManager : MonoBehaviour {

    public BoardManager boardScript;

	// Use this for initialization
	void Start () {
        // Gets the Gameobject BoardManager, and calls the start method on the BoardScript associated with that Object.
        boardScript = GetComponent<BoardManager>();
        boardScript.setupScene(30, 30);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
