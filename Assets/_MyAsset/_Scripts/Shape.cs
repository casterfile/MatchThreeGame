using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public BonusType Bonus { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }

    public string Type { get; set; }

    public Shape()
    {
        Bonus = BonusType.None;
    }

   
    /// Checks if the current shape is of the same type as the parameter
    public bool IsSameType(Shape otherShape)
    {
        if (otherShape == null || !(otherShape is Shape))
            throw new ArgumentException("otherShape");

        return string.Compare(this.Type, (otherShape as Shape).Type) == 0;
    }

   
    /// Constructor alternative
    public void Assign(string type, int row, int column)
    {

        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("type");

        Column = column;
        Row = row;
        Type = type;
    }

   
    /// Swaps properties of the two shapes
    /// We could do a shallow copy/exchange here, but anyway...
    public static void SwapColumnRow(Shape a, Shape b)
    {
        int temp = a.Row;
        a.Row = b.Row;
        b.Row = temp;

        temp = a.Column;
        a.Column = b.Column;
        b.Column = temp;
    }
}



