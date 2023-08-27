using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    #region Public Variables

    public GameObject orangeNailPrefab;
    public GameObject blueNailPrefab;

    public List<Transform> nailBoardPositions;
    public List<Transform> blueNailsDefaultPositions;
    public List<Transform> orangeNailsDefaultPositions;

    #endregion

    #region Private Variables

    private List<GameObject> blueNails = new List<GameObject>();
    private List<GameObject> orangeNails = new List<GameObject>();

    private PositionState[,] boardState;

    #endregion

    private void ResetGame()
    {
        for(int i = 0; i < this.blueNails.Count; i++)
        {
            this.blueNails[i].transform.position = this.blueNailsDefaultPositions[i].position;
        }

        for(int i = 0; i < this.orangeNails.Count; i++)
        {
            this.orangeNails[i].transform.position = this.orangeNailsDefaultPositions[i].position;
        }

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                this.boardState[x, y] = PositionState.EMPTY;
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
            this.blueNails.Add(blueNail);
        }

        for(int i = 0; i < this.orangeNailsDefaultPositions.Count; i++)
        {
            var orangeNail = GameObject.Instantiate(this.orangeNailPrefab);
            orangeNail.transform.position = this.orangeNailsDefaultPositions[i].transform.position;
            this.orangeNails.Add(orangeNail);
        }
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
