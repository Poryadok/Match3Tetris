using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image image;

    public bool IsEmpty { get; private set; }
    public Colors Color { get; private set; }

    public void SetEmpty()
    {
        IsEmpty = true;
        UpdateSprite();
    }

    public void SetColor(Colors color)
    {
        IsEmpty = false;
        Color = color;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        image.gameObject.SetActive(!IsEmpty);
        switch (Color)
        {
            case Colors.Blue:
                image.color = ColorList.Blue;
                break;
            case Colors.Green:
                image.color = ColorList.Green;
                break;
            case Colors.Red:
                image.color = ColorList.Red;
                break;
            case Colors.Yellow:
                image.color = ColorList.Yellow;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum Colors
{
    Blue,
    Green,
    Red,
    Yellow
}

public static class ColorList
{
    public static Color Blue = Color.blue;
    public static Color Green = Color.green;
    public static Color Red = Color.red;
    public static Color Yellow = Color.yellow;
}