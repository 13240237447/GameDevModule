using System;
using System.Collections.Generic;


namespace Logic.Primitives
{
    /// <summary>
    /// 优先级队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPriorityQueue<T>
    {
        void Add(T item);
        bool Empty { get; }
        T Peek();
        T Pop();

        void Clear();
    }


    public class PriorityQueue<T> : IPriorityQueue<T>
    {
        readonly List<T[]> items;

        readonly IComparer<T> comparer;

        private int Level { set; get; }
        
        private int Index { set; get; }

        public PriorityQueue()
            : this(Comparer<T>.Default)
        {
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            items = new List<T[]> { new T[1] };
            this.comparer = comparer;
        }

        public void Add(T item)
        {
            var addLevel = Level;
            var addIndex = Index;
            while (addLevel >= 1 && comparer.Compare(Above(addLevel, addIndex), item) > 0)
            {
                items[addLevel][addIndex] = Above(addLevel, addIndex);
                --addLevel;
                addIndex >>= 1;
            }

            items[addLevel][addIndex] = item;

            if (++Index >= (1 << Level))
            {
                Index = 0;
                if (items.Count <= ++Level)
                    items.Add(new T[1 << Level]);
            }
        }

        public bool Empty => Level == 0;

        T At(int level, int index)
        {
            return items[level][index];
        }
        
        T Above(int level, int index)
        {
            return items[level - 1][index >> 1];
        }

        T Last()
        {
            var lastLevel = Level;
            var lastIndex = Index;

            if (--lastIndex < 0)
                lastIndex = (1 << --lastLevel) - 1;

            return At(lastLevel, lastIndex);
        }

        public T Peek()
        {
            if (Level <= 0 && Index <= 0)
                throw new InvalidOperationException("PriorityQueue empty.");
            return At(0, 0);
        }

        public T Pop()
        {
            var ret = Peek();
            BubbleInto(0, 0, Last());
            if (--Index < 0)
                Index = (1 << --Level) - 1;
            return ret;
        }

        public void Clear()
        {
            Level = 0;
            Index = 0;
        }

        void BubbleInto(int intoLevel, int intoIndex, T val)
        {
            var downLevel = intoLevel + 1;
            var downIndex = intoIndex << 1;

            if (downLevel > Level || (downLevel == Level && downIndex >= Index))
            {
                items[intoLevel][intoIndex] = val;
                return;
            }

            if ((downLevel < Level || (downLevel == Level && downIndex < Index - 1)) &&
                comparer.Compare(At(downLevel, downIndex), At(downLevel, downIndex + 1)) >= 0)
                ++downIndex;

            if (comparer.Compare(val, At(downLevel, downIndex)) <= 0)
            {
                items[intoLevel][intoIndex] = val;
                return;
            }

            items[intoLevel][intoIndex] = At(downLevel, downIndex);
            BubbleInto(downLevel, downIndex, val);
        }
    }
}
