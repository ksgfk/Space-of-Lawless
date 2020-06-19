using UnityEngine;

namespace KSGFK
{
    public class EdgeController : MonoBehaviour
    {
        public SpriteRenderer left;
        public SpriteRenderer right;
        public SpriteRenderer up;
        public SpriteRenderer down;

        public Vector2 Size { get => transform.localScale; set => transform.localScale = value.ToVec3(1); }
    }
}