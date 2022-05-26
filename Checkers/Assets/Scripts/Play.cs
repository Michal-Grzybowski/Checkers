using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public static class Extensions
{
    public static List<T> GetClone<T>(this List<T> source)
    {
        return source.GetRange(0, source.Count);
    }
}
public class Play : MonoBehaviour
{

    public static int PiecesRowNumber;
    public static int BoardSize;
    public static GameObject[,] Board;
    public static PieceClass[,] ActualBoard;
    public static List<PieceClass> PiecesList;
    public static Color[] colors;
    public static List<(int, int)> LastMoves;
    public static PieceClass ActualPiece;
    public static Initialise init;
    public static int MovesCounterAfterFirstQueen;
    public List<Move> ActualListOfMoves;
    public static GameEngine Engine;
    public static GameEngine Engine2;
    public static bool IsWhiteBotTurn;
    public static int RandomMoveCounter = 4;
    public GameOverScreen GameOverScreen;

    //menu configuration 
    public static int GameType = 0;
    public static bool IsFirstBotWhite = true;
    public static string FirstBotAlgorithm = "MinMax";
    public static int FirstBotDepth = 3;
    public static int SecondBotDepth = 3;
    public static string SecondBotAlgorithm = "MinMax";


    /*TODO List:
        -remis po 15 ruchach od 1 damki
        -wygrane jak zbite piony i jak nie moze sie ruszyc

        - funkcja celu
        - min max
    */

    void Start()
    {
        Board = Initialise.Board;
        colors = Initialise.colors;
        PiecesRowNumber = Initialise.PiecesRowNumber;
        BoardSize = Initialise.BoardSize;
        PiecesList = Initialise.PiecesList;
        LastMoves = new List<(int, int)>();
        ActualPiece = null;
        init = GameObject.Find("Canvas").GetComponent<Initialise>();
        MovesCounterAfterFirstQueen = 0;

        CheckPossibleMoves(true, PiecesList);
        if (IsFirstBotWhite)
            Debug.Log("Bot1 color: white");
        else
            Debug.Log("Bot1 color: black");
        Debug.Log("Bot1 Algorithm: " + FirstBotAlgorithm);
        Debug.Log("Bot2 Algorithm: " + SecondBotAlgorithm);
        Debug.Log("Bot1 depth: " + FirstBotDepth);
        Debug.Log("Bot2 depth: " + SecondBotDepth);
        if (GameType == 1)
        {
            Debug.Log("Player vs AI mode");
            Engine = new GameEngine();
            Engine.EngineInitialise(BoardSize, PiecesList, MovesCounterAfterFirstQueen, FirstBotDepth, IsFirstBotWhite);
            if (IsFirstBotWhite)
            {
                if (FirstBotAlgorithm.Equals("MinMax"))
                {
                    Engine.startMinMaxBot(true);
                }
                else if (FirstBotAlgorithm.Equals("AlphaBeta"))
                {
                    Engine.startAlphaBetaBot(true);
                }
            }
        }
        else if (GameType == 2)
        {
            Debug.Log("AI vs AI mode");
            Engine = new GameEngine();
            Engine.EngineInitialise(BoardSize, PiecesList, MovesCounterAfterFirstQueen, FirstBotDepth, IsFirstBotWhite);

            Engine2 = new GameEngine();
            Engine2.EngineInitialise(BoardSize, PiecesList, MovesCounterAfterFirstQueen, SecondBotDepth, !IsFirstBotWhite);
            //Engine.startMinMaxBot(true);
        }
        IsWhiteBotTurn = true;
    }

