using System;
using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
    public static SpawnParticles instance;
    public Particle[] particles;

    private void Awake() {        
        instance = this;
    }

    public Particle NewParticle(string particleName) {
        var selected = particles[0];
        foreach (var item in particles) {
            if (item.particleName == particleName) selected = item;
        }
        return selected;
    }

    public void SpawnParticle(string particle) {
        Instantiate(NewParticle(particle).prefab, transform.position, Quaternion.identity);
    }

    public void SpawnParticle(Particle particle, Vector2 position) {
        Instantiate(particle.prefab, position, Quaternion.identity);
    }

    public void SpawnParticle(string particle, Vector2 position) {
        Instantiate(NewParticle(particle).prefab, position, Quaternion.identity);
    }

    public void SpawnParticle(Particle particle, Vector2 position, float scaleX) {
        var newParticle = Instantiate(particle.prefab, position, Quaternion.identity);
        newParticle.transform.localScale = new Vector2(newParticle.transform.localScale.x * scaleX, newParticle.transform.localScale.y);
    }

    public void SpawnParticle(string particle, Vector2 position, float scaleX) {
        var newParticle = Instantiate(NewParticle(particle).prefab, position, Quaternion.identity);
        newParticle.transform.localScale = new Vector2(newParticle.transform.localScale.x * scaleX, newParticle.transform.localScale.y);
    }
}

[Serializable]
public class Particle {
    public string particleName;
    public GameObject prefab;
}