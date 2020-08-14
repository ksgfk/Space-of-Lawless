using UnityEngine;

namespace KSGFK
{
    public class EntryEntityItem : EntryEntity
    {
        private readonly int _physicLayer;

        public override string RegisterName => "entity_item";

        public EntryEntityItem() { _physicLayer = LayerMask.NameToLayer("EntityItem"); }

        public override bool Check(out string reason)
        {
            reason = string.Empty;
            return true;
        }

        protected override Entity SpawnEntity()
        {
            var go = new GameObject(RegisterName) {tag = "Entity", layer = _physicLayer};
            var coll = go.AddComponent<CircleCollider2D>();
            coll.isTrigger = true;
            var ei = go.AddComponent<EntityItem>();
            return ei;
        }
    }
}