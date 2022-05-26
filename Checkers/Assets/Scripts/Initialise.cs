using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initialise : MonoBehaviour
{
    public static int PiecesRowNumber = 3;
    public static int BoardSize = 8;
    public GameObject Tile;
    public GameObject Piece;
    public static GameObject[,] Board;
    public static List<PieceClass> PiecesList;
    public static Color[] colors = new Color[] { new Color(203/255f,138/255f,64/255f,1)
            , new Color(100/255f, 51/255f, 5/255f, 1)
            , new Color(137/255f, 196/255f, 244/255f ,1)};
    public Sprite QueenBlack;
    public Sprite QueenWhite;


    // Start is called before the first frame update
    void Start()
    {

    }
    void Awake()
    {
        Board = new GameObject[BoardSize, BoardSize];
        PiecesList = new List<PieceClass>();
        GenerateBoard();
        PlacePieces();
    }

    private void GenerateBoard()
    {
        Transform panel = transform.Find("Panel");
        Vector2 canvasSize = gameObject.GetComponent<RectTransform>().sizeDelta; //cs
        Vector2 tileSize = Tile.GetComponent<RectTransform>().sizeDelta; //  size
        canvasSize.x /= 2;
        canvasSize.y /= 2;
        float left = (canvasSize.x - tileSize.x) * -1;
        float top = canvasSize.y - tileSize.y;
        Image tileImage = Tile.GetComponent<Image>();
        for (int i = 0; i<BoardSize; i++)
        {
            for(int j = 0; j < BoardSize; j++)
            {
                tileImage.color = colors[j % 2];
                Board[i, j] = Instantiate(Tile);
                Board[i, j].transform.SetParent(panel);
                Board[i, j].transform.localPosition = new Vector3(left, top);
                Board[i, j].transform.name = ((i) * 10 + (j)).ToString();
                Tile tile = Board[i, j].GetComponent<Tile>();
                Board[i, j].GetComponent<Button>().enabled = false;
                tile.X = i;
                tile.Y = j;
                left += tileSize.x; 

/*                tileImage.color = colors[j % 2];
                Board[i, j]= Instantiate(Tile);
                Board[i, j].transform.SetParent(panel);
                Board[i, j].transform.localPosition = new Vector3(left, top);
                Board[i, j].transform.name = ((i)*10 + (j)).ToString();
                Board[i, j].GetComponent<Button>().enabled = false;
                left += tileSize.x;*/
            }
            left = (canvasSize.x - tileSize.x) * -1;
            top -= tileSize.y;

            colors = swapColors(colors);
        }
    }

    private Color[] swapColors(Color[] colors)
    {
        Color tempCol;
        tempCol = colors[0];
        colors[0] = colors[1];
        colors[1] = tempCol;
        return colors;
    }

    private void PlacePieces()
    {
        Image img = Piece.GetComponent<Image>();
        img.color = Color.black;
        int counter = 0;
        for (int i = 0; i<PiecesRowNumber; i++)
        {
            for (int j = (i+1) % 2; j<BoardSize; j += 2)
            {
                counter += 1;
                GameObject newPiece = Instantiate(Piece);
                newPiece.name = counter.ToString();
                newPiece.transform.SetParent(Board[i, j].transform);
                newPiece.transform.localPosition = Vector3.zero;
                Piece piece = newPiece.GetComponent<Piece>();
                newPiece.GetComponent<Button>().enabled = false;
                PieceClass Pc = new PieceClass();
                Pc.X = i;
                Pc.Y = j;
                Pc.IsWhite = false;
                Pc.IsQueen = false;
                Pc.Moves = new List<Move>();
                piece.Get = Pc;
                PiecesList.Add(Pc);
            }
        }

        img.color = Color.white;
        for (int i = BoardSize - PiecesRowNumber; i < BoardSize; i++)
        {
            for (int j = (i + 1) % 2; j < BoardSize; j += 2)
            {
                counter += 1;
                GameObject newPiece = Instantiate(Piece);
                newPiece.name = counter.ToString();
                newPiece.transform.SetParent(Board[i, j].transform);
                newPiece.transform.localPosition = Vector3.zero;
                Piece piece = newPiece.GetComponent<Piece>();
                newPiece.GetComponent<Button>().enabled = true;
                PieceClass Pc = new PieceClass();
                Pc.X = i;
                Pc.Y = j;
                Pc.IsWhite = true;
                Pc.IsQueen = false;
                Pc.Moves = new List<Move>();
                piece.Get = Pc;
                PiecesList.Add(Pc);
            }
        }
    }
}
