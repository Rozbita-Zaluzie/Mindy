using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public int level;
    public int radky;
    public int sloupce;
    public int colors;

    public LevelData(int level, int radky, int sloupce, int colors)
    {
        this.level = level;
        this.radky = radky;
        this.sloupce = sloupce;
        this.colors = colors;
    }
}
