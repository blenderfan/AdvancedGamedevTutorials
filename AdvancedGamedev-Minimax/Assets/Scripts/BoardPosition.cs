using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPosition : MonoBehaviour
{

    #region Public Variables

    public BoardPositionType positionType = BoardPositionType.BOTH;
    public Collider sphereCollider = null;

    public Vector2Int boardPosition = new Vector2Int(0, 0);

    #endregion
}
