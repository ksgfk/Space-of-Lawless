using UnityEngine;

namespace KSGFK
{
    public interface IShipModule
    {
        GameObject BaseGameObject { get; }
        
        EntityShip Frame { get; set; }
    }
}