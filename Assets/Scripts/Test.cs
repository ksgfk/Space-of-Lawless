using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace KSGFK
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current.hKey.IsPressed())
            {
                World world = GameManager.Instance.World;
                var bullet = world.SpawnEntity<EntityBullet>("normal");
                var x = Random.Range(-1f, 1f);
                var y = Random.Range(-1f, 1f);
                bullet.Launch(new Vector2(x, y), Vector2.zero, Random.Range(1f, 10f), 10);
            }
        }
    }
}