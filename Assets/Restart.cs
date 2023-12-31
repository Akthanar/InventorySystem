using UnityEngine;
using UnityEngine.SceneManagement;



public class Restart : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(0);
        }
    }
}
