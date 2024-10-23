using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomeSetActive : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        StartCoroutine(PlayParticleLoop());
    }

    private IEnumerator PlayParticleLoop()
    {
        while (true)
        {
            float delay = Random.Range(0f, 1f);
            particle.Play();
            yield return new WaitForSeconds(delay);
            particle.Stop();
        }
    }
}
