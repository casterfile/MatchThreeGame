using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// Bonus types
[Flags]
public enum BonusType
{
    None,
    DestroyWholeRowColumn
}


public static class BonusTypeUtilities
{
   
    /// Helper method to check for specific bonus type
    public static bool ContainsDestroyWholeRowColumn(BonusType bt)
    {
        return (bt & BonusType.DestroyWholeRowColumn) 
            == BonusType.DestroyWholeRowColumn;
    }
}

/// Our simple game state
public enum GameState
{
    None,
    SelectionStarted,
    Animating
}
