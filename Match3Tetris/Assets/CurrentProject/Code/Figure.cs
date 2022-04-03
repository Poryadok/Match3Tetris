using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure
{
    public Vector2Int Position;
    public Vector2Int[] Points;
    public Colors[] Colors = new Colors[4];

    public void RandomizeColors(int number)
    {
        for (int i = 0; i < 4; i++)
        {
            Colors[i] = (Colors) Random.Range(0, number);
        }
    }

    public Vector2Int[] RotateRight()
    {
        var result = new Vector2Int[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = RotateVector(Points[i], 90);
        }

        return result;
    }

    private Vector2Int RotateVector(Vector2Int v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        int x = Mathf.RoundToInt(v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian));
        int y = Mathf.RoundToInt(v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian));
        return new Vector2Int(x, y);
    }
}

public static class FigureList
{
    public static Vector2Int[][] Figures = new[]
    {
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.up, new Vector2Int(-1, 1)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.up, new Vector2Int(0, 2)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.up, new Vector2Int(1, 1)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.right, new Vector2Int(1, 1)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.right, new Vector2Int(2, 0)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.right, new Vector2Int(1, -1)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.down, new Vector2Int(1, -1)},
        new[] {Vector2Int.left, Vector2Int.zero, Vector2Int.up, Vector2Int.right},
    };
}