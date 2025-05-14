using UnityEngine;

namespace Infrastructure
{
    public interface ISerializer<T>
    {
        T Load(string json);
    }
}