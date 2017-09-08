using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MatchesSet
{
    private List<GameObject> matchedCandies;

   
    /// Returns distinct list of matched candy
   
    public IEnumerable<GameObject> MatchedCandy
    {
        get
        {
            return matchedCandies.Distinct();
        }
    }

    public void AddObject(GameObject go)
    {
        if (!matchedCandies.Contains(go))
            matchedCandies.Add(go);
    }

    public void AddObjectRange(IEnumerable<GameObject> gos)
    {
        foreach (var item in gos)
        {
            AddObject(item);
        }
    }

    public MatchesSet()
    {
        matchedCandies = new List<GameObject>();
        BonusesContained = BonusType.None;
    }

    public BonusType BonusesContained { get; set; }
}

