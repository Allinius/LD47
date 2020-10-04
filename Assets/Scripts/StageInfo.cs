using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageInfo 
{
    public float stageEndTime;
    public int enemiesToSpawnAtOnce;
    public int totalEnemiesToSpawn;
    public float enemySpawnInterval;
    public float spawnerSpawnInterval;
}