    public void StartGame()
    {
        if (GameType == 2)
        {
            if(IsWhiteBotTurn == IsFirstBotWhite)
            {
                if(RandomMoveCounter > 0)
                {
                    RandomMoveCounter--;
                    Engine.RandomMove();
                }
                else if (FirstBotAlgorithm.Equals("MinMax"))
                {
                    Engine.startMinMaxBot(IsWhiteBotTurn);
                }
                else if (FirstBotAlgorithm.Equals("AlphaBeta"))
                {
                    Engine.startAlphaBetaBot(IsWhiteBotTurn);
                }
                //Engine.startMinMaxBot(IsWhiteBotTurn);
            }
            else
            {
                if(RandomMoveCounter > 0)
                {
                    RandomMoveCounter--;
                    Engine2.RandomMove();
                }
                else if (SecondBotAlgorithm.Equals("MinMax"))
                {
                    Engine2.startMinMaxBot(IsWhiteBotTurn);
                }
                else if (SecondBotAlgorithm.Equals("AlphaBeta"))
                {
                    Engine2.startAlphaBetaBot(IsWhiteBotTurn);
                }
            }
        }
    }


    public void ShowMoves(Piece piece)
    {
        UnSelect();
        ActualPiece = piece.Get;
        foreach (Move move in piece.Get.Moves)
        {
            int counter = 0;
            foreach ((int, int) dest in move.MoveList)
            {
                (int x, int y) = dest;
                if (counter == move.MoveList.Count - 1)
                {
                    Board[x, y].GetComponent<Image>().color = Color.blue;
                    Board[x, y].GetComponent<Button>().enabled = true;
                    LastMoves.Add((x, y));
                }
                else
                {
                    Board[x, y].GetComponent<Image>().color = colors[2];
                    LastMoves.Add((x, y));
                }
                counter++;
            }
        }

    }
    
    public int CheckCapture(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        int maxCapture = 0;
        ActualiseCopiedBoard(PiecesList);
        foreach (PieceClass piece in PiecesList)
        {
            if(piece.IsWhite == isWhiteTurn)
            {
                piece.Moves = FindeBestCapturesForThisPiece(piece);
                if (piece.Moves.Count > 0)
                {
                    maxCapture = piece.Moves[0].CapturedPiecesList.Count;
                }
            }
        }
        //wykluczam bicia ktore nie sa najdluzsze
/*        foreach (Piece piece in PiecesList)
        {
            foreach(Move move in piece.Moves)
            {
                if (move.CapturedPiecesList.Count < maxCapture)
                {
                    piece.Moves.Remove(move);
                }
            }
        }*/
        return maxCapture;
    }


    private List<Move> FindeBestCapturesForThisPiece (PieceClass piece)
    {
        ActualListOfMoves = new List<Move>();
        List<(int, int)> ActualMoveHistory = new List<(int, int)>();
        List<PieceClass> ActualCapturedPiecesList = new List<PieceClass>();
        
        if (piece.IsQueen)
        {
            FindPossibleCaptureForThisQueen(piece, piece.X, piece.Y, ActualMoveHistory, ActualCapturedPiecesList);
        }
        else
        {
            FindPossibleCaptureForThisPiece(piece, piece.X, piece.Y, ActualMoveHistory, ActualCapturedPiecesList);

        }

        return ActualListOfMoves;
    }

