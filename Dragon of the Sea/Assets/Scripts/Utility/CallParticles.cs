using UnityEngine;

public class CallParticles : MonoBehaviour
{
    public void CallParticle(string particleName) {
        var newParticle = SpawnParticles.instance.newParticle(particleName);
        SpawnParticles.instance.SpawnParticle(newParticle, transform.position);
    }
}
