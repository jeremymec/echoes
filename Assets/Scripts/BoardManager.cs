using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    // Array of different types of Tile PREFABS that can be used to create new tiles
    public GameObject[] tiles;

    // 2D Array of Tile GameObjects that make up the current game board
    public GameObject[,] board;

    // List of Room Objects on the board
    public List<Room> rooms;

    // Region Manager
    public RegionManager regionManager;

    // Enums for Direction
    public enum Direction { UP, DOWN, LEFT, RIGHT}

	public void setupScene (int width, int height) {
        // Debug.Log("Time at beginning of setupScene, " + GameManager.watch.ElapsedMilliseconds);
        createBoard(width, height);
	}

    /// <summary>
    /// Initializes and fills 2D array of GameObjects with newly Instantiated empty tiles (which are also displayed on the game screen)
    /// </summary>
    /// <param name="width">Width of Board</param>
    /// <param name="height">Height of Board</param>
    void createBoard(int width, int height)
    {
        // Initializes Region manager to be used later in board creation process
        this.regionManager = new RegionManager();

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

        placeRooms(500);

        // Debug.Log("Time before setting up maze, " + GameManager.watch.ElapsedMilliseconds);
        setupMaze();

    }

    void placeRooms(int frequency)
    {
        for (int i = 0; i < frequency; i++)
        {
            int sizeX = UnityEngine.Random.Range(4, 9);
            int sizeY = UnityEngine.Random.Range(4, 9);
            sizeX = processNumber(false, sizeX);
            sizeY = processNumber(false, sizeY);

            int startX = UnityEngine.Random.Range(0, board.GetLength(0) - (sizeX + 2));
            int startY = UnityEngine.Random.Range(0, board.GetLength(1) - (sizeY + 2));
            startX = processNumber(false, startX);
            startY = processNumber(false, startY);

            int[] bottomLeft = { startX, startY };

            // Debug.Log("MAKING ROOM, ATTEMPT #" + i + "WITH BOTTOMLEFT X: " + startX + " Y: " + startY);
            createRoom(bottomLeft, sizeX, sizeY);
        }
    }

    // if not even, then odd
    int processNumber(bool odd, int num)
    {   
        if (odd)
        {
            if (num % 2 == 0)
            {
                return num;

            }
            else
            {
                return num + 1;
            }
        } else
        {
            if (num % 2 == 0)
            {
                return num + 1;
            }
            else
            {
                return num;
            }
        }

    }

    bool createRoom(int[] bottomLeft, int sizeX, int sizeY)
    {

        for (int y = bottomLeft[1] - 1; y < sizeY + bottomLeft[1] + 1; y++)
        {
            for (int x = bottomLeft[0] - 1; x < sizeX + bottomLeft[0] + 1; x++)
            {
                // Debug.Log("About to access square at X: " + x + " Y: " + y);
                try
                {
                    if (this.board[x, y].GetComponent<TileScript>().getRoom() != null)
                    {
                        return false;
                    }
                } catch (System.IndexOutOfRangeException e)
                {
                    return false;
                }

            }
        }

        Room room = ScriptableObject.CreateInstance<Room>();
        room.init(this.board, bottomLeft, sizeX, sizeY);

        for (int y = bottomLeft[1]; y < sizeY + bottomLeft[1]; y++)
        {
            for (int x = bottomLeft[0]; x < sizeX + bottomLeft[0]; x++)
            {
                GameObject oldTile = this.board[x, y];
                int[] targetPos = oldTile.GetComponent<TileScript>().arrayPos;

                GameObject tile = null;

                //if ((y == bottomLeft[1] || y == sizeY + bottomLeft[1]) || (x == bottomLeft[0] || x == sizeX + bottomLeft[0]))
                //{
                //    tile = Instantiate(tiles[0], new Vector3(targetPos[0], targetPos[1], 0), Quaternion.identity) as GameObject;
                // } else
                // {
                    tile = Instantiate(tiles[1], new Vector3(targetPos[0], targetPos[1], 0), Quaternion.identity) as GameObject;
                // }

                // Debug.Log("Making new Tile at X: " + targetPos[0] + " Y: " + targetPos[1]);
                tile.GetComponent<TileScript>().setRoom(room);
                this.board[targetPos[0], targetPos[1]] = tile;

                GameObject.Destroy(oldTile);
            }
        }

        // Instructs the region manager to create a new region with the room type, and assigns it to the newly created room
        room.setRegion(regionManager.addRegion<Room>());
            
        // Adds the newly created room to list of rooms
        rooms.Add(room);

        // Since a room was created, return true
        return true;
    }

    void setupMaze()
    {
        for (int y = 0; y < board.GetLength(1); y++)
        {

            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (checkEmptyBlock(board[x, y]))
                {
                    Debug.Log("Creating Instance of Maze Builder at X: " + x + " Y: " + y);

                    MazeBuilder mazeBuilder = ScriptableObject.CreateInstance<MazeBuilder>();
                    Region<TileScript> r = regionManager.addRegion<TileScript>();
                    mazeBuilder.init(this.board, this.tiles, x, y, r);
                }
            }

        }
    }

    /// <summary>
    /// Helper method for setupMaze() which returns if a cell is an empty block, and thus safe for MazeBuilder to be run on 
    /// </summary>
    bool checkEmptyBlock(GameObject cell)
    {
        if (cell.GetComponent<TileScript>().getRoom() != null)
        {
            return false;
        }

        if (!(isEmpty(cell)))
        {
            return false;
        }

        int nonEmptyCount = 0;

        foreach (Direction dir in Enum.GetValues(typeof(BoardManager.Direction))){
            if (!(isEmpty(move(this.board, cell, dir, 1)))){
                return false;
            }

            if (!(isEmpty(move(this.board, cell, dir, 2))))
            {
                nonEmptyCount++;
            }
        }

        if (nonEmptyCount >= 4)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the GameObject ahead by a given number of tiles, in a given direction.
    /// </summary>
    /// <param name="cell">The cell to move FROM</param>
    /// <param name="dir">The direction (using Enum Types)</param>
    /// <param name="amount">Magnitude of the move</param>
    public static GameObject move(GameObject[,] board, GameObject cell, Direction dir, int amount)
    {
        GameObject target = null;
        int deltaX = 0;
        int deltaY = 0;

        // Switchcase to calculate deltax and deltay for given direction
        switch (dir)
        {
            case Direction.UP:
                deltaY += amount;
                break;

            case Direction.DOWN:
                deltaY -= amount;
                break;

            case Direction.RIGHT:
                deltaX += amount;
                break;

            case Direction.LEFT:
                deltaX -= amount;
                break;
        }

        // This exists to prevent throwing an error when the function checks outside the maze - instead, it will return null.
        try
        {
            // Gets the cell at the array position corresponding to the movement 
            int[] pos = (cell.GetComponent("TileScript") as TileScript).arrayPos;
            target = board[pos[0] + deltaX, (pos[1] + deltaY)];
        }
        catch (IndexOutOfRangeException e)
        {
            return null;
        }

        return target;
    }

        /// <summary>
    /// Checks if a given cell is an empty tile
    /// </summary>
    /// <returns>True if empty, false if any other type</returns>
    public static bool isEmpty(GameObject cell)
    {
        if (cell == null)
        {
            return false;
        }

        if (cell.GetComponent<TileScript>().getRoom() != null)
        {
            return false;
        }

        // Checks by tag comparison (THERE MIGHT BE A BETTER WAY TO DO THIS)
        if (cell.CompareTag("emptyTile"))
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
