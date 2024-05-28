using UnityEngine;
public class DestroyObject : MonoBehaviour {
    public bool destroyOverLifetime;
    public float lifeTime;
    private float currentTime;
    public void DestroyTarget(GameObject target) => Destroy(target);
    public void DestroyMe() => Destroy(gameObject);
    public void DestroyMeWait(float wait) => Destroy(gameObject, wait);
    private void Update() {
        if (destroyOverLifetime) {
            currentTime += Time.deltaTime;
            if (currentTime > lifeTime) DestroyTarget(gameObject);
        }
    }
}