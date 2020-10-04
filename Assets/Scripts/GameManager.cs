using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public Text scoreText;
    public GameObject enemySpawnerPrefab;
    public GameObject targetPrefab;
    public List<StageInfo> gameStages;

    public int score = 0;

    private Vector2 spawnExcludeDiameter = new Vector2(5f, 2f);
    private Vector2 spawnIncludeDiameter = new Vector2(8.5f, 4.5f);

    private List<GameObject> activeTargets;
    private int currentStageIndex = 0;
    private float lastSpawnerSpawned = -999f;
    private float gameStartTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        gameStartTime = Time.time;
        // Vector2 targetPos = RandomTargetPosition();
        // GameObject target = Instantiate(targetPrefab, targetPos, Quaternion.identity);
        // GameObject spawner = Instantiate(enemySpawnerPrefab, CalculateSpawnerPosition(targetPos), Quaternion.identity);
        // spawner.GetComponent<EnemySpawner>().target = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // advance to next stage if time is up
        StageInfo currentStage = gameStages[currentStageIndex];
        if (GameTime() >= currentStage.stageEndTime && currentStageIndex < gameStages.Count - 1) {
            currentStage = gameStages[++currentStageIndex];
            Debug.Log("New stage: " + currentStageIndex);
        }
        // spawn a spawner if possible
        if (GameTime() - lastSpawnerSpawned >= currentStage.spawnerSpawnInterval) {
            lastSpawnerSpawned = GameTime();
            Vector2 targetPos = RandomTargetPosition();
            GameObject target = Instantiate(targetPrefab, targetPos, Quaternion.identity);
            GameObject spawner = Instantiate(enemySpawnerPrefab, CalculateSpawnerPosition(targetPos), Quaternion.identity);
            EnemySpawner spawnerComponent = spawner.GetComponent<EnemySpawner>();
            spawnerComponent.target = target.transform;
            spawnerComponent.enemySpawnInterval = currentStage.enemySpawnInterval;
            spawnerComponent.enemiesToSpawnAtOnce = currentStage.enemiesToSpawnAtOnce;
            spawnerComponent.totalEnemiesToSpawn = currentStage.totalEnemiesToSpawn;            
        }
    }

    float GameTime() {
        return Time.time - gameStartTime;
    }

    Vector2 RandomTargetPosition() {
        while(true) {
            Vector2 vec = new Vector2(
                Random.Range(-spawnIncludeDiameter.x, spawnIncludeDiameter.x),
                Random.Range(-spawnIncludeDiameter.y, spawnIncludeDiameter.y)
            );
            if (!(vec.x > -spawnExcludeDiameter.x && vec.x < spawnExcludeDiameter.x &&
            vec.y > -spawnExcludeDiameter.y && vec.y < spawnExcludeDiameter.y)) {
                return vec;
            }
        }
        // Vector2 result = new Vector2();
        // result.x = RandomPointOnSides(spawnIncludeDiameter.x, spawnExcludeDiameter.x);
        // result.y = RandomPointOnSides(spawnIncludeDiameter.y, spawnExcludeDiameter.y);
        // return result;
    }

    float RandomPointOnSides(float includeRadius, float excludeRadius) {
        if (Random.value < 0.5f) {
            return Random.Range(-includeRadius, -excludeRadius);
        }
        return Random.Range(excludeRadius, includeRadius);
    }

    Vector2 CalculateSpawnerPosition(Vector2 targetPosition) {
        Vector2 targetQuartal = new Vector2(Mathf.Sign(targetPosition.x), Mathf.Sign(targetPosition.y));
        return new Vector2(
            Random.Range(-targetQuartal.x * spawnIncludeDiameter.x, -targetQuartal.x * (spawnIncludeDiameter.x + 3.0f)),
            Random.Range(-targetQuartal.y * spawnIncludeDiameter.y, -targetQuartal.y * (spawnIncludeDiameter.y + 3.0f))
        );
    }

    public void EndGame() {
        pauseMenu.PauseRestart();
    }

    public void EnemyKilled() {
        score++;
        scoreText.text = score.ToString();
    }
}
