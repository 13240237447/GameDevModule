using System;
using System.Collections.Generic;


namespace Logic
{
    static class Exts
    {
        public static int BinarySearchMany(this List<Entity> list, uint searchFor)
        {
            var start = 0;
            var end = list.Count;
            while (start != end)
            {
                var mid = (start + end) / 2;
                if (list[mid].EntityId < searchFor)
                    start = mid + 1;
                else
                    end = mid;
            }

            return start;
        }
        
        public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k, Func<K, V> createFn)
        {
            if (!d.TryGetValue(k, out var ret))
                d.Add(k, ret = createFn(k));
            return ret;
        }
        
        public static IEnumerable<Type> BaseTypes(this Type t)
        {
            while (t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }
    }

    
    interface ITraitContainer
    {
        void AddEntity(Entity entity, object trait);
        void RemoveEntity(uint actor);
    }
    
    
    public class TraitDictionary
    {
        private readonly Func<Type, ITraitContainer> CreateTraitContainer =
            (t) => (ITraitContainer)typeof(TraitContainer<>).MakeGenericType(t).GetConstructor(Type.EmptyTypes)?.Invoke(null);

        readonly Dictionary<Type, ITraitContainer> traits = new Dictionary<Type, ITraitContainer>();

        static void CheckDestroyed(Entity entity)
        {
            if (entity.Disposed)
                throw new InvalidOperationException($"Attempted to get trait from destroyed object ({entity})");
        }
        
        ITraitContainer InnerGet(Type t)
        {
            return traits.GetOrAdd(t, CreateTraitContainer);
        }

        TraitContainer<T> InnerGet<T>()
        {
            return (TraitContainer<T>)InnerGet(typeof(T));
        }
        
        public void AddTrait(Entity entity, object val)
        {
            var t = val.GetType();

            foreach (var i in t.GetInterfaces())
                InnerAdd(entity, i, val);
            foreach (var tt in t.BaseTypes())
                InnerAdd(entity, tt, val);
        }

        void InnerAdd(Entity entity, Type t, object val)
        {
            InnerGet(t).AddEntity(entity, val);
        }
        
        
        public void RemoveActor(Entity entity)
        {
            foreach (var t in traits)
                t.Value.RemoveEntity(entity.EntityId);
        }

        public void ApplyToActorsWithTrait<T>(Action<Entity, T> action)
        {
            InnerGet<T>().ApplyToAll(action);
        }
        
        class TraitContainer <T> : ITraitContainer
        {
            readonly List<Entity> entities = new List<Entity>();
        
            readonly List<T> traits = new List<T>();
        
        
            public void AddEntity(Entity entity, object trait)
            {
                var insertIndex = entities.BinarySearchMany(entity.EntityId + 1);
                entities.Insert(insertIndex, entity);
                traits.Insert(insertIndex, (T)trait);
            }
        
        
            public void RemoveEntity(uint actor)
            {
                var startIndex = entities.BinarySearchMany(actor);
                if (startIndex >= entities.Count || entities[startIndex].EntityId != actor)
                    return;

                var endIndex = startIndex + 1;
                while (endIndex < entities.Count && entities[endIndex].EntityId == actor)
                    endIndex++;

                var count = endIndex - startIndex;
                entities.RemoveRange(startIndex, count);
                traits.RemoveRange(startIndex, count);
            }
    
            
            public T GetOrDefault(Entity entity)
            {
                var index = entities.BinarySearchMany(entity.EntityId);
                if (index >= entities.Count || entities[index] != entity)
                    return default;

                if (index + 1 < entities.Count && entities[index + 1] == entity)
                    throw new InvalidOperationException($"Actor {entity.GetType().Name} has multiple traits of type `{typeof(T)}`");

                return traits[index];
            }
            
            public void ApplyToAll(Action<Entity, T> action)
            {
                for (var i = 0; i < entities.Count; i++)
                    action(entities[i], traits[i]);
            }
        }
    }
}
