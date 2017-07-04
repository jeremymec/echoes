using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder {

    enum Direction { UP, DOWN, LEFT, RIGHT}

    GameObject[,] board;
    GameObject start;
    Stack<GameObject> stack;

	public MazeBuilder(GameObject[,] board, int startX, int startY)
    {
        this.board = board;
        this.stack = new Stack<GameObject>();
        this.start = board[startX, startY];

        GameObject target = move(this.start, Direction.UP);

        Debug.Log("Initial Pos, X: " + this.start.GetComponent<TileScript>().arrayPos[0] + " Y: " + this.start.GetComponent<TileScript>().arrayPos[1]);
        Debug.Log("Target Pos, X: " + target.GetComponent<TileScript>().arrayPos[0] + " Y: " + target.GetComponent<TileScript>().arrayPos[1]);
    }

    void check(GameObject current, Stack<GameObject> stack)
    {
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {

        }
    }

    GameObject move(GameObject cell, Direction dir)
    {
        GameObject target = null;
        switch (dir)
        {
            case Direction.UP:
                
                try
                {
                    int[] pos = cell.GetComponent<TileScript>().arrayPos;
                    target = board[pos[0], (pos[1] + 1)];
                } catch (IndexOutOfRangeException e)
                {
                    return null;
                }
                break;
           
        }

        return target;
    }

}
