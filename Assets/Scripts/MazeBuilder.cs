using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : ScriptableObject {

    enum Direction { UP, DOWN, LEFT, RIGHT}

    GameObject[] tiles;
    GameObject[,] board;
    GameObject start;
    Stack<GameObject> stack;

	public MazeBuilder(GameObject[,] board, GameObject[] tiles, int startX, int startY)
    {
        this.board = board;
        this.tiles = tiles;
        this.stack = new Stack<GameObject>();
        this.start = board[startX, startY];

        check(this.start, this.stack);
    }

    void makePath(GameObject target)
    {
        int[] targetPos = target.GetComponent<TileScript>().arrayPos;
        Destroy(target);

    }

    void check(GameObject current, Stack<GameObject> stack)
    {
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            if (move(current, dir, 1) && (move(current, dir, 2))){
                makePath(current);
                stack.Push(current);
                check(move(current, dir, 1), stack);
            }
        }
        if (stack.Peek() != null)
        {
            check(stack.Pop(), stack);
        }
    }

    bool isEmpty(GameObject cell)
    {
        if (cell == null)
        {
            return false;
        }
        if (cell.CompareTag("emptyTile"))
        {
            return true;
        }
        return false;
    }

    GameObject move(GameObject cell, Direction dir, int amount)
    {
        GameObject target = null;
        int deltaX = 0;
        int deltaY = 0;

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

        try
        {
            int[] pos = cell.GetComponent<TileScript>().arrayPos;
            target = board[pos[0] + deltaX, (pos[1] + deltaY)];
        }
        catch (IndexOutOfRangeException e)
        {
            return null;
        }

        return target;
    }

}
