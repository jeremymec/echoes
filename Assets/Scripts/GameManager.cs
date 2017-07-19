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
        setupBoard();
    }

    void setupBoard()
    {
        boardScript = GetComponent<BoardManager>();
        boardScript.setupScene();
    }
	
	// Update is called once per frame
	void Update () {

	}
}
