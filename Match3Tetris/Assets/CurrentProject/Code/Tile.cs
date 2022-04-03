using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image spriteImage;
    [SerializeField] private Image match3Image;
    [SerializeField] private Image tetrisImage;

    public bool IsEmpty { get; private set; }
    public Colors Color { get; private set; }

    private bool _match3FallTarget;
    public bool Match3FallTarget
    {
        get
        {
            return _match3FallTarget;
        }
        set
        {
            if (_match3FallTarget != value)
            {
                _match3FallTarget = value;
                match3Image.gameObject.SetActive(_match3FallTarget);   
            }
        }
    }
    private bool _tetrisFallTarget;
    public bool TetrisFallTarget
    {
        get
        {
            return _tetrisFallTarget;
        }
        set
        {
            if (_tetrisFallTarget != value)
            {
                _tetrisFallTarget = value;
                tetrisImage.gameObject.SetActive(_tetrisFallTarget);   
            }
        }
    }

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
        spriteImage.gameObject.SetActive(!IsEmpty);
        switch (Color)
        {
            case Colors.Blue:
                spriteImage.color = ColorList.Blue;
                break;
            case Colors.Green:
                spriteImage.color = ColorList.Green;
                break;
            case Colors.Red:
                spriteImage.color = ColorList.Red;
                break;
            case Colors.Yellow:
                spriteImage.color = ColorList.Yellow;
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