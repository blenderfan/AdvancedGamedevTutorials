using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float explostionTime = 2.0f;

    public ParticleSystem explosion;

    private bool isExploding = false;

    void Start()
    {
        
    }

    private IEnumerator Explode()
    {
        this.isExploding = true;

        this.explosion.Play();

        var mr = this.GetComponentInChildren<MeshRenderer>();
        mr.enabled = false;

        yield return new WaitForSeconds(this.explostionTime);

        GameObject.Destroy(this.gameObject);

        this.isExploding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!this.isExploding)
        {
            this.StartCoroutine(this.Explode());

        }
    }
}
