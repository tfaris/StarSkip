﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PredefinedStuff : ScriptableObject
{
    public int gridX, gridZ;
    [Tooltip("X and Z position within the grid space (0 = bottom left, 1 = top right)")]
    [Range(0, 1)]
    public float xWithinGrid, zWithinGrid;
    public GameObject objectToSpawn;
}
