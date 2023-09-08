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

    #region Private Variables

    //It is actually probably 7, but as it is only for memory allocation, the exact number is not so important
    private const int maxMoves = 9;

    private BoardState boardState;

    private int searchDepth = 1;

    private JobHandle bestMoveHandle;

    private NativeNTree<float> gameTree;
    private NativeReference<Move> bestMoveReference;

    private Move bestMove;

    #endregion

    public Move GetBestMove() => this.bestMove;

    [BurstCompile]
    private struct BestMoveJob : IJob
    {

        #region Public Variables

        public BoardState state;

        public int searchDepth;

        public NativeNTree<float> gameTree;
        public NativeReference<Move> bestMove;

        public PlayerColor maxPlayer;

        #endregion

        #region Private Variables

        private const int boardPositionCount = 9;
        private const int maxMoves = 9;

        private int simulatedMoves;

        #endregion


        private unsafe BoardState SimulateMove(BoardState state, NativeArray<PositionState> depthStack, Move move, int depth)
        {
            this.simulatedMoves++;

            var newState = state;
            int depthOffset = depth * boardPositionCount;

            var srcPtr = (PositionState*)depthStack.GetUnsafePtr() + depthOffset;
            var dstPtr = (PositionState*)newState.boardState.GetUnsafePtr();
            UnsafeUtility.MemCpy(dstPtr, srcPtr, sizeof(PositionState) * boardPositionCount);

            newState.MakeMove(move);

            newState.currentColor = newState.currentColor == PlayerColor.BLUE ? PlayerColor.ORANGE : PlayerColor.BLUE;

            if(depth + 1 <= this.searchDepth)
            {
                srcPtr = (PositionState*)newState.boardState.GetUnsafePtr();
                dstPtr = (PositionState*)depthStack.GetUnsafePtr() + (depth + 1) * boardPositionCount;
                UnsafeUtility.MemCpy(dstPtr, srcPtr, sizeof(PositionState) * boardPositionCount);
            }

            return newState;
        }


        private void MinMaxExpand(NativeNTree<float> tree, int treeNode, NativeArray<PositionState> depthStack, BoardState state, Move move, int depth,
            float alpha, float beta)
        {
            var newState = this.SimulateMove(state, depthStack, move, depth);

            bool min = depth % 2 != 0;

            float eval = this.Evaluate(state);

            var node = tree.data[treeNode];
            var newNode = tree.AddChild(node, min ? float.NegativeInfinity : float.PositiveInfinity);
            int newNodeIdx = newNode.arrIdx;

            //Game undecided
            if (eval == 0.0f && depth + 1 <= this.searchDepth)
            {

                var nextMoves = new NativeList<Move>(maxMoves, Allocator.Temp);
                newState.GetMoves(ref nextMoves);

                for (int i = 0; i < nextMoves.Length; i++)
                {
                    this.MinMaxExpand(tree, newNodeIdx, depthStack, newState, nextMoves[i], depth + 1, alpha, beta);

                    if (min)
                    {

                        alpha = Mathf.Max(alpha, tree.data[newNodeIdx].item);
                        if (alpha >= beta)
                        {
                            break;
                        }

                    }
                    else
                    {

                        beta = Mathf.Min(beta, tree.data[newNodeIdx].item);
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            } else
            {
                newNode.item = eval;
                tree.data[newNodeIdx] = newNode;
            }


            //This is the part, where one can see why the algorithm is called Minimax ^^
            if (min)
            {
                float minVal = Mathf.Min(tree.data[newNodeIdx].item, node.item);
                node.item = minVal;
            }
            else
            {
                float maxVal = Mathf.Max(tree.data[newNodeIdx].item, node.item);
                node.item = maxVal;
            }


            tree.data[node.arrIdx] = node;
        }

        public unsafe void Execute()
        {
            this.simulatedMoves = 0;

            var depthStack = new NativeArray<PositionState>(boardPositionCount * (this.searchDepth + 1), Allocator.Temp);

            var srcPtr = (PositionState*)this.state.boardState.GetUnsafePtr();
            var dstPtr = (PositionState*)depthStack.GetUnsafePtr();
            UnsafeUtility.MemCpy(dstPtr, srcPtr, sizeof(PositionState) * boardPositionCount);

            this.gameTree.Clear();
            this.gameTree.AddRoot(float.NegativeInfinity);

            var nextMoves = new NativeList<Move>(maxMoves, Allocator.Temp);
            this.state.GetMoves(ref nextMoves);

            float max = float.NegativeInfinity;
            for (int i = 0; i < nextMoves.Length; i++)
            {
                this.MinMaxExpand(this.gameTree, 0, depthStack, this.state, nextMoves[i], 0, float.NegativeInfinity, float.PositiveInfinity);
                var child = this.gameTree.data[0];

                float score = child.item;

                if(score > max)
                {
                    max = score;
                    this.bestMove.Value = nextMoves[i];
                }

                this.gameTree.Clear();
                this.gameTree.AddRoot(float.NegativeInfinity);
            }

            Debug.Log($"Simulated {this.simulatedMoves} moves!");
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

    public void Init(int searchDepth, PositionState[,] positions, PlayerColor color, int uncommittedBlue, int uncommittedOrange)
    {
        this.searchDepth = searchDepth;
        this.gameTree = new NativeNTree<float>(Allocator.Persistent);
        this.boardState = new BoardState(color, uncommittedBlue, uncommittedOrange);
        this.bestMoveReference = new NativeReference<Move>(Allocator.Persistent);

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                int idx = y * 3 + x;
                this.boardState.boardState[idx] = positions[x, y];
            }
        }
    }

    public void FinishBestMove()
    {
        this.bestMoveHandle.Complete();

        this.bestMove = this.bestMoveReference.Value;
    }

    public void ScheduleBestMove()
    {

        var bestMoveJob = new BestMoveJob()
        {
            bestMove = this.bestMoveReference,
            gameTree = this.gameTree,
            maxPlayer = this.boardState.currentColor,
            searchDepth = this.searchDepth,
            state = this.boardState,
        };

        this.bestMoveHandle = bestMoveJob.Schedule();
    }

    public void Dispose()
    {
        this.gameTree.Dispose();
        this.boardState.Dispose();
        this.bestMoveReference.Dispose();
    }
}
