using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform target;
    public float enemySpawnInterval = 2.0f;
    public int enemiesToSpawnAtOnce = 1;
    public int totalEnemiesToSpawn = 20;

    private float lastUpdate = 0.0f;
    private int enemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdate >= enemySpawnInterval) {
            lastUpdate = Time.time;
            for (int i = 0; i < enemiesToSpawnAtOnce; i++) {
                if (enemiesSpawned >= totalEnemiesToSpawn) {
                    DestroySelf();
                    continue;
                }
                GameObject enemy = Instantiate(enemyPrefab, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                enemy.GetComponent<EnemyController>().target = target;
                enemiesSpawned++;
            }
        }
    }

    void DestroySelf() {
        Destroy(target.gameObject);
        Destroy(gameObject);
    }
}
