using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceClass
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsWhite { get; set; }
    public bool IsQueen { get; set; }
    public List<Move> Moves { get; set; }
}
