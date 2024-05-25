using System.Collections;

using UnityEngine;

public class WaterDropsGenerator : MonoBehaviour
{
    public float spawnFireRate;
    public bool spawn;
    public GameObject waterDrop;
    public float heightPosition;
    public Vector2 randomXPosition;
    public Vector2 spawnPosition;

    Vector3 mimSpawnPosition()=> new Vector3(transform.position.x + randomXPosition.x, transform.position.y + heightPosition, 0);
    Vector3 maxSpawnPosition() => new Vector3(transform.position.x + randomXPosition.y, transform.position.y + heightPosition, 0);

    public void StartSpawn() {
        spawn = true;
        StartCoroutine(Spawn());
    }

    public void StopSpawn() {
        spawn = false;
        StopCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        spawnPosition.x = Random.Range(randomXPosition.x + transform.position.x, randomXPosition.y + transform.position.x);
        spawnPosition.y = transform.position.y + heightPosition;
        var drop = Instantiate(waterDrop, spawnPosition, Quaternion.identity);
        drop.GetComponent<Follower>().target = transform;
        yield return new WaitForSeconds(spawnFireRate);
        if(spawn) StartCoroutine(Spawn());
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;        
        Gizmos.DrawLine(mimSpawnPosition(), maxSpawnPosition());
    }
}
