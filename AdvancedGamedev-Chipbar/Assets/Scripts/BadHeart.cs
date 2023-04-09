using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadHeart : MonoBehaviour
{

    public float healthLost = 10.0f;

    public float spinSpeed = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInChildren<Player>();
        if(player != null)
        {
            player.RemoveHealth(this.healthLost);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        this.transform.Rotate(new Vector3(0.0f, this.spinSpeed * Time.deltaTime, 0.0f));
    }

}
