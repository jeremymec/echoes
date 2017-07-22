using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    // Enums for Direction
    public enum Direction { UP, DOWN, LEFT, RIGHT, NULL }

    // Enums for type of connection between rooms
    public enum Connector { DOOR, MAZE }

    // STATIC PARAMETERS
    public int boardWidth = 1000;
    public int boardHeight = 1000;
    public int roomFrequency = 1;
    public int roomPadding = 2;

    // Array of different types of Tile PREFABS that can be used to create new tiles
    public GameObject[] tiles;
    
    // Needed to programically create portals in different directions
    public Sprite[] portalSprites;

    // 2D Array of Tile GameObjects that make up the current game board
    public GameObject[,] board;

    // List of Room Objects on the board
    public List<Room> rooms;

    // Region Manager
    public RegionManager regionManager;

    public int debug1;
    public int debug2;

	public void setupScene () {

        createBoard();

        rooms = new List<Room>();
        placeRooms(roomFrequency);

        setupMaze();

        findConnectors();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = (int)pz.x;
            int y = (int)pz.y;

            // findConnectors();
            Debug.Log(x);
            Debug.Log(y);
            floodFillRegion(board[x, y], regionManager.addRegion());
        }
    }

    /// <summary>
    /// Initializes and fills 2D array of GameObjects with newly Instantiated empty tiles (which are also displayed on the game screen)
    /// </summary>
    /// <param name="width">Width of Board</param>
    /// <param name="height">Height of Board</param>
    void createBoard()
    {
        // Initializes Region manager to be used later in board creation process
        this.regionManager = new RegionManager();

        // Initializes array with GameObjects, that will be filled with Tiles
        board = new GameObject[boardWidth + 1, boardHeight + 1];

        for (int y = 0; y < boardHeight + 1; y++)
        {

            for (int x = 0; x < boardWidth + 1; x++)
            {
                // Instantiates tile of the specified type (empty tile) with coords determined by the for loop, and 0 rotiation.  
                GameObject tile = Instantiate(tiles[0], new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                tile.GetComponent<TileScript>().setType(TileScript.Type.EMPTY);

                // Places tile into GameObject array
                this.board[x, y] = tile;

                // Calls the 'insert' function of the TileScript, which stores it's position in the array
                tile.SendMessage("insert", new int[2] { x, y });
            }

        }

    }

    void placeRooms(int frequency)
    {
        for (int i = 0; i < frequency; i++)
        {
            int sizeX = UnityEngine.Random.Range(4, 9); // ROOM PARAMS CHANGE TO BE GLOBAL LATER PLS
            int sizeY = UnityEngine.Random.Range(4, 9); // ROOM PARAMS CHANGE TO BE GLOBAL LATER PLS
            sizeX = processNumber(false, sizeX);
            sizeY = processNumber(false, sizeY);

            int startX = UnityEngine.Random.Range(0, boardWidth - (sizeX + 1));
            int startY = UnityEngine.Random.Range(0, boardHeight - (sizeY + 1));
            startX = processNumber(false, startX);
            startY = processNumber(false, startY);

            int[] bottomLeft = { startX, startY };

            createRoom(bottomLeft, --sizeX, --sizeY);
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
        
        // Checks the area of the room, PLUS the padding
        for (int y = bottomLeft[1] - (roomPadding); y < sizeY + bottomLeft[1] + (roomPadding); y++)
        {
            for (int x = bottomLeft[0] - (roomPadding); x < sizeX + bottomLeft[0] + (roomPadding); x++)
            {
                // Debug.Log("About to access square at X: " + x + " Y: " + y);
                try
                {

                    TileScript ts = this.board[x, y].GetComponent<TileScript>();
                    if (ts.getRoom() != null)
                    {
                        return false;
                    }
                } catch (System.IndexOutOfRangeException)
                {
                    return false;
                }

            }
        }

        Room room = new Room();
        room.init(this.board, bottomLeft, sizeX, sizeY);

        // Instructs the region manager to create a new region with the room type, and assigns it to the newly created room
        Region roomRegion = regionManager.addRegion();
        room.setRegion(roomRegion);

        // Adds the newly created room to list of rooms
        rooms.Add(room);

        for (int y = bottomLeft[1] - roomPadding; y < sizeY + bottomLeft[1] + roomPadding + 1; y++)
        {
            for (int x = bottomLeft[0] - roomPadding; x < sizeX + bottomLeft[0] + roomPadding + 1; x++)
            {
                GameObject oldTile = this.board[x, y];
                
                int[] targetPos = oldTile.GetComponent<TileScript>().arrayPos;

                GameObject tile;

                // If the current tile is in the padding, create a emptyFloorTile instead of a normal tile
                if ((y < bottomLeft[1] || y > sizeY + bottomLeft[1]) || (x < bottomLeft[0] || x > sizeX + bottomLeft[0]))
                {
                    tile = replaceTile(oldTile, tiles[5], this.board);

                } else
                {
                    tile = replaceTile(oldTile, tiles[1], this.board);
                }

                TileScript ts = tile.GetComponent<TileScript>();

                ts.clone(oldTile.GetComponent<TileScript>());

                ts.setRoom(room);
                ts.setRegion(roomRegion);

                roomRegion.tiles.Add(tile);

                ts.setType(TileScript.Type.ROOM);

                this.board[targetPos[0], targetPos[1]] = tile;

                GameObject.Destroy(oldTile);
            }
        }
           
        // Since a room was created, return true
        return true;
    }

    void setupMaze()
    {
        for (int y = 0; y < boardHeight; y++)
        {

            for (int x = 0; x < boardWidth; x++)
            {
                if (checkEmptyBlock(board[x, y]))
                {
                    // Debug.Log("Creating Instance of Maze Builder at X: " + x + " Y: " + y);

                    MazeBuilder mazeBuilder = ScriptableObject.CreateInstance<MazeBuilder>();
                    Region r = regionManager.addRegion();
                    mazeBuilder.init(this.board, this.tiles, x, y, r);
                }
            }

        }
    }


    void findConnectors()
    {
        // Iterate through the board to identity possible connectors
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                // Check if current tile is a connector
                GameObject currentTile = board[x, y];

                // Only empty tiles can be connectors
                if (isEmpty(currentTile))
                {
                    // Gets all adjacent tiles (ignoring diagonals)
                    GameObject lookAheadUp = BoardManager.move(board, currentTile, Direction.UP, 1);
                    GameObject lookAheadDown = BoardManager.move(board, currentTile, Direction.DOWN, 1);
                    GameObject lookAheadLeft = BoardManager.move(board, currentTile, Direction.LEFT, 1);
                    GameObject lookAheadRight = BoardManager.move(board, currentTile, Direction.RIGHT, 1);

                    // Checks the following conditions for BOTH the up and down tile - NOT null, NOT empty, and NOT already a portal. If these are met, connect
                    if (isFloor(lookAheadUp) && isFloor(lookAheadDown))
                    {
                        checkConnectorRegion(lookAheadUp, currentTile, lookAheadDown);

                    } else if (isFloor(lookAheadLeft) && isFloor(lookAheadRight))
                    {
                        checkConnectorRegion(lookAheadLeft, currentTile, lookAheadRight);
                    }

                }
            }
        }
    }

    void checkConnectorRegion(GameObject firstTile, GameObject middleTile, GameObject lastTile)
    {
        if (!(RegionManager.compareRegion(firstTile, lastTile)))
        {
            createConnector(firstTile, middleTile, lastTile);
        }
    }

    void createConnector(GameObject firstTile, GameObject middleTile, GameObject lastTile)
    {
        GameObject roomTile = firstTile;
        GameObject outsideTile = lastTile;

        // Different types of connectors
        if (firstTile.CompareTag("roomPadding") ^ lastTile.CompareTag("roomPadding"))
        {
            // Sets the insideTile to the tile which has room padding
            if (firstTile.CompareTag("roomPadding"))
            {
                roomTile = firstTile;
                outsideTile = lastTile;

            }
            else
            {
                roomTile = lastTile;
                outsideTile = firstTile;
            }

            Direction direction = getDirection(outsideTile, roomTile);

            // If connecting a corridor to a room, ensure not just padding
            GameObject lookInside = move(this.board, roomTile, direction, (roomPadding));

            // If the probe finds a room, connector can be added
            if (lookInside.GetComponent<TileScript>().CompareTag("floorTile"))
            {
                Region finalRegion = roomTile.GetComponent<TileScript>().getRegion();

                addConnector(outsideTile, middleTile, lookInside, Connector.DOOR, direction);

                Debug.Log("Flood Fill Region called at X: " + middleTile.GetComponent<TileScript>().arrayPos[0] + " Y: " + middleTile.GetComponent<TileScript>().arrayPos[1]);
                floodFillRegion(middleTile, finalRegion);
            }

        }
        // Connection between two rooms
        else if (firstTile.CompareTag("floorTile") && lastTile.CompareTag("roomPadding"))
        {
            Direction direction = getDirection(outsideTile, roomTile);
            GameObject lookInside = move(this.board, roomTile, direction, (roomPadding));
        }

    }

    // Gets direction FROM first TO second. I.e. From tile [2, 2] to [1, 2] returns LEFT
    static Direction getDirection(GameObject first, GameObject second)
    {
        int[] firstPos = first.GetComponent<TileScript>().arrayPos;
        int[] secondPos = second.GetComponent<TileScript>().arrayPos;

        int delta = secondPos[0] - firstPos[0];

        if (delta > 0)
        {
            return Direction.RIGHT;

        } else if (delta < 0)
        {
            return Direction.LEFT;

        } else
        {
            delta = secondPos[1] - firstPos[1];
            if (delta > 0)
            {
                return Direction.UP;

            } else if (delta < 0)
            {
                return Direction.DOWN;

            }
        }

        return Direction.NULL;
    }


    // Returns if a given tile has a portal script attached to it, and is thus a portal
    bool isPortal(GameObject tile)
    {
        if (tile.GetComponent<PortalScript>() == null)
        {
            return false;
        }

        return true;
    }

    // NOTE: THIS WILL RETURN FALSE IF TILE A PORTAL
    bool isFloor(GameObject tile)
    {
        if (tile == null)
        {
            return false;
        }

        if (isPortal(tile))
        {
            return false;
        }

        if (tile.CompareTag("floorTile") || tile.CompareTag("roomPadding"))
        {
            return true;
        }

        return false;
    }

    void addConnector(GameObject firstTile, GameObject middleTile, GameObject lastTile, Connector type, Direction dir)
    {
        // Create the first tile (floor tile)
        GameObject firstTileReplacement = BoardManager.replaceTile(firstTile, tiles[4], this.board);

        // Replace middle tile
        GameObject replacementConnectingTile = BoardManager.replaceTile(middleTile, tiles[4], this.board);

        // Replace last tile with portal tile
        GameObject lastTileReplacement = BoardManager.replaceTile(lastTile, tiles[3], this.board);

        // If door to a room, need to go through room padding
        if (type == Connector.DOOR)
        {
            // Create list to store the connecting tiles between the portals
            List<GameObject> connectingTileReplacements = new List<GameObject>();

            // Create remaining connecting tiles
            for (int i = 1; i < roomPadding; i++)
            {
                GameObject oldConnectingTile = BoardManager.move(this.board, middleTile, dir, i);
                connectingTileReplacements.Add(BoardManager.replaceTile(oldConnectingTile, tiles[4], this.board));
            }
        }

        if (dir == Direction.UP)
        {
            lastTileReplacement.GetComponent<SpriteRenderer>().sprite = portalSprites[0];
            GameObject previous = move(this.board, lastTileReplacement, Direction.DOWN, 1);
            previous.GetComponent<SpriteRenderer>().sprite = portalSprites[8];

            // DEBUG CODE REMOVE LATER
            firstTileReplacement.GetComponent<TileScript>().debug = "UP DIR";
            lastTileReplacement.GetComponent<TileScript>().debug = "UP DIR";

        }
        else if (dir == Direction.DOWN)
        {
            lastTileReplacement.GetComponent<SpriteRenderer>().sprite = portalSprites[3];
            GameObject previous = move(this.board, lastTileReplacement, Direction.UP, 1);
            previous.GetComponent<SpriteRenderer>().sprite = portalSprites[4];

            // DEBUG CODE REMOVE LATER
            firstTileReplacement.GetComponent<TileScript>().debug = "DOWN DIR";
            lastTileReplacement.GetComponent<TileScript>().debug = "DOWN DIR";

        }
        else if (dir == Direction.LEFT)
        {
            lastTileReplacement.GetComponent<SpriteRenderer>().sprite = portalSprites[2];
            GameObject previous = move(this.board, lastTileReplacement, Direction.RIGHT, 1);
            previous.GetComponent<SpriteRenderer>().sprite = portalSprites[5];

            // DEBUG CODE REMOVE LATER
            firstTileReplacement.GetComponent<TileScript>().debug = "LEFT DIR";
            lastTileReplacement.GetComponent<TileScript>().debug = "LEFT DIR";

        }
        else if (dir == Direction.RIGHT)
        {
            lastTileReplacement.GetComponent<SpriteRenderer>().sprite = portalSprites[1];
            GameObject previous = move(this.board, lastTileReplacement, Direction.LEFT, 1);
            previous.GetComponent<SpriteRenderer>().sprite = portalSprites[7];

            // DEBUG CODE REMOVE LATER
            firstTileReplacement.GetComponent<TileScript>().debug = "RIGHT DIR";
            lastTileReplacement.GetComponent<TileScript>().debug = "RIGHT DIR";

        }
    }

    public static bool compareRoom(GameObject firstTile, GameObject secondTile)
    {
        // return true;

        TileScript firstTs = firstTile.GetComponent<TileScript>();
        TileScript secondTs = secondTile.GetComponent<TileScript>();

        Room firstRoom = firstTs.getRoom();
        Room secondRoom = secondTs.getRoom();

        if (firstRoom == null && secondRoom == null)
        {
            return true;
        }

        if (firstRoom == null ^ secondRoom == null)
        {
            return false;
        }

        return (firstRoom.Equals(secondRoom));
    }

    public void floodFillRegion(GameObject startTile, Region region)
    {
        TileScript ts = startTile.GetComponent<TileScript>();
        ts.setRegion(region);
        // startTile.GetComponent<SpriteRenderer>().sprite = portalSprites[10];

        GameObject lookAheadUp = BoardManager.move(board, startTile, Direction.UP, 1);
        GameObject lookAheadDown = BoardManager.move(board, startTile, Direction.DOWN, 1);
        GameObject lookAheadLeft = BoardManager.move(board, startTile, Direction.LEFT, 1);
        GameObject lookAheadRight = BoardManager.move(board, startTile, Direction.RIGHT, 1);

        if ((lookAheadUp != null))
        {
            if (!(isEmpty(lookAheadUp)) && !(RegionManager.compareRegion(startTile, lookAheadUp)) && compareRoom(startTile, lookAheadUp))
            {
                floodFillRegion(lookAheadUp, region);
            }
        }
        if ((lookAheadDown != null))
        {
            if (!(isEmpty(lookAheadDown)) && !(RegionManager.compareRegion(startTile, lookAheadDown)) && compareRoom(startTile, lookAheadDown))
            {
                floodFillRegion(lookAheadDown, region);
            }
        }
        if ((lookAheadLeft != null))
        {
            if (!(isEmpty(lookAheadLeft)) && !(RegionManager.compareRegion(startTile, lookAheadLeft)) && compareRoom(startTile, lookAheadLeft))
            {
                floodFillRegion(lookAheadLeft, region);
            }
        }
        if ((lookAheadRight != null))
        {
            if (!(isEmpty(lookAheadRight)) && !(RegionManager.compareRegion(startTile, lookAheadRight)) && compareRoom(startTile, lookAheadRight))
            {
                floodFillRegion(lookAheadRight, region);
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
        catch (IndexOutOfRangeException)
        {
            return null;
        }

        return target;
    }

    public static GameObject replaceTile(GameObject original, GameObject prefab, GameObject[,] board)
    {
        TileScript tsOld = original.GetComponent<TileScript>();
        int[] targetPos = tsOld.arrayPos;

        GameObject tile;

        tile = Instantiate(prefab, new Vector3(targetPos[0], targetPos[1], 0), Quaternion.identity) as GameObject;

        TileScript ts = tile.GetComponent<TileScript>();
        ts.clone(tsOld);
        ts.setType(TileScript.Type.PASSAGE);

        board[targetPos[0], targetPos[1]] = tile;

        Region region = tsOld.getRegion();
        region.tiles.Remove(original);
        region.tiles.Add(tile);

        GameObject.Destroy(original);

        return tile;
    }

    public static GameObject replaceTile(GameObject original, GameObject prefab, GameObject[,] board, Region region)
    {
        TileScript tsOld = original.GetComponent<TileScript>();
        int[] targetPos = tsOld.arrayPos;

        GameObject tile;

        tile = Instantiate(prefab, new Vector3(targetPos[0], targetPos[1], 0), Quaternion.identity) as GameObject;

        TileScript ts = tile.GetComponent<TileScript>();
        ts.clone(tsOld);
        ts.setType(TileScript.Type.PASSAGE);
        ts.setRegion(region);

        region.tiles.Remove(original);
        region.tiles.Add(tile);

        board[targetPos[0], targetPos[1]] = tile;

        GameObject.Destroy(original);

        return tile;
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

        // Checks by tag comparison (THERE MIGHT BE A BETTER WAY TO DO THIS)
        if (cell.CompareTag("emptyTile"))
        {
            return true;
        }

        return false;
    }


}
