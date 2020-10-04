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
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdate >= enemySpawnInterval && !inactive) {
            lastUpdate = Time.time;
            for (int i = 0; i < enemiesToSpawnAtOnce; i++) {
                if (enemiesSpawned >= totalEnemiesToSpawn) {
                    StartTargetDeath(10f);
                    continue;
                }
                GameObject enemy = Instantiate(enemyPrefab, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.spawner = this;
                enemyController.target = target.transform;
                enemyController.color = color;
                enemiesSpawned++;
            }
        }
    }

    public void StartTargetDeath(float seconds) {
        GameObject targetImage = target.transform.Find("Image").gameObject;
        SpriteRenderer targetImageRenderer = targetImage.GetComponent<SpriteRenderer>();
        iTween.ScaleTo(targetImage, iTween.Hash(
            "scale", new Vector3(0.02f,0.02f,1f),
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutBack,
            "looptype", iTween.LoopType.pingPong
        ));
        iTween.ColorTo(targetImage, iTween.Hash(
            "color", Color.white,
            "time", seconds,
            "easetype", iTween.EaseType.easeOutQuint,
            "includechildren", true,
            "oncomplete", "DestroySelfAndTarget",
            "oncompletetarget", gameObject
        ));
    }

    void DestroySelfAndTarget() {
        Destroy(target.gameObject);
        Destroy(gameObject);
    }
}
