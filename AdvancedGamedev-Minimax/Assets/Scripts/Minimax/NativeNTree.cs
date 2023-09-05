using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public unsafe struct NativeNTree<T> : IDisposable where T : unmanaged
{

    public NativeList<TreeNode> data;

    public struct TreeNode
    {
        public int arrIdx;
        public int parentIdx;

        public T item;
    }

    public int root;

    public NativeNTree(Allocator allocator)
    {
        this.data = new NativeList<TreeNode>(1, allocator);
        this.root = -1;
    }

    public TreeNode GetRootNode()
    {
        if (this.root >= 0)
        {
            return this.data[this.root];
        }
        return default;
    }

    public void Clear()
    {
        this.data.Clear();
        this.root = -1;
    }

    public TreeNode AddChild(TreeNode parentNode, T data)
    {
        var node = new TreeNode()
        {
            arrIdx = this.data.Length,
            item = data,
            parentIdx = parentNode.arrIdx
        };
        this.data.Add(node);
        return node;
    }

    public void AddRoot(T data)
    {
        var rootNode = new TreeNode()
        {
            arrIdx = 0,
            item = data,
            parentIdx = -1
        };
        this.data.Add(rootNode);
        this.root = 0;
    }

    public void Dispose()
    {
        if (this.data.IsCreated)
        {
            this.data.Dispose();
        }
    }
}
