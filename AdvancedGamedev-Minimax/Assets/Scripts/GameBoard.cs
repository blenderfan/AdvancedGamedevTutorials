using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    #region Public Variables

    public DragAndDropCommand dragAndDrop;

    public GameObject orangeNailPrefab;
    public GameObject blueNailPrefab;

    public List<BoardPosition> nailBoardPositions;
    public List<BoardPosition> blueNailsDefaultPositions;
    public List<BoardPosition> orangeNailsDefaultPositions;

    #endregion

    #region Private Variables

    private Camera mainCamera = null;

    private Dictionary<Collider, RotaNail> colliderToNailMap = new Dictionary<Collider, RotaNail>();
    private Dictionary<RotaNail, BoardPosition> nailToPositionMap = new Dictionary<RotaNail, BoardPosition>();

    private int blueNailsPlaced = 0;
    private int orangeNailsPlaced = 0;

    private List<RotaNail> blueNails = new List<RotaNail>();
    private List<RotaNail> orangeNails = new List<RotaNail>();

    private PositionState[,] boardState;

    private PlayerColor currentColor = PlayerColor.BLUE;

    #endregion

    private void ResetGame()
    {
        this.blueNailsPlaced = 0;
        this.orangeNailsPlaced = 0;
        this.nailToPositionMap.Clear();

        for(int i = 0; i < this.blueNails.Count; i++)
        {
            this.blueNails[i].transform.position = this.blueNailsDefaultPositions[i].transform.position;
            this.nailToPositionMap.Add(this.blueNails[i], this.blueNailsDefaultPositions[i]);
        }

        for(int i = 0; i < this.orangeNails.Count; i++)
        {
            this.orangeNails[i].transform.position = this.orangeNailsDefaultPositions[i].transform.position;
            this.nailToPositionMap.Add(this.orangeNails[i], this.orangeNailsDefaultPositions[i]);
        }

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                this.boardState[x, y] = PositionState.EMPTY;
            }
        }
    }

    private void RegisterInputEvents()
    {
        InputManager.Instance.OnStartTouch += OnStartTouch;
        InputManager.Instance.OnEndTouch += OnEndTouch;
    }

    private void DeregisterInputEvents()
    {
        InputManager.Instance.OnStartTouch -= OnStartTouch;
        InputManager.Instance.OnEndTouch -= OnEndTouch;
    }

    public bool IsValidMove(BoardPosition startPosition, BoardPosition targetPosition)
    {
        if(targetPosition.positionType == BoardPositionType.BOTH)
        {
            var position = targetPosition.boardPosition;

            if (startPosition.positionType != BoardPositionType.BOTH)
            {
                return this.boardState[position.x, position.y] == PositionState.EMPTY;
            } else
            {
                var startBoardPosition = startPosition.boardPosition;
                int xDiff = Mathf.Abs(startBoardPosition.x - position.x);
                int yDiff = Mathf.Abs(startBoardPosition.y - position.y);

                if(xDiff == 1 || yDiff == 1)
                {
                    return this.boardState[position.x, position.y] == PositionState.EMPTY
                        && this.blueNailsPlaced == 3 && this.orangeNailsPlaced == 3;
                }
            } 

        }
        return false;
    }

    public void OnStartTouch(Vector2 position, float time)
    {
        if(!this.dragAndDrop.DragInProgress())
        {
            var ray = CameraUtility.CreateCameraRay(this.mainCamera, position);
  
            if(Physics.Raycast(ray, out var hitInfo, 300.0f, 1 << LayerMask.NameToLayer("Nail")))
            {
                var collider = hitInfo.collider;
                if(this.colliderToNailMap.ContainsKey(collider))
                {
                    var nail = this.colliderToNailMap[collider];
                    if(this.currentColor == nail.color)
                    {
                        this.dragAndDrop.StartDrag(this.nailToPositionMap[nail], position, nail);
                    }
                }
            }
            
        }
    }

    private void MakeMove(RotaNail nail, BoardPosition startPosition, BoardPosition targetPosition)
    {
        var targetBoardPosition = targetPosition.boardPosition;

        nail.transform.position = targetPosition.transform.position;

        if (startPosition.positionType == BoardPositionType.BOTH)
        {
            this.boardState[startPosition.boardPosition.x, startPosition.boardPosition.y] = PositionState.EMPTY;
        }
        this.boardState[targetBoardPosition.x, targetBoardPosition.y] = (PositionState)nail.color;

        if (startPosition.positionType == BoardPositionType.BLUE) this.blueNailsPlaced++;
        if (startPosition.positionType == BoardPositionType.ORANGE) this.orangeNailsPlaced++;

        this.nailToPositionMap.Remove(nail);
        this.nailToPositionMap.Add(nail, targetPosition);

        

        bool hasWon = this.CheckIfPlayerWon(this.currentColor);

        this.currentColor = this.currentColor == PlayerColor.BLUE ? PlayerColor.ORANGE : PlayerColor.BLUE;

        Debug.Log(hasWon);
    }

    public void OnEndTouch(Vector2 position, float time)
    {
        var dragResult = this.dragAndDrop.EndDrag();
        if (dragResult.nail != null)
        {
            if (dragResult.success)
            {

                this.MakeMove(dragResult.nail, dragResult.startPosition, dragResult.targetPosition);

            }
            else
            {
                var startPosition = dragResult.startPosition;
                dragResult.nail.transform.position = startPosition.transform.position;
            }
        }
    }

    private void Start()
    {
        this.boardState = new PositionState[3, 3];

        for(int i = 0; i < this.blueNailsDefaultPositions.Count; i++)
        {
            var blueNail = GameObject.Instantiate(this.blueNailPrefab);
            blueNail.transform.position = this.blueNailsDefaultPositions[i].transform.position;
            var nail = blueNail.GetComponentInChildren<RotaNail>();
            this.blueNails.Add(nail);

            this.colliderToNailMap.Add(nail.capsuleCollider, nail);
            this.nailToPositionMap.Add(nail, this.blueNailsDefaultPositions[i]);
        }

        for(int i = 0; i < this.orangeNailsDefaultPositions.Count; i++)
        {
            var orangeNail = GameObject.Instantiate(this.orangeNailPrefab);
            orangeNail.transform.position = this.orangeNailsDefaultPositions[i].transform.position;
            var nail = orangeNail.GetComponentInChildren<RotaNail>();
            this.orangeNails.Add(nail);

            this.colliderToNailMap.Add(nail.capsuleCollider, nail);
            this.nailToPositionMap.Add(nail, this.orangeNailsDefaultPositions[i]);
        }

        this.dragAndDrop.Init(this);
        this.mainCamera = Camera.main;
        this.RegisterInputEvents();
    }

    private bool CheckLine(PlayerColor color, int x0, int y0, int x1, int y1, int x2, int y2)
    {
        if ((int)this.boardState[x0, y0] == (int)color
            && (int)this.boardState[x1, y1] == (int)color
            && (int)this.boardState[x2, y2] == (int)color)
        {
            return true;
        }
        return false;
    }

    private bool CheckIfPlayerWon(PlayerColor color)
    {
        bool playerHasWon = false;

        //Horizontal
        playerHasWon |= CheckLine(color,
            0, 0,
            1, 0,
            2, 0);
        playerHasWon |= CheckLine(color,
            0, 1,
            1, 1,
            2, 1);
        playerHasWon |= CheckLine(color,
            0, 2,
            1, 2,
            2, 2);

        //Vertical
        playerHasWon |= CheckLine(color,
            0, 0,
            0, 1,
            0, 2);

        playerHasWon |= CheckLine(color,
            1, 0,
            1, 1,
            1, 2);

        playerHasWon |= CheckLine(color,
            2, 0,
            2, 1,
            2, 2);

        //Diagonals
        playerHasWon |= CheckLine(color,
            0, 0,
            1, 1,
            2, 2);

        playerHasWon |= CheckLine(color,
            2, 0,
            1, 1,
            0, 2);

        return playerHasWon;
    }

    public GameOutcome CheckGameOutcome()
    {
        if (CheckIfPlayerWon(PlayerColor.BLUE)) return GameOutcome.BLUE_WINS;
        else if (CheckIfPlayerWon(PlayerColor.ORANGE)) return GameOutcome.ORANGE_WINS;
        else return GameOutcome.ONGOING;
    }

}