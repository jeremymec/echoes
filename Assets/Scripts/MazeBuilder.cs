using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeBuilder : ScriptableObject {

    enum Direction { UP, DOWN, LEFT, RIGHT}

    private static System.Random rng = new System.Random();

    // Array of tiles types, passed in by BoardManager as opposed to dragged in using unity
    GameObject[] tiles;

    // Board array passed in by BoardManager
    GameObject[,] board;

    // Tile to begin building the maze on
    GameObject start;

    // Empty stack utilized by the DFS algorithm
    Stack<GameObject> stack;

    // Region for the maze
    Region<TileScript> region;

	public MazeBuilder()
    {

    }

    public void init(GameObject[,] board, GameObject[] tiles, int startX, int startY, Region<TileScript> region)
    {
        this.board = board;
        this.tiles = tiles;
        this.stack = new Stack<GameObject>();
        this.start = board[startX, startY];
        this.region = region;

        makePath(this.start);

        check(this.start, this.stack);
        Debug.Log("Time after setting up maze, " + GameManager.watch.ElapsedMilliseconds);
    }
    
    //TODO: REMOVE THIS, DEBUGGING ONLY
    public void test()
    {
        
    }

    /// <summary>
    /// Takes an empty tile and 'makes a path', i.e. turn it into a traversable floor tile
    /// </summary>
    void makePath(GameObject target)
    {

        // Gets coords of target to be replaced
        int[] targetPos = target.GetComponent<TileScript>().arrayPos;

        // Creates new floor tile and replaces the array reference to the empty tile with this new tile
        GameObject tile = Instantiate(tiles[1], new Vector3(targetPos[0], targetPos[1], 0), Quaternion.identity) as GameObject;

        TileScript ts = tile.GetComponent<TileScript>();
        ts.arrayPos = targetPos;
        ts.setRegion(this.region);

        this.board[targetPos[0], targetPos[1]] = tile;

        // Removes the target from the game board
        Destroy(target);
    }

    /// <summary>
    /// Recursive function used by DFS to build the maze
    /// </summary>
    /// <param name="current">Current tile in DFS</param>
    /// <param name="stack">The stack, which is needed to step back through maze</param>
    void check(GameObject current, Stack<GameObject> stack)
    {

        TileScript ts = current.GetComponent("TileScript") as TileScript;
        // Debug.Log("Now checking Tile of type " + current.tag + "with position in array X: " + ts.arrayPos[0] + " Y: " + ts.arrayPos[1]);

        // Check if maze can be continued in EACH direction
        foreach (Direction dir in getRandomDirections())
        {

            // Stores whether the first AND second tile in the specified direction is empty 
            bool lookAheadOne = isEmpty(move(current, dir, 1));
            bool lookAheadTwo = isEmpty(move(current, dir, 2));

            // If both are empty, the maze can continue building
            if (lookAheadOne && lookAheadTwo){
                stack.Push(current);

                GameObject nextTile = move(current, dir, 1);
                makePath(nextTile);

                GameObject targetTile = move(current, dir, 2);
                makePath(targetTile);
                check(targetTile, stack);
            }
        }

        // If dead end is reached, step back using the stack
        if (stack.Count != 0)
        {
            check(stack.Pop(), stack);
        }
    }

    List<Direction> getRandomDirections()
    {
        
        List<Direction> directionList = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();

        int n = directionList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Direction value = directionList[k];
            directionList[k] = directionList[n];
            directionList[n] = value;
        }

        return directionList;
    }

    /// <summary>
    /// Checks if a given cell is an empty tile
    /// </summary>
    /// <returns>True if empty, false if any other type</returns>
    bool isEmpty(GameObject cell)
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

    /// <summary>
    /// Returns the GameObject ahead by a given number of tiles, in a given direction.
    /// </summary>
    /// <param name="cell">The cell to move FROM</param>
    /// <param name="dir">The direction (using Enum Types)</param>
    /// <param name="amount">Magnitude of the move</param>
    GameObject move(GameObject cell, Direction dir, int amount)
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

}
