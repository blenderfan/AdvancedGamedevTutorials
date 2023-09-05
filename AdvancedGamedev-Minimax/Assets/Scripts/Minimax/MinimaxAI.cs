using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class MinimaxAI : IDisposable
{
    #region Public Variables

    public int searchDepth = 1;

    #endregion

    #region Private Variables

    //It is actually probably 7, but as it is only for memory allocation, the exact number is not so important
    private const int maxMoves = 9;

    private BoardState boardState;
    private NativeNTree<float> gameTree;

    private Move bestMove;

    #endregion

    public Move GetBestMove() => this.bestMove;

    [BurstCompile]
    private struct BestMoveJob : IJob
    {

        public BoardState state;

        public int searchDepth;

        public NativeNTree<float> gameTree;

        public PlayerColor maxPlayer;


        private void MinMaxExpand(NativeNTree<float> tree)
        {

        }

        public unsafe void Execute()
        {
            const int boardPositionCount = 9;

            var depthStack = new NativeArray<PositionState>(boardPositionCount * (this.searchDepth + 1), Allocator.Temp);

            var srcPtr = (PositionState*)this.state.boardState.GetUnsafePtr();
            var dstPtr = (PositionState*)depthStack.GetUnsafePtr();
            UnsafeUtility.MemCpy(dstPtr, srcPtr, sizeof(PositionState) * boardPositionCount);

            this.gameTree.Clear();
            this.gameTree.AddRoot(float.NegativeInfinity);

 

            this.MinMaxExpand(this.gameTree);

            var child = this.gameTree.data[1];
            
        }



        public float Evaluate(BoardState state)
        {
            float score = 0.0f;

            bool maxWon = state.CheckIfPlayerWon(this.maxPlayer);
            bool minWon = state.CheckIfPlayerWon(this.maxPlayer == PlayerColor.BLUE ? PlayerColor.ORANGE : PlayerColor.BLUE);

            if (maxWon) score = 1.0f;
            if (minWon) score = -1.0f;

            return score;
        }
    }

    public void Init(PlayerColor color, int uncommittedBlue, int uncommittedOrange)
    {

        this.gameTree = new NativeNTree<float>(Allocator.Persistent);
        this.boardState = new BoardState(color, uncommittedBlue, uncommittedOrange);
    }

    public void ScheduleBestMove()
    {
        this.gameTree.Clear();
    }

    public void Dispose()
    {
        this.gameTree.Dispose();
        this.boardState.Dispose();
    }
}
