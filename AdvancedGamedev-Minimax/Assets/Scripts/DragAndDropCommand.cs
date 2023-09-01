using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropCommand : MonoBehaviour
{

    private enum DragState
    {
        STARTED = 0,
        MOVED = 1,
        RELEASED = 2,
        ENDED = 3,
    }

    #region Public Variables

    public float dragStartMinDistance = 10.0f;
    public float dragHeight = 0.5f;

    #endregion

    #region Private Variables

    private bool snapped = false;

    private BoardPosition startPosition = null;
    private BoardPosition hitPosition = null;

    private DragState state = DragState.ENDED;

    private GameBoard board = null;

    private RotaNail currentNail = null;

    private Vector2 dragStart;

    #endregion

    public void Init(GameBoard board)
    {
        this.board = board;
    }

    public bool DragInProgress()
    {
        return this.state != DragState.ENDED;
    }

    public void StartDrag(BoardPosition startPosition, Vector2 screenPosition, RotaNail nail)
    {
        this.snapped = false;
        this.startPosition = startPosition;
        this.state = DragState.STARTED;
        this.dragStart = screenPosition;
        this.currentNail = nail;
    }

    private void CleanUpDrag()
    {
        this.currentNail = null;
    }

    public DragAndDropNailResult EndDrag()
    {
        var result = new DragAndDropNailResult()
        {
            nail = this.currentNail,
            success = false,
            startPosition = this.startPosition,
            targetPosition = this.hitPosition,
        };

        if(this.state != DragState.ENDED)
        {
            if(this.state == DragState.STARTED)
            {
                this.state = DragState.ENDED;
                this.CleanUpDrag();

            } else if(this.state == DragState.MOVED)
            {
                if(this.snapped)
                {
                    this.state = DragState.ENDED;
                    this.CleanUpDrag();

                    result.success = this.board.IsValidMove(result.startPosition, result.targetPosition);

                } else
                {
                    this.state = DragState.ENDED;
                    this.CleanUpDrag();
                }
            }
        }
        return result;
    }

    private void Update()
    {
        if(this.state == DragState.STARTED)
        {
            this.hitPosition = null;
            var currentTouchPosition = InputManager.Instance.GetTouchPosition();
            var dist = Vector2.Distance(this.dragStart, currentTouchPosition);

            if(dist > this.dragStartMinDistance)
            {
                this.state = DragState.MOVED;
            }
        } else if(this.state == DragState.MOVED)
        {
            var currentTouchPosition = InputManager.Instance.GetTouchPosition();
            var ray = CameraUtility.CreateCameraRay(Camera.main, currentTouchPosition);

            if(Physics.Raycast(ray, out var hitInfo, 300.0f, 1 << LayerMask.NameToLayer("Position")))
            {
                this.hitPosition = hitInfo.collider.GetComponentInParent<BoardPosition>();
                this.snapped = true;
                this.currentNail.transform.position = hitInfo.transform.position;
                
            } else
            {
                this.hitPosition = null;
                this.snapped = false;
                var plane = new Plane()
                {
                    normal = Vector3.up,
                    distance = -this.dragHeight
                };
                plane.Raycast(ray, out float distance);

                var pos = Camera.main.transform.position + ray.direction.normalized * distance;

                this.currentNail.transform.position = pos;
            }
        }
    }
}
