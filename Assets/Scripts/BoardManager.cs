using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    // Array of different types of Tile PREFABS that can be used to create new tiles
    public GameObject[] tiles;

    // 2D Array of Tile GameObjects that make up the current game board
    public GameObject[,] board;

	public void setupScene (int width, int height) {
        Debug.Log("Time at beginning of setupScene, " + GameManager.watch.ElapsedMilliseconds);
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

        placeRooms(10);

        // Debug.Log("Time before setting up maze, " + GameManager.watch.ElapsedMilliseconds);
        // setupMaze();

    }

    void placeRooms(int frequency)
    {
        for (int i = 0; i < frequency; i++)
        {
            int sizeX = Random.Range(3, 8);
            int sizeY = Random.Range(3, 8);

            int debug0 = board.GetLength(0); // DEBUG
            int debug1 = board.GetLength(1); // DEBUG

            int startX = Random.Range(0, board.GetLength(0) - (sizeX + 1));
            int startY = Random.Range(0, board.GetLength(1) - (sizeX + 1));

            int[] bottomLeft = { startX, startY };

            createRoom(bottomLeft, sizeX, sizeY);
        }
    }

    bool createRoom(int[] bottomLeft, int sizeX, int sizeY)
    {

        for (int y = bottomLeft[1]; y < sizeY + (bottomLeft[1] - 1); y++)
        {
            for (int x = bottomLeft[0]; y < sizeX + (bottomLeft[0] - 1); x++)
            {
                Debug.Log("About to access square at X: " + x + " Y: " + y);
                if (this.board[x, y].GetComponent<TileScript>().getRoom() != null)
                {
                    return false;
                }
            }
        }

        Room room = ScriptableObject.CreateInstance<Room>();
        room.init(this.board, bottomLeft, sizeX, sizeY);

        for (int y = bottomLeft[1]; y < sizeY; y++)
        {
            for (int x = bottomLeft[0]; y < sizeX; x++)
            {
                GameObject oldTile = this.board[x, y];
                int[] targetPos = oldTile.GetComponent<TileScript>().arrayPos;

                GameObject tile = Instantiate(tiles[1], new Vector3(targetPos[1], targetPos[1], 0), Quaternion.identity) as GameObject;
                tile.GetComponent<TileScript>().setRoom(room);
                this.board[targetPos[0], targetPos[1]] = tile;

                GameObject.Destroy(oldTile);
            }
        }

        return true;
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
