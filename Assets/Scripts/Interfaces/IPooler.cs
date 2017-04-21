using UnityEngine;

namespace SnakeGame
{
    internal interface IPooler
    {
        GameObject GetPooledObject();
    }
}
