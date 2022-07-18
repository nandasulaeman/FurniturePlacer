using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
    // Start is called before the first frame update

    private static DeletePlayerPrefs playerInstance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (playerInstance == null)
        {
            playerInstance = this;
            PlayerPrefs.DeleteAll();
        }
        else
        {
            DestroyObject(gameObject);
        }
    }
    void Start()
    {

    }


}
