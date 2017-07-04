using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject[] tiles;
    public GameObject[,] board;

	public void setupScene (int width, int height) {
        createBoard(width, height);
	}

    void createBoard(int width, int height)
    {
        board = new GameObject[width, height];

        for (int y = 0; y < height; y++)
        {

            for (int x = 0; x < height; x++)
            {
                GameObject tile = Instantiate(tiles[1], new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                this.board[x, y] = tile;
                tile.SendMessage("insert", new int[2] { x, y });
                // Debug.Log("Created game object at X: " + x + " and Y: " + y);
            }

        }

        setupMaze();

    }

    void setupMaze()
    {
        MazeBuilder mazeBuilder = new MazeBuilder(this.board, 5, 5);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
