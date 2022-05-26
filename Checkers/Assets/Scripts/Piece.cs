using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceClass Get { get; set; }
    /*    public int X { get; set; }
        public int Y { get; set; }
        private Play play; 
        public bool IsWhite { get; set; }
        public bool IsQueen { get; set; }
        public List<Move> Moves { get; set; }*/
    private Play play;
    public void Start()
    {
        play = GameObject.Find("Canvas").GetComponent<Play>();
    }
    public void Select()
    {
        play.ShowMoves(this);
    }
}

