using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeBuilder : ScriptableObject {

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
    Region region;

	public MazeBuilder()
    {

    }

    public void init(GameObject[,] board, GameObject[] tiles, int startX, int startY, Region region)
    {
        this.board = board;
        this.tiles = tiles;
        this.stack = new Stack<GameObject>();
        this.start = board[startX, startY];
        this.region = region;

        check(this.start, this.stack);
        // Debug.Log("Time after setting up maze, " + GameManager.watch.ElapsedMilliseconds);
    }
    

    /// <summary>
    /// Recursive function used by DFS to build the maze
    /// </summary>
    /// <param name="current">Current tile in DFS</param>
    /// <param name="stack">The stack, which is needed to step back through maze</param>
    void check(GameObject current, Stack<GameObject> stack)
    {
        // Check if maze can be continued in EACH direction
        foreach (BoardManager.Direction dir in getRandomDirections())
        {

            // Stores whether the first AND second tile in the specified direction is empty 
            GameObject aheadOne = BoardManager.move(this.board, current, dir, 1);
            GameObject aheadTwo = BoardManager.move(this.board, current, dir, 2);

            bool lookAheadOne = (BoardManager.isEmpty(aheadOne) && (checkSurrounding(aheadOne) <= 1));
            bool lookAheadTwo = (BoardManager.isEmpty(aheadTwo) && (checkSurrounding(aheadTwo) <= 0));

            // If both are empty, the maze can continue building
            if (lookAheadOne && lookAheadTwo){
                stack.Push(current);

                GameObject nextTile = BoardManager.move(this.board, current, dir, 1);
                GameObject replacementTile = BoardManager.replaceTile(nextTile, tiles[1], this.board, region);
                TileScript replacementTileScript = replacementTile.GetComponent<TileScript>();
                // replacementTileScript.setRegion(this.region);
                replacementTileScript.setType(TileScript.Type.MAZE);

                GameObject targetTile = BoardManager.move(this.board, current, dir, 2);
                GameObject replacementTarget = BoardManager.replaceTile(targetTile, tiles[1], this.board, region);
                TileScript replacementTargetScript = replacementTarget.GetComponent<TileScript>();
                // replacementTargetScript.setRegion(this.region);
                replacementTargetScript.setType(TileScript.Type.MAZE);

                check(targetTile, stack);
            }
        }

        // If dead end is reached, step back using the stack
        if (stack.Count != 0)
        {
            check(stack.Pop(), stack);
        }
    }

    /// <summary>
    /// Checks the area surronding a cell for non-empty cells. Returns the number of surronding, non-empty cells
    /// </summary>
    /// <param name="cell"></param>
    int checkSurrounding(GameObject cell)
    {
        int count = 0;

        foreach (BoardManager.Direction dir in getRandomDirections())
        {
           if (!(BoardManager.isEmpty(BoardManager.move(this.board, cell, dir, 1)))){
                count++;
            }
        }

        return count;
    }

    List<BoardManager.Direction> getRandomDirections()
    {
        
        List<BoardManager.Direction> directionList = Enum.GetValues(typeof(BoardManager.Direction)).Cast<BoardManager.Direction>().ToList();

        int n = directionList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            BoardManager.Direction value = directionList[k];
            directionList[k] = directionList[n];
            directionList[n] = value;
        }

        return directionList;
    }


}
