using System;
using System.Collections.Generic;
using AStar;
using UnityEngine.Pool;


namespace Module.AStar
{
    public class AStar<T> where T : IPathNode<T>
    {
        enum NodeState
        {
            None,

            Open,

            Closed
        }

        class WrapNode : IComparable<WrapNode>
        {
            public NodeState State { set; get; }

            public T Node { set; get; }

            public T PreviousNode { set; get; }

            /// <summary>
            /// 当前得分
            /// </summary>
            public int GScore { set; get; }

            /// <summary>
            /// 启发得分
            /// </summary>
            public int HScore { set; get; }

            /// <summary>
            /// 总分
            /// </summary>
            public int FScore => GScore + HScore;

            public void Clean()
            {
                State = NodeState.None;
                Node = default;
                PreviousNode = default;
                GScore = 0;
                HScore = 0;
            }
           
            public int CompareTo(WrapNode other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return FScore.CompareTo(other.FScore);
            }
        }

        private readonly ObjectPool<WrapNode> WrapNodePool =
            new ObjectPool<WrapNode>(() => new WrapNode(), (s) => s.Clean());

        /// <summary>
        /// 启发函数
        /// </summary>
        private Func<T, T, int> Heuristic { set; get; }

        private Func<T, T, int> CostFunc { set; get; }
        private Func<T, bool> TargetPredicate { set; get; }

        /// <summary>
        /// 优先级队列
        /// </summary>
        private IPriorityQueue<WrapNode> PriorityQueue { set; get; }

        private Dictionary<T, WrapNode> NodeCache { set; get; }
        private T Target { set; get; }

        /// <summary>
        /// A*算法
        /// </summary>
        /// <param name="heuristic">启发函数</param>
        /// <param name="costFunc">消耗函数</param>
        public AStar(Func<T, T, int> heuristic, Func<T, T, int> costFunc)
        {
            PriorityQueue = new PriorityQueue<WrapNode>();
            NodeCache = new Dictionary<T, WrapNode>();
            CostFunc = costFunc;
            Heuristic = heuristic;
        }

        public List<T> FindPath(T start, T target, Func<T, bool> targetPredicate = null)
        {
            Target = target;
            TargetPredicate = targetPredicate ?? DefaultTargetPredicate;
            foreach (var cacheValue in NodeCache.Values)
            {
                WrapNodePool.Release(cacheValue);
            }
            PriorityQueue.Clear();
            NodeCache.Clear();
            AddInitialNode(start);
            return FindPathInner();
        }

        void AddInitialNode(T node)
        {
            var startNode = WrapNodePool.Get();
            startNode.Node = node;
            startNode.GScore = 0;
            startNode.GScore = Heuristic(node, Target);
            startNode.State = NodeState.Open;
            PriorityQueue.Add(startNode);
        }

        private bool DefaultTargetPredicate(T node)
        {
            return node.Equals(Target);
        }

        private List<T> FindPathInner()
        {
            while (CanExpand())
            {
                var p = Expand();
                if (TargetPredicate(p.Node))
                {
                    return MakePath(p);
                }
            }
#if UNITY_EDITOR
            throw new Exception($"No Path to {Target}");
#endif
            return null;
        }

        bool CanExpand()
        {
            while (!PriorityQueue.Empty) 
            {
                var state = PriorityQueue.Peek().State;
                if (state == NodeState.Closed) 
                {
                    PriorityQueue.Pop();
                } 
                else 
                {
                    return true;
                }
            }
            return false;
        }

        WrapNode Expand()
        {
            var minWrapNode = PriorityQueue.Pop();
            minWrapNode.State = NodeState.Closed;
            NodeCache[minWrapNode.Node] = minWrapNode;
            foreach (var connectNode in minWrapNode.Node.ConnectNodes)
            {
                var newScore = minWrapNode.GScore + CostFunc(connectNode, minWrapNode.Node);
                if (!NodeCache.TryGetValue(connectNode, out var wrapNode))
                {
                    wrapNode = WrapNodePool.Get();
                    wrapNode.Node = connectNode;
                    wrapNode.GScore = newScore;
                    wrapNode.HScore = Heuristic(connectNode, Target);
                    wrapNode.State = NodeState.Open;
                    wrapNode.PreviousNode = minWrapNode.Node;
                    NodeCache.Add(connectNode,wrapNode);
                }
                else
                {
                    if (newScore < wrapNode.GScore)
                    {
                        wrapNode.GScore = newScore;
                        wrapNode.PreviousNode = minWrapNode.Node;
                    }
                }

                if (wrapNode.State == NodeState.Open)
                {
                    PriorityQueue.Add(wrapNode);
                }
            }

            return minWrapNode;
        }


        List<T> MakePath(WrapNode destination)
        {
            var ret = new List<T>();
            var currentNode = destination.Node;
            while (NodeCache[currentNode].PreviousNode != null &&
                   !NodeCache[currentNode].PreviousNode.Equals(currentNode))
            {
                ret.Add(currentNode);
                currentNode = NodeCache[currentNode].PreviousNode;
            }

            ret.Add(currentNode);
            ret.Reverse();
            return ret;
        }
    }
}
