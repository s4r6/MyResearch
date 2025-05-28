using UnityEngine;
using UnityEngine.InputSystem.Controls;
using View.UI;

public class TestDocument : MonoBehaviour
{
    [SerializeField]
    DocumentView view;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("•\Ž¦");
            //view.DisplayDocument();
        }


    }
}
