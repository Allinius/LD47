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

    public GameObject audioSourcePrefab;
    public AudioClip popAudio;
    public AudioClip whistleAudio;

    public int score = 0;

    private Vector2 spawnExcludeDiameter = new Vector2(5f, 2f);
    private Vector2 spawnIncludeDiameter = new Vector2(8.5f, 4.5f);

    private List<GameObject> activeTargets;
    private int currentStageIndex = 0;
    private float lastSpawnerSpawned = -Mathf.Infinity;
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
            lastSpawnerSpawned = -Mathf.Infinity;
        }
        // spawn a spawner if possible
        if (GameTime() - lastSpawnerSpawned >= currentStage.spawnerSpawnInterval) {
            lastSpawnerSpawned = GameTime();
            Vector2 targetPos = RandomTargetPosition();
            Color color = Random.ColorHSV(0.2f, 0.8f, 0.8f, 1f, 1f, 1f);
            GameObject target = Instantiate(targetPrefab, targetPos, Quaternion.identity);
            target.GetComponentInChildren<SpriteRenderer>().color = color;
            iTween.RotateBy(target, iTween.Hash(
                "amount", new Vector3(0f,0f,1f),
                "time", 2f,
                "looptype", iTween.LoopType.loop,
                "easetype", iTween.EaseType.linear)
            );
            GameObject spawner = Instantiate(enemySpawnerPrefab, CalculateSpawnerPosition(targetPos), Quaternion.identity);
            EnemySpawner spawnerComponent = spawner.GetComponent<EnemySpawner>();
            spawnerComponent.target = target;
            spawnerComponent.enemySpawnInterval = currentStage.enemySpawnInterval;
            spawnerComponent.enemiesToSpawnAtOnce = currentStage.enemiesToSpawnAtOnce;
            spawnerComponent.totalEnemiesToSpawn = currentStage.totalEnemiesToSpawn;
            spawnerComponent.color = color;

        }
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

    public float GameTime() {
        return Time.time - gameStartTime;
    }

    public void EndGame() {
        pauseMenu.PauseRestart();
    }

    public void EnemyKilled(bool angry) {
        if (!angry) {
            score++;
            scoreText.text = score.ToString();
        }
        PlayAudio("pop");
    }

    public void PlayAudio(string clipName) {
        AudioClip audioClip = null;
        if (clipName == "pop") {
            audioClip = popAudio;
        } else if (clipName == "whistle") {
            audioClip = whistleAudio;
        }
        GameObject audioSourceInstance = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
        AudioSource audioSourceComponent = audioSourceInstance.GetComponent<AudioSource>();
        audioSourceComponent.clip = audioClip;
        audioSourceComponent.Play();
        iTween.ScaleTo(audioSourceInstance, iTween.Hash(
            "scale", new Vector3(),
            "time", 2f,
            "oncomplete", "DestroyOnComplete",
            "oncompletetarget", gameObject,
            "oncompleteparams", audioSourceInstance
        ));
    }

    void DestroyOnComplete(GameObject obj) {
        Destroy(obj);
    }


}
