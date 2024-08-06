using System.Collections.Generic;


namespace Logic.AStar
{
    public interface IPathNode<T>
    {
        /// <summary>
        /// 与之相连接的点
        /// </summary>
        IEnumerable<T> ConnectNodes { get; }
    }
    
}
