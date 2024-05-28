using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public List<Waves> waves = new List<Waves>();
    public float spawnFireRate;
    float currentSpawnTime;
    int currentSpawnedObject;
    public float timeUntilBeginWave;
    public TextMeshProUGUI timeText;
    public GameObject boss;

    [SerializeField] private int currentWave;
    [SerializeField] private bool canSpawn;

    public Transform spawnPoint;

    private void Start() {
        Invoke("StartSpawn", timeUntilBeginWave);
    }

    void Update()
    {
        if (timeUntilBeginWave > 0) {
            timeUntilBeginWave -= Time.deltaTime;
            timeText.text = "Next Wave In: " + (int)timeUntilBeginWave;
        }
        if (timeUntilBeginWave < 0) timeUntilBeginWave = 0;

        if (canSpawn) {
            currentSpawnTime += Time.deltaTime;
            timeText.text = "Next Wave In: " + (int)(waves[currentWave].timeUntilNextWave - currentSpawnTime);
            if (currentSpawnTime >= spawnFireRate) {
                TrySpawn();
            }
        }
    }

    void TrySpawn() {
        currentSpawnTime = 0;
        if (currentSpawnedObject >= waves[currentWave].numberOfEnemies) {
            StopSpawn();
        } else {
            SpawnNewEnemy();
        }
    }

    void StartSpawn() {
        canSpawn = true;
    }

    void StopSpawn() {
        canSpawn = false;
        currentSpawnedObject = 0;
        currentSpawnTime = 0;
        NextWave();
    }

    void NextWave() {
        if (currentWave < waves.Count - 1) {
            currentWave++;
            StartCoroutine(WaitngForTheNextWave(waves[currentWave].timeUntilNextWave));
        } else {
            currentWave = 0;
            Debug.Log("Acabo");
            Invoke("SpawnBoss", 2f);
        }
    }

    IEnumerator WaitngForTheNextWave(float waitTime) {        
        yield return new WaitForSeconds(waitTime);
        StartSpawn();
    }

    void SpawnNewEnemy() {
        if (waves[currentWave].enemy_types.Count > 1) {
            int randomType = UnityEngine.Random.Range(0, waves[currentWave].enemy_types.Count);
            GameObject newObj = Instantiate(waves[currentWave].enemy_types[randomType], spawnPoint.position, Quaternion.identity);
            TryVelocity(newObj);
        } else {
            GameObject newObj = Instantiate(waves[currentWave].enemy_types[0], spawnPoint.position, Quaternion.identity);
            TryVelocity(newObj);
        }
        currentSpawnedObject++;
    }

    void TryVelocity(GameObject missile) {
        if(missile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) {
            rb.velocity = Vector2.right * 2;
        }
    }

    void SpawnBoss() {
        Instantiate(boss, spawnPoint.position, Quaternion.identity);        
    }
}

[Serializable]
public class Waves {
    public List<GameObject> enemy_types;
    public int numberOfEnemies;
    public int timeUntilNextWave;
}