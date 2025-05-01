using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneScript : MonoBehaviour
{
    public Button startBtn;

    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("IntroScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
