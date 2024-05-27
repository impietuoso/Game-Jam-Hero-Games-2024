using UnityEngine;

public class CallParticles : MonoBehaviour
{
    public void CallParticle(string particleName) {
        var newParticle = SpawnParticles.instance.NewParticle(particleName);
        SpawnParticles.instance.SpawnParticle(newParticle, transform.position);
    }
}
