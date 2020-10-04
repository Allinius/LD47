using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject target;
    public float enemySpawnInterval = 2.0f;
    public int enemiesToSpawnAtOnce = 1;
    public int totalEnemiesToSpawn = 20;
    public Color color;

    private float lastUpdate = 0.0f;
    private int enemiesSpawned = 0;
    private bool inactive = false;

    // Start is called before the first frame update
    void Start()
    {
        // color = new Color(
        //     Random.Range(0f,0.5f),
        //     Random.Range(0.7f, 1f),
        //     Random.Range(0.7f,1f)
        // );
        color = Random.ColorHSV(0.3f, 0.7f, 0.8f, 1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdate >= enemySpawnInterval && !inactive) {
            lastUpdate = Time.time;
            for (int i = 0; i < enemiesToSpawnAtOnce; i++) {
                if (enemiesSpawned >= totalEnemiesToSpawn) {
                    StartTargetDeath();
                    continue;
                }
                GameObject enemy = Instantiate(enemyPrefab, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.target = target.transform;
                enemyController.color = color;
                enemiesSpawned++;
            }
        }
    }

    void StartTargetDeath() {
        iTween.ScaleTo(target.gameObject, iTween.Hash(
            "scale", new Vector3(1f,1f,1f),
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutBack,
            "looptype", iTween.LoopType.pingPong
        ));
        iTween.ColorTo(target.gameObject, iTween.Hash(
            "color", Color.red,
            "time", 5f,
            "easetype", iTween.EaseType.easeOutQuint,
            "oncomplete", "DestroySelfAndTarget",
            "oncompletetarget", gameObject
        ));
    }

    void DestroySelfAndTarget() {
        Destroy(target.gameObject);
        Destroy(gameObject);
    }
}
