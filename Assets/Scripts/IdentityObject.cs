using UnityEngine;

namespace KSGFK
{
    public class IdentityObject : MonoBehaviour
    {
        [SerializeField] private int runtimeId = -1;

        public int RuntimeId { get => runtimeId; set => runtimeId = Helper.SingleAssign(value, runtimeId != -1); }
    }
}