using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : ScriptableObject {

    enum Direction { UP, DOWN, LEFT, RIGHT}

    // Array of tiles types, passed in by BoardManager as opposed to dragged in using unity
    GameObject[] tiles;

    // Board array passed in by BoardManager
    GameObject[,] board;

    // Tile to begin building the maze on
    GameObject start;

    // Empty stack utilized by the DFS algorithm
    Stack<GameObject> stack;

	public MazeBuilder()
    {

    }

    public void init(GameObject[,] board, GameObject[] tiles, int startX, int startY)
    {
        this.board = board;
        this.tiles = tiles;
        this.stack = new Stack<GameObject>();
        this.start = board[startX, startY];
        makePath(this.start);

        check(this.start, this.stack);
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
        tile.GetComponent<TileScript>().arrayPos = targetPos;
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
        Debug.Log("Now checking Tile of type " + current.tag + "with position in array X: " + ts.arrayPos[0] + " Y: " + ts.arrayPos[1]);

        // Check if maze can be continued in EACH direction
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {

            // Stores whether the first AND second tile in the specified direction is empty 
            bool lookAheadOne = isEmpty(move(current, dir, 1));
            bool lookAheadTwo = isEmpty(move(current, dir, 2));

            // If both are empty, the maze can continue building
            if (lookAheadOne && lookAheadTwo){
                makePath(move(current, dir, 1));
                stack.Push(current);

                GameObject nextTile = move(current, dir, 1);
                check(nextTile, stack);
            }
        }

        // If dead end is reached, step back using the stack
        if (stack.Count != 0)
        {
            check(stack.Pop(), stack);
        }
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
