namespace KSGFK
{
    public abstract class Item : IdentityObject
    {
        public abstract void OnUse(EntityLiving user);
    }
}