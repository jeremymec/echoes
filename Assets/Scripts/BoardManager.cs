using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    // Array of different types of Tile PREFABS that can be used to create new tiles
    public GameObject[] tiles;

    // 2D Array of Tile GameObjects that make up the current game board
    public GameObject[,] board;

	public void setupScene (int width, int height) {
        createBoard(width, height);
	}

    /// <summary>
    /// Initializes and fills 2D array of GameObjects with newly Instantiated empty tiles (which are also displayed on the game screen)
    /// </summary>
    /// <param name="width">Width of Board</param>
    /// <param name="height">Height of Board</param>
    void createBoard(int width, int height)
    {
        // Initializes array with GameObjects, that will be filled with Tiles
        board = new GameObject[width, height];

        for (int y = 0; y < height; y++)
        {

            for (int x = 0; x < height; x++)
            {
                // Instantiates tile of the specified type (empty tile) with coords determined by the for loop, and 0 rotiation.  
                GameObject tile = Instantiate(tiles[0], new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                // Places tile into GameObject array
                this.board[x, y] = tile;

                // Calls the 'insert' function of the TileScript, which stores it's position in the array
                tile.SendMessage("insert", new int[2] { x, y });
            }

        }

        setupMaze();

    }

    void setupMaze()
    {
        // Creates a new MazeBuilder object for the newly created board fill of empty tiles
        MazeBuilder mazeBuilder = ScriptableObject.CreateInstance<MazeBuilder>();
        mazeBuilder.init(this.board, this.tiles, 0, 0);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
