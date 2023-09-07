using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct BoardState : IDisposable
{
    #region Public Variables

    public PlayerColor currentColor;

    public NativeArray<PositionState> boardState;

    public int uncommittedBlue;
    public int uncommittedOrange;

    #endregion


    public BoardState(PlayerColor color, int uncommittedBlue, int uncommittedOrange)
    {
        this.currentColor = color;
        this.boardState = new NativeArray<PositionState>(9, Allocator.Persistent);

        this.uncommittedBlue = uncommittedBlue;
        this.uncommittedOrange = uncommittedOrange;
    }

    private void FindMovesForPosition(ref NativeList<Move> moves, int x, int y)
    {
        var startPosition = new Vector2Int(x, y);
        var startPositionType = BoardPositionType.BOTH;

        for(int i = 0; i < this.boardState.Length; i++)
        {
            var state = this.boardState[i];

            if (state == PositionState.EMPTY)
            {
                int endX = x % 3;
                int endY = y / 3;

                int diffX = Mathf.Abs(x - endX);
                int diffY = Mathf.Abs(y - endY);

                if (diffX == 1 ^ diffY == 1)
                {
                    moves.Add(new Move()
                    {
                        start = startPosition,
                        startPositionType = startPositionType,
                        end = new Vector2Int(endX, endY),
                    });
                }
            }
        }
    }

    public void MakeMove(Move move)
    {

        int startIdx = move.start.y * 3 + move.start.x;
        int endIdx = move.end.y * 3 + move.end.x;

        if(this.currentColor == PlayerColor.BLUE && this.uncommittedBlue > 0)
        {
            this.uncommittedBlue--;
            this.boardState[endIdx] = PositionState.BLUE;
        } else if(this.currentColor == PlayerColor.ORANGE && this.uncommittedOrange > 0)
        {
            this.uncommittedOrange--;
            this.boardState[endIdx] = PositionState.ORANGE;
        }
        else
        {
            this.boardState[startIdx] = PositionState.EMPTY;
            this.boardState[endIdx] = this.currentColor == PlayerColor.BLUE ? PositionState.BLUE : PositionState.ORANGE;
        }
    }

    public void GetMoves(ref NativeList<Move> moves)
    {


        for(int i = 0; i < this.boardState.Length; i++)
        {
            var positionState = this.boardState[i];
            int x = i % 3;
            int y = i / 3;

            switch(positionState)
            {
                case PositionState.BLUE:
                    if(this.currentColor == PlayerColor.BLUE && this.uncommittedBlue == 0)
                    {
                        this.FindMovesForPosition(ref moves, x, y);
                    }
                    break;
                case PositionState.ORANGE:
                    if(this.currentColor == PlayerColor.ORANGE && this.uncommittedOrange == 0)
                    {
                        this.FindMovesForPosition(ref moves, x, y);
                    }
                    break;
                case PositionState.EMPTY:
                    if(this.currentColor == PlayerColor.BLUE && this.uncommittedBlue > 0)
                    {
                        moves.Add(new Move()
                        {
                            startPositionType = BoardPositionType.BLUE,
                            start = Vector2Int.zero,
                            end = new Vector2Int(x, y)
                        });
                    }

                    if(this.currentColor == PlayerColor.ORANGE && this.uncommittedOrange > 0)
                    {
                        moves.Add(new Move()
                        {
                            startPositionType = BoardPositionType.ORANGE,
                            start = Vector2Int.zero,
                            end = new Vector2Int(x, y)
                        });
                    }

                    break;
            }
        }
    }

    private bool CheckLine(PlayerColor color, int x0, int y0, int x1, int y1, int x2, int y2)
    {
        int idx0 = y0 * 3 + x0;
        int idx1 = y1 * 3 + x1;
        int idx2 = y2 * 3 + x2;

        if ((int)this.boardState[idx0] == (int)color
            && (int)this.boardState[idx1] == (int)color
            && (int)this.boardState[idx2] == (int)color)
        {
            return true;
        }
        return false;
    }

    public bool CheckIfPlayerWon(PlayerColor color)
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

    public void Dispose()
    {
        this.boardState.Dispose();
    }
}
