using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{

    public float healthGained = 10.0f;

    public float spinSpeed = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInChildren<Player>();
        if(player != null)
        {
            player.AddHealth(this.healthGained);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        this.transform.Rotate(new Vector3(0.0f, this.spinSpeed * Time.deltaTime, 0.0f));
    }

}
