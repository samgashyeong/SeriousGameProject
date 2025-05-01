using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button skipBtn;
    void Start()
    {
        skipBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("QuizScene");
        });
    }


    void Update()
    {

    }
}
