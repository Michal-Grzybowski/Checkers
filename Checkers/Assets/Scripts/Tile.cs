using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X { get; set; }
    public int Y { get; set; }
    private Play play;
    void Start()
    {
        play = GameObject.Find("Canvas").GetComponent<Play>();
    }

    public void Select()
    {
        play.Move(this);
    }
}
