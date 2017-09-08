using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// Custom class to accomodate useful stuff for our shapes array
public class ShapesArray
{

    private GameObject[,] shapes = new GameObject[ConstantsVariable.Rows, ConstantsVariable.Columns];

   
    /// Indexer
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return shapes[row, column];
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        set
        {
            shapes[row, column] = value;
        }
    }

   
    /// Swaps the position of two items, also keeping a backup
    public void Swap(GameObject g1, GameObject g2)
    {
        //hold a backup in case no match is produced
        backupG1 = g1;
        backupG2 = g2;

        var g1Shape = g1.GetComponent<Shape>();
        var g2Shape = g2.GetComponent<Shape>();

        //get array indexes
        int g1Row = g1Shape.Row;
        int g1Column = g1Shape.Column;
        int g2Row = g2Shape.Row;
        int g2Column = g2Shape.Column;

        //swap them in the array
        var temp = shapes[g1Row, g1Column];
        shapes[g1Row, g1Column] = shapes[g2Row, g2Column];
        shapes[g2Row, g2Column] = temp;

        //swap their respective properties
        Shape.SwapColumnRow(g1Shape, g2Shape);

    }

   
    /// Undoes the swap
    public void UndoSwap()
    {
        if (backupG1 == null || backupG2 == null)
            throw new Exception("Backup is null");

        Swap(backupG1, backupG2);
    }

    private GameObject backupG1;
    private GameObject backupG2;


   

   
    /// Returns the matches found for a list of GameObjects
    /// MatchesSet class is not used as this method is called on subsequent collapses/checks, 
    /// not the one inflicted by user's drag
    public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
    {
        List<GameObject> matches = new List<GameObject>();
        foreach (var go in gos)
        {
            matches.AddRange(GetMatches(go).MatchedCandy);
        }
        return matches.Distinct();
    }

   
    /// Returns the matches found for a single GameObject
    public MatchesSet GetMatches(GameObject go)
    {
        MatchesSet MatchesSet = new MatchesSet();

        var horizontalMatches = GetMatchesHorizontally(go);
        if (ContainsDestroyRowColumnBonus(horizontalMatches))
        {
            horizontalMatches = GetEntireRow(go);
            if (!BonusTypeUtilities.ContainsDestroyWholeRowColumn(MatchesSet.BonusesContained))
                MatchesSet.BonusesContained |= BonusType.DestroyWholeRowColumn;
        }
        MatchesSet.AddObjectRange(horizontalMatches);

        var verticalMatches = GetMatchesVertically(go);
        if (ContainsDestroyRowColumnBonus(verticalMatches))
        {
            verticalMatches = GetEntireColumn(go);
            if (!BonusTypeUtilities.ContainsDestroyWholeRowColumn(MatchesSet.BonusesContained))
                MatchesSet.BonusesContained |= BonusType.DestroyWholeRowColumn;
        }
        MatchesSet.AddObjectRange(verticalMatches);

        return MatchesSet;
    }

    private bool ContainsDestroyRowColumnBonus(IEnumerable<GameObject> matches)
    {
        if (matches.Count() >= ConstantsVariable.MinimumMatches)
        {
            foreach (var go in matches)
            {
                if (BonusTypeUtilities.ContainsDestroyWholeRowColumn
                    (go.GetComponent<Shape>().Bonus))
                    return true;
            }
        }

        return false;
    }

    private IEnumerable<GameObject> GetEntireRow(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int row = go.GetComponent<Shape>().Row;
        for (int column = 0; column < ConstantsVariable.Columns; column++)
        {
            matches.Add(shapes[row, column]);
        }
        return matches;
    }

    private IEnumerable<GameObject> GetEntireColumn(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        int column = go.GetComponent<Shape>().Column;
        for (int row = 0; row < ConstantsVariable.Rows; row++)
        {
            matches.Add(shapes[row, column]);
        }
        return matches;
    }

   
    /// Searches horizontally for matches
    private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check left
        if (shape.Column != 0)
            for (int column = shape.Column - 1; column >= 0; column--)
            {
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        //check right
        if (shape.Column != ConstantsVariable.Columns - 1)
            for (int column = shape.Column + 1; column < ConstantsVariable.Columns; column++)
            {
                if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[shape.Row, column]);
                }
                else
                    break;
            }

        //we want more than three matches
        if (matches.Count < ConstantsVariable.MinimumMatches)
            matches.Clear();

        return matches.Distinct();
    }

   
    /// Searches vertically for matches
    private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
    {
        List<GameObject> matches = new List<GameObject>();
        matches.Add(go);
        var shape = go.GetComponent<Shape>();
        //check bottom
        if (shape.Row != 0)
            for (int row = shape.Row - 1; row >= 0; row--)
            {
                if (shapes[row, shape.Column] != null &&
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }

        //check top
        if (shape.Row != ConstantsVariable.Rows - 1)
            for (int row = shape.Row + 1; row < ConstantsVariable.Rows; row++)
            {
                if (shapes[row, shape.Column] != null && 
                    shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                {
                    matches.Add(shapes[row, shape.Column]);
                }
                else
                    break;
            }


        if (matches.Count < ConstantsVariable.MinimumMatches)
            matches.Clear();

        return matches.Distinct();
    }

   
    /// Removes (sets as null) an item from the array
    public void Remove(GameObject item)
    {
        shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] = null;
    }

   
    /// Collapses the array on the specific columns, after checking for empty items on them
    public DistinctCandyCreator Collapse(IEnumerable<int> columns)
    {
        DistinctCandyCreator collapseInfo = new DistinctCandyCreator();


        ///search in every column
        foreach (var column in columns)
        {
            //begin from bottom row
            for (int row = 0; row < ConstantsVariable.Rows - 1; row++)
            {
                //if you find a null item
                if (shapes[row, column] == null)
                {
                    //start searching for the first non-null
                    for (int row2 = row + 1; row2 < ConstantsVariable.Rows; row2++)
                    {
                        //if you find one, bring it down (i.e. replace it with the null you found)
                        if (shapes[row2, column] != null)
                        {
                            shapes[row, column] = shapes[row2, column];
                            shapes[row2, column] = null;

                            //calculate the biggest distance
                            if (row2 - row > collapseInfo.MaxDistance) 
                                collapseInfo.MaxDistance = row2 - row;

                            //assign new row and column (name does not change)
                            shapes[row, column].GetComponent<Shape>().Row = row;
                            shapes[row, column].GetComponent<Shape>().Column = column;

                            collapseInfo.AddCandy(shapes[row, column]);
                            break;
                        }
                    }
                }
            }
        }

        return collapseInfo;
    }

   
    /// Searches the specific column and returns info about null items
    public IEnumerable<ShapeSet> GetEmptyItemsOnColumn(int column)
    {
        List<ShapeSet> emptyItems = new List<ShapeSet>();
        for (int row = 0; row < ConstantsVariable.Rows; row++)
        {
            if (shapes[row, column] == null)
                emptyItems.Add(new ShapeSet() { Row = row, Column = column });
        }
        return emptyItems;
    }
}

