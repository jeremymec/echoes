using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameManager is instantiated at game startup, and controls the overall flow of the game.
/// </summary>
public class GameManager : MonoBehaviour {

    public BoardManager boardScript;
    // public static System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

    // Use this for initialization
    void Start () {
        // Debug.Log("Time at init of GameManager, " + GameManager.watch.ElapsedMilliseconds);

        // Gets the Gameobject BoardManager, and calls the start method on the BoardScript associated with that Object.
        boardScript = GetComponent<BoardManager>();
        boardScript.setupScene(20, 20);
    }
	
	// Update is called once per frame
	void Update () {

	}
}
