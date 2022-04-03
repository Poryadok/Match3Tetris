using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Tile[] tiles;
    [SerializeField] private Vector2Int spawnPosition;

    public float TickDelay;

    private int score;
    private bool isGameOver;

    private Tile[][] field;

    private float lastTick;

    private Figure currentFigure;

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
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        PreMathRenderClear();

        if (currentFigure != null)
        {
            if (Input.GetButtonDown("Right"))
            {
                
            }
        }
        
        if (currentFigure == null)
        {
            currentFigure = new Figure()
                {Points = FigureList.Figures[Random.Range(0, FigureList.Figures.Length)], Position = spawnPosition};
            currentFigure.RandomizeColors(4);
            lastTick = Time.time;
        }

        if (Time.time > lastTick + TickDelay)
        {
            FigureDrop();
        }
        
        CheckField();

        if (currentFigure != null)
        {
            RenderFigure();   
        }
    }

    private void PreMathRenderClear()
    {
        if (currentFigure == null)
        {
            return;
        }

        var position = currentFigure.Position;
        for (int i = 0; i < currentFigure.Points.Length; i++)
        {
            var pos = position + currentFigure.Points[i];
            if (pos.x < 10 && pos.y < 20)
            {
                field[pos.x][pos.y].SetEmpty();
            }
        }
    }

    private void RenderFigure()
    {
        var position = currentFigure.Position;
        for (int i = 0; i < currentFigure.Points.Length; i++)
        {
            var pos = position + currentFigure.Points[i];
            if (pos.x < 10 && pos.y < 20)
            {
                field[pos.x][pos.y].SetColor(currentFigure.Colors[i]);
            }
        }
    }

    private void FigureDrop()
    {
        if (CanDrop())
        {
            currentFigure.Position += Vector2Int.down;
        }

        if (!CanDrop())
        {
            RenderFigure();
            currentFigure = null;
        }

        lastTick = Time.time;
    }

    private bool CanDrop()
    {
        
        
        var position = currentFigure.Position;
        var points = currentFigure.Points;

        var canDrop = true;

        for (int i = 0; i < points.Length; i++)
        {
            var checkPosition = points[i] + Vector2Int.down;
            if (points.Contains(checkPosition))
                continue;
            checkPosition += position;
            if (checkPosition.y < 0)
            {
                canDrop = false;
                break;
            }

            if (!field[checkPosition.x][checkPosition.y].IsEmpty)
            {
                canDrop = false;
                break;
            }
        }

        return canDrop;
    }

    private void CheckField()
    {
        CheckRaws();
        CheckMatch3();
    }

    private void CheckRaws()
    {
        for (int y = 0; y < 20; y++)
        {
            bool isComplete = true;
            for (int x = 0; x < 10; x++)
            {
                if (field[x][y].IsEmpty || (currentFigure != null &&
                                            currentFigure.Points.Contains(new Vector2Int(x, y) -
                                                                          currentFigure.Position)))
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
                }
            }
        }
    }

    private void CheckMatch3()
    {
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
                    score += 1;
                    tilesToClean.Add(field[x][y]);
                    tilesToClean.Add(field[x + 1][y]);
                    tilesToClean.Add(field[x + 2][y]);
                }
            }
        }

        // check cols
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                if (!(IsBlock(x, y) && IsBlock(x, y + 2) && IsBlock(x, y + 2)))
                {
                    continue;
                }

                if (field[x][y].Color == field[x][y + 1].Color && field[x][y + 1].Color == field[x][y + 2].Color)
                {
                    score += 1;
                    tilesToClean.Add(field[x][y]);
                    tilesToClean.Add(field[x][y + 1]);
                    tilesToClean.Add(field[x][y + 2]);
                }
            }
        }

        foreach (var tile in tilesToClean)
        {
            tile.SetEmpty();
        }
    }

    private bool IsBlock(int x, int y)
    {
        return !field[x][y].IsEmpty;
    }
}