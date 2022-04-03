using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;
    [SerializeField] private Vector2Int spawnPosition;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI GameoverText;

    public float TickDelay;
    public float AnimationTickDelay;

    private int score;
    private bool isGameOver;

    private Tile[][] field;

    private float lastTick;
    private bool isPlayingAnimation;

    private Piece currentPiece;

    private void Start()
    {
        field = new Tile[10][];
        for (int i = 0; i < field.Length; i++)
        {
            field[i] = new Tile[20];
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            field[i % 10][i / 10] = tiles[i];
        }

        foreach (var tile in tiles)
        {
            tile.SetEmpty();
        }

        GameoverText.gameObject.SetActive(false);
        ScoreText.text = "0";
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (isPlayingAnimation)
        {
            if (Time.time > lastTick + AnimationTickDelay)
            {
                PlayAnimations();
                if (isPlayingAnimation)
                {
                    CheckField();
                }
            }
        }
        else
        {
            PieceUpdate();
        }
    }

    private void PieceUpdate()
    {
        PreMathRenderClear();

        if (currentPiece != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                var rotatedPiecePoints = currentPiece.GetRotatedRightPoints();
                ClampPiecePoints(rotatedPiecePoints);

                bool isRotationValid = true;
                foreach (var point in rotatedPiecePoints)
                {
                    if (point.y < 20 && IsBlock(point.x, point.y))
                    {
                        isRotationValid = false;
                        break;
                    }
                }

                if (isRotationValid)
                {
                    currentPiece.Points = currentPiece.RotateRight();
                    currentPiece.Position += rotatedPiecePoints[0] - (currentPiece.Points[0] + currentPiece.Position);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (CanMove(Vector2Int.right))
                {
                    currentPiece.Position += Vector2Int.right;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (CanMove(Vector2Int.left))
                {
                    currentPiece.Position += Vector2Int.left;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                while (CanMove(Vector2Int.down))
                {
                    currentPiece.Position += Vector2Int.down;
                }
            }
        }

        if (currentPiece == null)
        {
            currentPiece = new Piece()
                {Points = FigureList.Figures[Random.Range(0, FigureList.Figures.Length)], Position = spawnPosition};
            currentPiece.RandomizeColors(4);
            lastTick = Time.time;
        }

        if (Time.time > lastTick + TickDelay)
        {
            TickDelay *= 0.999f;
            FigureDrop();
        }

        if (currentPiece != null)
        {
            RenderFigure();
        }
    }

    private static void ClampPiecePoints(Vector2Int[] rotatedPiecePoints)
    {
        for (int i = 0; i < 4; i++)
        {
            if (rotatedPiecePoints[i].x < 0)
            {
                var offset = -rotatedPiecePoints[i].x;
                for (int j = 0; j < 4; j++)
                {
                    rotatedPiecePoints[j] += Vector2Int.right * offset;
                }
            }
            else if (rotatedPiecePoints[i].x > 9)
            {
                var offset = rotatedPiecePoints[i].x - 9;
                for (int j = 0; j < 4; j++)
                {
                    rotatedPiecePoints[j] -= Vector2Int.right * offset;
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (rotatedPiecePoints[i].y < 0)
            {
                var offset = -rotatedPiecePoints[i].y;
                for (int j = 0; j < 4; j++)
                {
                    rotatedPiecePoints[j] += Vector2Int.up * offset;
                }
            }
        }
    }

    private void PreMathRenderClear()
    {
        if (currentPiece == null)
        {
            return;
        }

        var position = currentPiece.Position;
        for (int i = 0; i < currentPiece.Points.Length; i++)
        {
            var pos = position + currentPiece.Points[i];
            if (pos.x < 10 && pos.y < 20)
            {
                field[pos.x][pos.y].SetEmpty();
            }
        }
    }

    private void RenderFigure()
    {
        var position = currentPiece.Position;
        for (int i = 0; i < currentPiece.Points.Length; i++)
        {
            var pos = position + currentPiece.Points[i];
            if (pos.x < 10 && pos.y < 20)
            {
                field[pos.x][pos.y].SetColor(currentPiece.Colors[i]);
            }
        }
    }

    private void FigureDrop()
    {
        if (CanMove(Vector2Int.down))
        {
            currentPiece.Position += Vector2Int.down;
        }
        else
        {
            foreach (var point in currentPiece.Points)
            {
                if (point.y + currentPiece.Position.y >= 20)
                {
                    isGameOver = true;
                    GameoverText.gameObject.SetActive(true);
                    break;
                }
            }

            RenderFigure();
            currentPiece = null;

            if (!isGameOver)
            {
                CheckField();
            }
        }

        lastTick = Time.time;
    }

    private bool CanMove(Vector2Int direction)
    {
        var position = currentPiece.Position;
        var points = currentPiece.Points;

        var canMove = true;

        for (int i = 0; i < points.Length; i++)
        {
            var checkPosition = points[i] + direction;
            if (points.Contains(checkPosition))
                continue;
            checkPosition += position;
            if (checkPosition.y < 0 || checkPosition.x < 0 || checkPosition.x > 9)
            {
                canMove = false;
                break;
            }

            if (checkPosition.y < 20)
            {
                if (!field[checkPosition.x][checkPosition.y].IsEmpty)
                {
                    canMove = false;
                    break;
                }
            }
        }

        return canMove;
    }

    private void CheckField()
    {
        if (CheckRaws() || CheckMatch3())
        {
            isPlayingAnimation = true;
        }

        ScoreText.text = score.ToString();
    }

    private bool CheckRaws()
    {
        bool isSmthComplete = false;

        for (int y = 0; y < 20; y++)
        {
            bool isComplete = true;
            for (int x = 0; x < 10; x++)
            {
                if (field[x][y].IsEmpty || (currentPiece != null &&
                                            currentPiece.Points.Contains(new Vector2Int(x, y) -
                                                                         currentPiece.Position)))
                {
                    isComplete = false;
                    break;
                }
            }

            if (isComplete)
            {
                score += 100;
                for (int x = 0; x < 10; x++)
                {
                    field[x][y].SetEmpty();
                    field[x][y].TetrisFallTarget = true;
                }

                isSmthComplete = true;
            }
        }

        return isSmthComplete;
    }

    private bool CheckMatch3()
    {
        bool isSmthComplete = false;

        List<Tile> tilesToClean = new List<Tile>();

        // check rows
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (!(IsBlock(x, y) && IsBlock(x + 1, y) && IsBlock(x + 2, y)))
                {
                    continue;
                }

                if (field[x][y].Color == field[x + 1][y].Color && field[x + 1][y].Color == field[x + 2][y].Color)
                {
                    score += 10;
                    tilesToClean.Add(field[x][y]);
                    tilesToClean.Add(field[x + 1][y]);
                    tilesToClean.Add(field[x + 2][y]);
                    field[x][y].Match3FallTarget = true;
                    field[x + 1][y].Match3FallTarget = true;
                    field[x + 2][y].Match3FallTarget = true;
                }
            }
        }

        // check cols
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                if (!(IsBlock(x, y) && IsBlock(x, y + 1) && IsBlock(x, y + 2)))
                {
                    continue;
                }

                if (field[x][y].Color == field[x][y + 1].Color && field[x][y + 1].Color == field[x][y + 2].Color)
                {
                    score += 1;
                    tilesToClean.Add(field[x][y]);
                    tilesToClean.Add(field[x][y + 1]);
                    tilesToClean.Add(field[x][y + 2]);
                    field[x][y].Match3FallTarget = true;
                    field[x][y + 1].Match3FallTarget = true;
                    field[x][y + 2].Match3FallTarget = true;
                }
            }
        }

        isSmthComplete = tilesToClean.Count > 0;

        foreach (var tile in tilesToClean)
        {
            tile.SetEmpty();
        }

        return isSmthComplete;
    }

    private bool IsBlock(int x, int y)
    {
        return !field[x][y].IsEmpty;
    }

    private void PlayAnimations()
    {
        if (TetrisFall())
        {
            lastTick = Time.time;
            return;
        }

        if (Match3Fall())
        {
            lastTick = Time.time;
        }
        else
        {
            foreach (var tile in tiles)
            {
                tile.TetrisFallTarget = false;
                tile.Match3FallTarget = false;
            }

            isPlayingAnimation = false;
        }
    }

    private bool TetrisFall()
    {
        bool isSmthFell = false;

        for (int i = 0; i < 20; i++)
        {
            if (field[0][i].TetrisFallTarget)
            {
                for (int y = i; y < 20 - 1; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        if (!field[x][y + 1].IsEmpty)
                        {
                            field[x][y].SetColor(field[x][y + 1].Color);
                            field[x][y + 1].SetEmpty();
                        }

                        field[x][y].TetrisFallTarget = field[x][y + 1].TetrisFallTarget;
                        field[x][y].Match3FallTarget = field[x][y + 1].Match3FallTarget;
                        field[x][y + 1].TetrisFallTarget = false;
                        field[x][y + 1].Match3FallTarget = false;
                    }
                }

                isSmthFell = true;
                break;
            }
        }

        return isSmthFell;
    }

    private bool Match3Fall()
    {
        bool isSmthFell = false;

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20 - 1; y++)
            {
                if (field[x][y].Match3FallTarget && field[x][y].IsEmpty && !field[x][y + 1].IsEmpty)
                {
                    field[x][y].SetColor(field[x][y + 1].Color);
                    field[x][y + 1].SetEmpty();
                    field[x][y + 1].Match3FallTarget = true;
                    isSmthFell = true;
                }
            }
        }

        return isSmthFell;
    }
}