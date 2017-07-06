using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : ScriptableObject {

    public GameObject[,] board;

    int sizeX;
    int sizeY;

    int[] bottomLeft;

    public Room()
    {

    }


}
