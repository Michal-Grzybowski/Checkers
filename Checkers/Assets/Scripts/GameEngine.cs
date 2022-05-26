using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine
{ 
    
    public static int BoardSize;
    public static GameObject[,] Board;
    public static List<PieceClass> PiecesList;
    public static int drawCounter;
    public static bool draw = false;
/*    public static PieceClass BestPiece;
    public static Move BestMove;*/
    public static Play play;
    public int BotDepth;
    public bool IsBotWhiteColor; 

    /*    void EngineInitialise()
        {
            play = GameObject.Find("Canvas").GetComponent<Play>();
            PiecesRowNumber = Play.PiecesRowNumber;
            BoardSize = Play.BoardSize;
            GameObject[,] Board = Play.Board;
            List<Piece> PiecesList = Play.PiecesList;
            drawCounter = Play.MovesCounterAfterFirstQueen;
        }*/

    public void EngineInitialise(int boardSize, List<PieceClass> piecesList, int movesCounterAfterFirstQueen, int botDepth, bool botColor)
    {
        play = GameObject.Find("Canvas").GetComponent<Play>();
        BoardSize = boardSize;
        PiecesList = piecesList;
        drawCounter = movesCounterAfterFirstQueen;
        BotDepth = botDepth;
        IsBotWhiteColor = botColor;
    }

    public void ActualiseData (List<PieceClass> piecesList, int movesCounterAfterFirstQueen)
    {
        PiecesList = piecesList;
        drawCounter = movesCounterAfterFirstQueen;
/*        BestPiece = null;
        BestMove = null;*/
    }
    public int checkGameCondition(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        //sprawdzanie wygranej
        bool hasMove = false;
        bool pieceExist = false;
        foreach (PieceClass p in PiecesList)
        {
            if (p.IsWhite == isWhiteTurn)
            {
                pieceExist = true;
                if (p.Moves.Count > 0)
                {
                    hasMove = true;
                }
            }
        }
        //drukowanie wygranej
        if (!(pieceExist && hasMove))
        {
            if (isWhiteTurn)
            {
                //Debug.Log("Wygraly czarne");
                return -1;
            }
            else
            {
                //Debug.Log("Wygraly biale");
                return 1;
            }
        }
        return 0;
    }

    public float RatePosition(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        float positionScore = checkGameCondition(isWhiteTurn, PiecesList);
        if (positionScore == 1) return float.MaxValue;
        if (positionScore == -1) return -1 * float.MaxValue;
        foreach(PieceClass piece in PiecesList)
        {
            float value = 1.0f;
            if (!piece.IsWhite) value *= -1;
            if (piece.IsQueen) value *= 2;
            if(piece.X == 0 || piece.Y == 0 || piece.X == BoardSize-1 || piece.Y == BoardSize - 1)
            {
                value *= 1.25f;
            }
            else if(piece.X == 1 || piece.Y == 1 || piece.X == BoardSize - 2 || piece.Y == BoardSize - 2)
            {
                value *= 1.15f;
            }
            positionScore += value;
        }
        if (drawCounter > 30)
        {
            //Debug.Log("Remis!");
            draw = true;
            return 0.0f;
        }
        else draw = false;
        return positionScore;
    }

    public float RatePosition2(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        float positionScore = checkGameCondition(isWhiteTurn, PiecesList);
        if (positionScore == 1) return float.MaxValue;
        if (positionScore == -1) return -1 * float.MaxValue;
        foreach (PieceClass piece in PiecesList)
        {
            float value = 3.0f;
            if (piece.IsQueen) value = 5;
            if (!piece.IsWhite) value *= -1;
            if (!piece.IsQueen && piece.IsWhite && piece.X <3)
            {
                value *= 1.5f;
            }
            else if (piece.IsQueen && !piece.IsWhite && piece.X > BoardSize - 4)
            {
                value *= 1.5f;
            }
            positionScore += value;
        }
        if (drawCounter > 30)
        {
            //Debug.Log("Remis!");
            draw = true;
            return 0.0f;
        }
        else draw = false;
        return positionScore;
    }

    public float RatePosition3(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        float positionScore = checkGameCondition(isWhiteTurn, PiecesList);
        if (positionScore == 1) return float.MaxValue;
        if (positionScore == -1) return -1 * float.MaxValue;
        foreach (PieceClass piece in PiecesList)
        {
            float value = 3.0f;
            if (piece.IsQueen) value = 5;
            if (!piece.IsWhite) value *= -1;
            if (piece.X == 0 || piece.Y == 0 || piece.X == BoardSize - 1 || piece.Y == BoardSize - 1)
            {
                value *= 1.30f;
            }
            else if (piece.X == 1 || piece.Y == 1 || piece.X == BoardSize - 2 || piece.Y == BoardSize - 2)
            {
                value *= 1.10f;
            }
            positionScore += value;
        }
        if (drawCounter > 30)
        {
           // Debug.Log("Remis!");
            draw = true;
            return 0.0f;
        }
        else draw = false;
        return positionScore;
    }
    public void RandomMove()
    {
        int counter = (int)(Random.Range(0.0f, 7.0f));
        foreach(PieceClass piece in PiecesList)
        {
            foreach(Move move in piece.Moves)
            {
                if (counter == 0)
                {
                    play.BotMove(piece, move);
                    counter = -1;
                    break;
                }
                counter--;
            }
        }
    }

    public void startMinMaxBot(bool isWhiteTurn)
    {
        (_, PieceClass BestPiece, Move BestMove) = MinMax(PiecesList, BotDepth, IsBotWhiteColor, isWhiteTurn, null, null);
        play.BotMove(BestPiece, BestMove);
    }

    public void startAlphaBetaBot(bool isWhiteTurn)
    {
        (_, PieceClass BestPiece, Move BestMove) = AlphaBeta(PiecesList, float.MinValue, float.MaxValue, BotDepth, IsBotWhiteColor, isWhiteTurn, null, null);
        play.BotMove(BestPiece, BestMove);
    }

    public (float, PieceClass, Move) MinMax(List<PieceClass> ListOfPieces, int depth, bool maximizingPlayer, bool isWhiteTurn, PieceClass BestPiece, Move BestMove)
    {
        float positionScore = RatePosition(isWhiteTurn, ListOfPieces);
        if (depth == 0 || positionScore == int.MaxValue || positionScore == int.MinValue || draw)
            return (positionScore, BestPiece, BestMove);
        if (maximizingPlayer)
        {
            float maxEval = -int.MaxValue;
            foreach (PieceClass piece in ListOfPieces)
            {
                foreach (Move move in piece.Moves)
                {
                    List<PieceClass> CopiedListOfPieces = getListOfPieceCopy(ListOfPieces);
                    SimulateAMove(move, piece, CopiedListOfPieces, !isWhiteTurn);
                    (float actualScore, _, _) = MinMax(CopiedListOfPieces, depth - 1, false, !isWhiteTurn, BestPiece, BestMove);
                    if (actualScore > maxEval)
                    {
                        BestPiece = piece;
                        BestMove = move;
                        maxEval = actualScore;
                    }
                }
            }
            return (maxEval, BestPiece, BestMove);
        }
        else
        {
            float minEval = int.MaxValue;
            foreach (PieceClass piece in ListOfPieces)
            {
                foreach (Move move in piece.Moves)
                {
                    List<PieceClass> CopiedListOfPieces = getListOfPieceCopy(ListOfPieces);
                    SimulateAMove(move, piece, CopiedListOfPieces, !isWhiteTurn);
                    (float actualScore, _, _) = MinMax(CopiedListOfPieces, depth - 1, true, !isWhiteTurn, BestPiece, BestMove);

                    if (actualScore < minEval)
                    {
                        BestPiece = piece;
                        BestMove = move;
                        minEval = actualScore;
                    }
                }
            }
            return (minEval, BestPiece, BestMove);
        }
    }

    public (float, PieceClass, Move) AlphaBeta(List<PieceClass> ListOfPieces, float alpha, float beta, int depth, bool maximizingPlayer, bool isWhiteTurn, PieceClass BestPiece, Move BestMove)
    {
        float positionScore = RatePosition(isWhiteTurn, ListOfPieces);
        if (depth == 0 || positionScore == int.MaxValue || positionScore == int.MinValue || draw)
            return (positionScore, BestPiece, BestMove);
        if (maximizingPlayer)
        {
            float maxEval = -int.MaxValue;
            foreach (PieceClass piece in ListOfPieces)
            {
                foreach (Move move in piece.Moves)
                {
                    List<PieceClass> CopiedListOfPieces = getListOfPieceCopy(ListOfPieces);
                    SimulateAMove(move, piece, CopiedListOfPieces, !isWhiteTurn);
                    (float actualScore, _, _) = AlphaBeta(CopiedListOfPieces, alpha, beta, depth - 1, false, !isWhiteTurn, BestPiece, BestMove);             
                    if (actualScore > maxEval)
                    {
                        BestPiece = piece;
                        BestMove = move;
                        maxEval = actualScore;
                    }
                    alpha = Mathf.Max(alpha, actualScore);
                    if(beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return (maxEval, BestPiece, BestMove);
        }
        else
        {
            float minEval = int.MaxValue;
            foreach (PieceClass piece in ListOfPieces)
            {
                foreach (Move move in piece.Moves)
                {
                    List<PieceClass> CopiedListOfPieces = getListOfPieceCopy(ListOfPieces);
                    SimulateAMove(move, piece, CopiedListOfPieces, !isWhiteTurn);
                    (float actualScore, _, _) = AlphaBeta(CopiedListOfPieces, alpha, beta, depth - 1, true, !isWhiteTurn, BestPiece, BestMove);
                    if (actualScore < minEval)
                    {
                        BestPiece = piece;
                        BestMove = move;
                        minEval = actualScore;
                    }
                    beta = Mathf.Min(beta, actualScore);
                    if(beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return (minEval, BestPiece, BestMove);
        }
    }

    private void SimulateAMove(Move move, PieceClass piece, List<PieceClass> PiecesList, bool isWhiteTurn)
    {
        (int x, int y) = move.MoveList[move.MoveList.Count - 1];
        List<PieceClass> listOfPieceToBeRemoved = new List<PieceClass>();

        foreach (PieceClass capturedPiece in move.CapturedPiecesList)
        {
            foreach (PieceClass ActualPiece in PiecesList)
            {
                if(capturedPiece.X == ActualPiece.X && capturedPiece.Y == ActualPiece.Y)
                {
                    listOfPieceToBeRemoved.Add(ActualPiece);
                }
            }
        }
        PiecesList.RemoveAll(x => listOfPieceToBeRemoved.Contains(x));
        foreach (PieceClass p in PiecesList)
        {
            if(p.X == piece.X && p.Y == piece.Y)
            {
                p.X = x;
                p.Y = y;
                if (p.IsWhite && p.X == 0)
                {
                    p.IsQueen = true;
                }
                else if(!p.IsWhite && p.X == BoardSize - 1)
                {
                    p.IsQueen = true;
                }
            }
            p.Moves.Clear();
        }
        play.CheckPossibleMoves(isWhiteTurn, PiecesList);
    }

    private List<PieceClass> getListOfPieceCopy(List<PieceClass> listOfPieces)
    {
        List<PieceClass> result = new List<PieceClass>();
        foreach(PieceClass piece in listOfPieces)
        {
            result.Add(getPieceCopy(piece));
        }
        return result;
    }

    private PieceClass getPieceCopy(PieceClass piece)
    {
        PieceClass result = new PieceClass();
        result.X = piece.X;
        result.Y = piece.Y;
        result.IsQueen = piece.IsQueen;
        result.IsWhite = piece.IsWhite;
        List<Move> newMoves = new List<Move>();
        foreach(Move move in piece.Moves)
        {
            Move CopiedMove = new Move();
            CopiedMove.CapturedPiecesList = move.CapturedPiecesList.GetClone();
            CopiedMove.MoveList = move.MoveList.GetClone();
            newMoves.Add(CopiedMove);
        }
        result.Moves = newMoves;
        return result;
    }
    
}
