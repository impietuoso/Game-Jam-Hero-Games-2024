using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour {
    public bool fireStorm;
    public GameObject meteor;
    public GameObject indicator;
    public Transform fireStormSpawn;
    public Vector2 fireStormLimitSpawn;
    public int meteorCount;
    public float meteorFireRate;
    float mimX => fireStormSpawn.position.x - fireStormLimitSpawn.x;
    float maxX => fireStormSpawn.position.x + fireStormLimitSpawn.y;

    void Start()
    {
        Invoke("Fire", 3);
    }

    void Update()
    {
        if(fireStorm) {
            fireStorm = false;
            StartCoroutine(FireStorm());
        }
    }

    void Fire() {
        StartCoroutine(FireStorm());
    }

    public IEnumerator FireStorm() {
        for (int i = 0; i < meteorCount; i++) {
            var spawnPos = RandomPosition();
            var indicadorPos = new Vector2(spawnPos.x, transform.position.y);
            Instantiate(indicator, indicadorPos, Quaternion.identity);
            yield return new WaitForSeconds(meteorFireRate);
            Instantiate(meteor, spawnPos, Quaternion.identity);
        }
    }

    Vector2 RandomPosition() {
        var newX = new Vector2(Random.Range(mimX, maxX), fireStormSpawn.position.y);        
        return newX;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(fireStormSpawn != null) Gizmos.DrawWireCube(fireStormSpawn.position, new Vector3(maxX - mimX, 1, maxX - mimX));
    }

}