using System.Collections.Generic;


namespace Logic
{
    public class EntityInfo
    {
        readonly TypeDictionary traits = new TypeDictionary();

        public EntityInfo(IEnumerable<TraitInfo> traitInfos)
        {
            foreach (var traitInfo in traitInfos)
            {
                traits.Add(traitInfo);       
            }
        }
        
        public bool HasTraitInfo<T>() where T : TraitInfo { return traits.Contains<T>(); }
        
        public T TraitInfo<T>() where T : TraitInfo { return traits.Get<T>(); }
        
        public T TraitInfoOrDefault<T>() where T : TraitInfo { return traits.GetOrDefault<T>(); }
        
        public IEnumerable<T> TraitInfos<T>() where T : TraitInfo { return traits.WithInterface<T>(); }
    }
}