    //tutaj korzysta z aktualnego stanu planszy a nie z tego ktory jest w drzewie
    private void FindPossibleCaptureForThisPiece(PieceClass piece, int x, int y, List<(int, int)> ActualMoveHistory, List<PieceClass> ActualCapturedPiecesList)
    {

        bool isCaptureFound = false;
        if (y > 1 && x > 1) //bicie lewa gora
        {
            if (ActualBoard[x - 1, y - 1] != null  //spradzam czy stoi pion
                && ActualBoard[x - 1, y - 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x - 2, y - 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x - 1, y-1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;

                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x-2, y-2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x - 1, y - 1]);

                FindPossibleCaptureForThisPiece(piece, x-2, y-2, CopiedMoveHistory, CopiedCapturedPieces);
            }
        }
        if (y < BoardSize - 2 && x > 1) // bicie prawa gora
        {
            if (ActualBoard[x - 1, y + 1] != null //spradzam czy stoi pion
                && ActualBoard[x - 1, y + 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x - 2, y + 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x - 1, y + 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;
                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x-2, y+2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x - 1, y + 1]);

                FindPossibleCaptureForThisPiece(piece, x - 2, y + 2, CopiedMoveHistory, CopiedCapturedPieces);

            }
        }
        if (x < BoardSize - 2 && y > 1) //lewy dó³
        {
            if (ActualBoard[x + 1, y - 1] != null //spradzam czy stoi pion
                && ActualBoard[x + 1, y - 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x + 2, y - 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x + 1, y - 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;
                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x+2, y-2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x + 1, y - 1]);

                FindPossibleCaptureForThisPiece(piece, x + 2, y - 2, CopiedMoveHistory, CopiedCapturedPieces);

            }
        }
        if (x < BoardSize - 2 && y < BoardSize - 2) //prawy dol
        {
            if (ActualBoard[x + 1, y + 1] != null //spradzam czy stoi pion
                && ActualBoard[x + 1, y + 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x + 2, y + 2] == null// czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x + 1, y + 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;

                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x+2, y+2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x + 1, y + 1]);

                FindPossibleCaptureForThisPiece(piece, x + 2, y + 2, CopiedMoveHistory, CopiedCapturedPieces);
            }
        }
        if (!isCaptureFound)
        {
            if(ActualCapturedPiecesList.Count > 0)
            {
                if (ActualListOfMoves.Count == 0 || ActualCapturedPiecesList.Count > ActualListOfMoves[0].CapturedPiecesList.Count)
                {
                    ActualListOfMoves.Clear();
                    Move newMove = new Move();
                    //czy tu nie potrzeba kopii?
                    newMove.CapturedPiecesList = ActualCapturedPiecesList;
                    newMove.MoveList = ActualMoveHistory;
                    ActualListOfMoves.Add(newMove);
                }
                else if(ActualListOfMoves.Count == 0 || ActualCapturedPiecesList.Count == ActualListOfMoves[0].CapturedPiecesList.Count)
                {
                    Move newMove = new Move();
                    //czy tu nie potrzeba kopii?
                    newMove.CapturedPiecesList = ActualCapturedPiecesList;
                    newMove.MoveList = ActualMoveHistory;
                    ActualListOfMoves.Add(newMove);
                }
            }
        }
    }

    private void FindPossibleCaptureForThisQueen(PieceClass piece, int x, int y, List<(int, int)> ActualMoveHistory, List<PieceClass> ActualCapturedPiecesList)
    {
        bool isCaptureFound = false;
        int main_x = x;
        int main_y = y;
        while (y > 1 && x > 1) //bicie lewa gora
        {
            if (ActualBoard[x - 1, y - 1] != null //spradzam czy stoi pion
                && ActualBoard[x - 1, y - 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x - 2, y - 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x - 1, y - 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;

                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x - 2, y - 2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x - 1, y - 1]);

                FindPossibleCaptureForThisQueen(piece, x - 2, y - 2, CopiedMoveHistory, CopiedCapturedPieces);
                x = -1;
            }
            //jesli napotkam dwa piony obok siebie lub swoj pion to porzucam eksploracje
            else if ((ActualBoard[x - 1, y - 1] != null
                && ActualBoard[x - 2, y - 2] != null)    
                || (ActualBoard[x - 1, y - 1] != null
                && ActualBoard[x - 1, y - 1].IsWhite == piece.IsWhite))
            {
                x = -1;
            }
            else
            {
                x--;
                y--;
            }
        }
        x = main_x;
        y = main_y;
        while (y < BoardSize - 2 && x > 1) // bicie prawa gora
        {
            if (ActualBoard[x - 1, y + 1] != null //spradzam czy stoi pion
                && ActualBoard[x - 1, y + 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x - 2, y + 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x - 1, y + 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;
                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x - 2, y + 2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x - 1, y + 1]);

                FindPossibleCaptureForThisQueen(piece, x - 2, y + 2, CopiedMoveHistory, CopiedCapturedPieces);
                x = -1;
            }
            else if ((ActualBoard[x - 1, y + 1] != null
               && ActualBoard[x - 2, y + 2] != null)
               || (ActualBoard[x - 1, y + 1] != null
               && ActualBoard[x - 1, y + 1].IsWhite == piece.IsWhite))
            {
                x = -1;
            }
            else
            {
                x--;
                y++;
            }
        }
        x = main_x;
        y = main_y;
        while (x < BoardSize - 2 && y > 1) //lewy dó³
        {
            if (ActualBoard[x + 1, y - 1] != null //spradzam czy stoi pion
                && ActualBoard[x + 1, y - 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x + 2, y - 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x + 1, y - 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;
                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x + 2, y - 2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x + 1, y - 1]);

                FindPossibleCaptureForThisQueen(piece, x + 2, y - 2, CopiedMoveHistory, CopiedCapturedPieces);
                y = -1;
            }
            else if ((ActualBoard[x + 1, y - 1] != null
              && ActualBoard[x + 2, y - 2] != null)
              || (ActualBoard[x + 1, y - 1] != null
              && ActualBoard[x + 1, y - 1].IsWhite == piece.IsWhite))
            {
                y = -1;
            }
            else
            {
                x++;
                y--;
            }
        }
        x = main_x;
        y = main_y;
        while (x < BoardSize - 2 && y < BoardSize - 2) //prawy dol
        {
            if (ActualBoard[x + 1, y + 1] != null //spradzam czy stoi pion
                && ActualBoard[x + 1, y + 1].IsWhite != piece.IsWhite //czy przeciwnika
                && ActualBoard[x + 2, y + 2] == null // czy za nim jest puste pole
                && !ActualCapturedPiecesList.Contains(ActualBoard[x + 1, y + 1]))// czy to nie jest pion ktory juz byl zbity
            {
                isCaptureFound = true;

                List<(int, int)> CopiedMoveHistory = ActualMoveHistory.GetClone();
                CopiedMoveHistory.Add((x + 2, y + 2));
                List<PieceClass> CopiedCapturedPieces = ActualCapturedPiecesList.GetClone();
                CopiedCapturedPieces.Add(ActualBoard[x + 1, y + 1]);

                FindPossibleCaptureForThisQueen(piece, x + 2, y + 2, CopiedMoveHistory, CopiedCapturedPieces);
                x = BoardSize;
            }
            else if ((ActualBoard[x + 1, y + 1] != null
                 && ActualBoard[x + 2, y + 2] != null)
                 || (ActualBoard[x + 1, y + 1] != null
                 && ActualBoard[x + 1, y + 1].IsWhite == piece.IsWhite))
            {
                x = BoardSize;
            }
            else
            {
                x++;
                y++;
            }
        }
        if (!isCaptureFound)
        {
            if (ActualCapturedPiecesList.Count > 0)
            {
                if (ActualListOfMoves.Count == 0 || ActualCapturedPiecesList.Count > ActualListOfMoves[0].CapturedPiecesList.Count)
                {
                    ActualListOfMoves.Clear();
                    Move newMove = new Move();
                    newMove.CapturedPiecesList = ActualCapturedPiecesList;
                    newMove.MoveList = ActualMoveHistory;
                    ActualListOfMoves.Add(newMove);
                }
                else if (ActualListOfMoves.Count == 0 || ActualCapturedPiecesList.Count == ActualListOfMoves[0].CapturedPiecesList.Count)
                {
                    Move newMove = new Move();
                    newMove.CapturedPiecesList = ActualCapturedPiecesList;
                    newMove.MoveList = ActualMoveHistory;
                    ActualListOfMoves.Add(newMove);
                }
            }
        }
    }
    public void CheckPossibleMoves(bool isWhiteTurn, List<PieceClass> PiecesList)
    {
        if(CheckCapture(isWhiteTurn, PiecesList) == 0)
        {
            //to przypisuje ruchy bez bicia

            foreach (PieceClass piece in PiecesList)
            {
                if (!piece.IsQueen)
                {
                    if (piece.IsWhite == isWhiteTurn)
                    {
                        if (isWhiteTurn)
                        {
                            if (piece.X > 0)
                            {
                                if (piece.Y > 0)
                                {
                                    int x = piece.X - 1;
                                    int y = piece.Y - 1;
                                    if (Board[x, y].transform.childCount == 0)
                                    {
                                        AddMove(piece, x, y);
                                    }
                                }
                                if (piece.Y < BoardSize - 1)
                                {
                                    int x = piece.X - 1;
                                    int y = piece.Y + 1;
                                    if (Board[x, y].transform.childCount == 0)
                                    {
                                        AddMove(piece, x, y);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (piece.X < BoardSize - 1)
                            {
                                if (piece.Y > 0)
                                {
                                    int x = piece.X + 1;
                                    int y = piece.Y - 1;
                                    if (Board[x, y].transform.childCount == 0)
                                    {
                                        AddMove(piece, x, y);
                                    }
                                }
                                if (piece.Y < BoardSize - 1)
                                {
                                    int x = piece.X + 1;
                                    int y = piece.Y + 1;
                                    if (Board[x, y].transform.childCount == 0)
                                    {
                                        AddMove(piece, x, y);
                                    }
                                }
                            }
                        }
                    }
                }
                else // tu przypisuje ruchy damki bez bicia.
                {
                    if (piece.IsWhite == isWhiteTurn)
                    {
                        int x = piece.X;
                        int y = piece.Y;
                        while (x>0 && y > 0) //lewa gora
                        {
                            x--;
                            y--;
                            if (Board[x, y].transform.childCount == 0)
                            {
                                AddMove(piece, x, y);
                            }
                            else
                            {
                                x = 0;
                            }
                        }
                        x = piece.X;
                        y = piece.Y;
                        while (x > 0 && y <BoardSize - 1) //prawa gora
                        {
                            x--;
                            y++;
                            if (Board[x, y].transform.childCount == 0)
                            {
                                AddMove(piece, x, y);
                            }
                            else
                            {
                                x = 0;
                            }
                        }
                        x = piece.X;
                        y = piece.Y;
                        while (y > 0 && x < BoardSize - 1) //lewy dol
                        {
                            x++;
                            y--;
                            if (Board[x, y].transform.childCount == 0)
                            {
                                AddMove(piece, x, y);
                            }
                            else
                            {
                                y = 0;
                            }
                        }
                        x = piece.X;
                        y = piece.Y;
                        while (x < BoardSize - 1 && y < BoardSize - 1) //prawy dol
                        {
                            x++;
                            y++;
                            if (Board[x, y].transform.childCount == 0)
                            {
                                AddMove(piece, x, y);
                            }
                            else
                            {
                                x = BoardSize;
                            }
                        }
                    }
                }
            }
        } 
        //w przeciwnym przypadku CheckCapture je przypisal

    }

    public void Move(Tile Tile)
    {
        //wyciagniecie wsp. pola na ktore sie przenosi pionek
        /*        int x = (int.Parse(Tile.name))/10;
                int y = (int.Parse(Tile.name))%10;*/
        int x = Tile.X;
        int y = Tile.Y;

        Piece ActualPieceMono = getPieceFromBoard(ActualPiece.X, ActualPiece.Y);
        //odznaczenie pol i usuniecie pionka
        //Board[ActualPiece.X, ActualPiece.Y].transform.DetachChildren();
        UnSelect();

        //przeniesienie pionka
        ActualPiece.X = x;
        ActualPiece.Y = y;
        ActualPieceMono.Get.X = x;
        ActualPieceMono.Get.Y = y;
        ActualPieceMono.transform.SetParent(Board[x, y].transform, false);
       // ActualPieceMono.transform.localPosition = Vector3.zero;

        //sprawdzic czy doszedl do konca jesli tak to zmienic w damke
        if((x == 0 && ActualPiece.IsWhite) || (x == BoardSize - 1 && !ActualPiece.IsWhite))
        {
            //zmieniam w damke bia³¹
            PromoteToQueen(ActualPiece);
        }

        Move PickedMove = null;
        foreach(Move move in ActualPiece.Moves)
        {
            (int a, int b) = move.MoveList[move.MoveList.Count - 1];
            if (a == x && b == y)
            {
                PickedMove = move;
            }
        }

        //usuwanie zbitych
        if (PickedMove.CapturedPiecesList.Count > 0)
        {
            foreach (PieceClass piece in PickedMove.CapturedPiecesList)
            {
                Board[piece.X, piece.Y].transform.DetachChildren();
                PiecesList.Remove(piece);
            }
        }

        //zmiana tury
        foreach (PieceClass p in PiecesList)
        {
            //jesli tego samego koloru 
            if (p.IsWhite == ActualPiece.IsWhite)
            {
                p.Moves.Clear();
                getPieceFromBoard(p.X, p.Y).GetComponent<Button>().enabled = false;
            }
            else
            {
                getPieceFromBoard(p.X, p.Y).GetComponent<Button>().enabled = true;
            }
        }
        CheckPossibleMoves(!ActualPiece.IsWhite, PiecesList);

        if (MovesCounterAfterFirstQueen > 0)
        {
            MovesCounterAfterFirstQueen++;
        }

        //sprawdz warunki zakonczenia

        //remis
        if (MovesCounterAfterFirstQueen > 30)
        {
            Debug.Log("Remis!");
            GameOverScreen.EndGame(0);
        }

        //sprawdzanie wygranej
        bool hasMove = false;
        bool pieceExist = false;
        foreach (PieceClass p in PiecesList)
        {
            if (p.IsWhite != ActualPiece.IsWhite)
            {
                pieceExist = true;
                if (p.Moves.Count > 0)
                {
                    hasMove = true;
                }
            }
        }
        //drukowanie wygranej
        if(!(pieceExist && hasMove))
        {
            if (ActualPiece.IsWhite)
            {
                Debug.Log("Wygraly biale");
                GameOverScreen.EndGame(1);
            }
            else
            {
                Debug.Log("Wygraly czarne");
                GameOverScreen.EndGame(-1);
            }
        }
        if(GameType == 1)
        {
            Engine.ActualiseData(PiecesList, MovesCounterAfterFirstQueen);

            if (FirstBotAlgorithm.Equals("MinMax"))
            {
                Engine.startMinMaxBot(!ActualPiece.IsWhite);
            }
            else if (FirstBotAlgorithm.Equals("AlphaBeta"))
            {
                Engine.startAlphaBetaBot(!ActualPiece.IsWhite);
            }
        }
        ActualPiece = null;
    }

     
    public void BotMove(PieceClass piece, Move PickedMove)
    {
        (int x, int y) = PickedMove.MoveList[PickedMove.MoveList.Count - 1];

        //odznaczenie pol i usuniecie pionka
        Piece ActualPieceMono = getPieceFromBoard(piece.X, piece.Y);
       // Board[piece.X, piece.Y].transform.DetachChildren();

        //przeniesienie pionka

        piece.X = x;
        piece.Y = y;
        ActualPieceMono.Get.X = x;
        ActualPieceMono.Get.Y = y;
        ActualPieceMono.transform.SetParent(Board[x, y].transform, false);
        //ActualPieceMono.transform.localPosition = Vector3.zero;

        //sprawdzic czy doszedl do konca jesli tak to zmienic w damke
        if ((x == 0 && piece.IsWhite) || (x == BoardSize - 1 && !piece.IsWhite))
        {
            //zmieniam w damke bia³¹
            PromoteToQueen(piece);
        }

        //usuwanie zbitych
        if (PickedMove.CapturedPiecesList.Count > 0)
        {
            foreach (PieceClass p in PickedMove.CapturedPiecesList)
            {
                Board[p.X, p.Y].transform.DetachChildren();
                PiecesList.Remove(p);
            }
        }

        //zmiana tury
        foreach (PieceClass p in PiecesList)
        {
            //jesli tego samego koloru 
            if (p.IsWhite == piece.IsWhite)
            {
                p.Moves.Clear();
                getPieceFromBoard(p.X, p.Y).GetComponent<Button>().enabled = false;
            }
            else
            {
                getPieceFromBoard(p.X, p.Y).GetComponent<Button>().enabled = true;
            }
        }
        CheckPossibleMoves(!piece.IsWhite, PiecesList);

        if (MovesCounterAfterFirstQueen > 0)
        {
            MovesCounterAfterFirstQueen++;
        }

        //sprawdz warunki zakonczenia
        //remis
        if (MovesCounterAfterFirstQueen > 30)
        {
            Debug.Log("Remis!");
            GameOverScreen.EndGame(0);
        }

        //sprawdzanie wygranej
        bool hasMove = false;
        bool pieceExist = false;
        foreach (PieceClass p in PiecesList)
        {
            if (p.IsWhite != piece.IsWhite)
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
            if (piece.IsWhite)
            {
                Debug.Log("Wygraly biale");
                GameOverScreen.EndGame(1);
            }
            else
            {
                Debug.Log("Wygraly czarne");
                GameOverScreen.EndGame(-1);
            }
        }
        // Debug.Log(Engine.RatePosition(!piece.IsWhite, PiecesList));
        IsWhiteBotTurn = !piece.IsWhite;
        if(GameType == 2)
        {
            //System.Threading.Thread.Sleep(1000);
            //Start.Couroutine
            if(piece.IsWhite == Engine.IsBotWhiteColor)
            {
                Engine2.ActualiseData(PiecesList, MovesCounterAfterFirstQueen);
/*                if (SecondBotAlgorithm.Equals("MinMax"))
                {
                    Engine2.startMinMaxBot(!piece.IsWhite);
                }
                else if (SecondBotAlgorithm.Equals("AlphaBeta"))
                {
                    Engine2.startAlphaBetaBot(!piece.IsWhite);
                }*/
               // Engine2.startMinMaxBot(!piece.IsWhite);
            }
            else
            {
                Engine.ActualiseData(PiecesList, MovesCounterAfterFirstQueen);
/*                if (FirstBotAlgorithm.Equals("MinMax"))
                {
                    Engine.startMinMaxBot(!piece.IsWhite);
                }
                else if (FirstBotAlgorithm.Equals("AlphaBeta"))
                {
                    Engine.startAlphaBetaBot(!piece.IsWhite);
                }*/
                // Engine.startMinMaxBot(!piece.IsWhite);
            }
        }
    }

    public void PromoteToQueen(PieceClass piece)
    {
        piece.IsQueen = true;
        Image img = Board[piece.X, piece.Y].transform.GetChild(0).GetComponent<Image>();
        if (piece.IsWhite)
        {
            img.sprite = init.QueenWhite;
        }
        else
        {
            img.sprite = init.QueenBlack;
            img.color = Color.white;
        }
        if (MovesCounterAfterFirstQueen == 0)
        {
            MovesCounterAfterFirstQueen++;
        }
    }

    // -----------------------------------support functions----------------------------------




    private void AddMove(PieceClass piece, int x, int y)
    {
        Move move = new Move();
        List<(int, int)> MoveHistory = new List<(int, int)>();
        List<PieceClass> CapturedPieces = new List<PieceClass>();
        move.CapturedPiecesList = CapturedPieces;
        MoveHistory.Add((x, y));
        move.MoveList = MoveHistory;
        piece.Moves.Add(move);

    }
    private Piece getPieceFromBoard(int x, int y)
    {
        return Board[x, y].transform.GetChild(0).gameObject.GetComponentInChildren(typeof(Piece)) as Piece;

    }


    private void UnSelect()
    {
        if (LastMoves.Count > 0)
        {
            foreach ((int, int) elem in LastMoves)
            {
                (int x, int y) = elem;
                Board[x, y].GetComponent<Image>().color = colors[1];
                Board[x, y].GetComponent<Button>().enabled = false;
            }
        }
        LastMoves.Clear();
    }

    private void ActualiseCopiedBoard(List<PieceClass> PiecesList)
    {
        ActualBoard = new PieceClass[BoardSize, BoardSize];
        foreach(PieceClass piece in PiecesList)
        {
            ActualBoard[piece.X, piece.Y] = piece;
        }
    }
}
