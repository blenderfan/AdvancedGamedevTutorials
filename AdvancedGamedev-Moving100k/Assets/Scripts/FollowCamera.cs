using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    #region Public Variables

    public GameObject target = null;

    #endregion

    private void Update()
    {
        if (this.target != null)
        {
            this.transform.rotation = Quaternion.LookRotation(this.target.transform.position - this.transform.position);
        }
    }
}
