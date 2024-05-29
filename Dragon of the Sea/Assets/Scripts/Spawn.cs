using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Spawn : MonoBehaviour {
    public static Spawn instance;
    public enum Final { Boss, FimDaWave}
    public Final final = new();

    public List<Waves> waves = new List<Waves>();
    public float spawnFireRate;
    float currentSpawnTime;
    int currentSpawnedObject;
    public float timeUntilBeginWave;
    public TextMeshProUGUI timeText;
    public GameObject boss;
    public List<GameObject> enemires = new();
    public bool waitingForLastEnemy;
    private bool ending;

    [SerializeField] private int currentWave;
    [SerializeField] private bool canSpawn;

    public Transform spawnPoint;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        Invoke("StartSpawn", timeUntilBeginWave);
    }

    void Update() {
        if(waitingForLastEnemy) {
            if(enemires.Count == 0 && !ending) {
                ending = true;
                Player.instance.StopPlayer();
                VsnController.instance.StartVSN("GameWin");
            }
        }

        

        if (canSpawn) {
            currentSpawnTime += Time.deltaTime;
            //timeText.text = "Derrota todos os Invasores";
            //timeText.text = "Próxima Invasão em " + (waves[currentWave].timeUntilNextWave - currentSpawnTime).ToString("F0");
            if (currentSpawnTime >= spawnFireRate) {
                TrySpawn();
            }
        } else {
            if (timeUntilBeginWave > 0 && currentWave < waves.Count) {
                timeUntilBeginWave -= Time.deltaTime;
                timeText.text = "Próxima Invasão: " + timeUntilBeginWave.ToString("F0");
                
            } else timeText.text = "Derrota todos os Invasores";

            if (timeUntilBeginWave < 0) timeUntilBeginWave = 0;
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
        if (currentWave < waves.Count -1) {
            currentWave++;
            StartCoroutine(WaitngForTheNextWave(waves[currentWave].timeUntilNextWave));
        } else {
            currentWave = 0;
            Debug.Log("Acabo");
            if(final == Final.Boss) {
                Invoke("SpawnBoss", 2f);
            } else {
                waitingForLastEnemy = true;
            }
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
            enemires.Add(newObj);
        } else {
            GameObject newObj = Instantiate(waves[currentWave].enemy_types[0], spawnPoint.position, Quaternion.identity);
            TryVelocity(newObj);
            enemires.Add(newObj);
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

    public void RemoveEnemy(GameObject enemy) {
        if (enemires.Contains(enemy)) enemires.Remove(enemy);
        else Debug.Log("Inimigo não encontrado");
    }
}

[Serializable]
public class Waves {
    public List<GameObject> enemy_types;
    public int numberOfEnemies;
    public int timeUntilNextWave;
}