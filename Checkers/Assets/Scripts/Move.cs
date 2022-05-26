using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public List<(int, int)> MoveList { get; set; }
    public List<PieceClass> CapturedPiecesList { get; set; }
}
