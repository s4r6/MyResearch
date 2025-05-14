using Infrastructure.Repository;
using UnityEngine;

public class TestObjectRepository : MonoBehaviour
{
    private void Start()
    {
        var repository = new ObjectRepository();
        repository.Initialize();
    }
}
