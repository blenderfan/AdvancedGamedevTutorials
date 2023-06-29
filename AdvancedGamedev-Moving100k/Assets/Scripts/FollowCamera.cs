using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    #region Public Variables

    public float followSpeed = 5.0f;

    public GameObject target = null;

    #endregion

    private void Update()
    {
        if (this.target != null)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(this.target.transform.position - this.transform.position), this.followSpeed * Time.deltaTime);
        }
    }
}
