using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizScript : MonoBehaviour
{
    private Sprite[,] quizImages = new Sprite[10, 3];
    private int[] userAnswers = new int[10];
    private bool[] isOnClicked = new bool[4];
    public GameObject quizObjects;
    public GameObject quizObjects2;
    public GameObject quizObjects3;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button skipBtn;
    public Text stageNumText;
    private Vector3 velocity1 = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;
    private Vector3 velocity3 = Vector3.zero;

    private float smoothTime = 0.4f;
    private bool isMoving = false;

    private int stageNum = 1;

    private int currentFilterType = 0;
    private int targetFilterType = 0;
    void Start()
    {
        // 버튼 클릭 시 NextSet 호출
        button1.onClick.AddListener(() => OnButtonClicked(0));
        button2.onClick.AddListener(() => OnButtonClicked(1));
        button3.onClick.AddListener(() => OnButtonClicked(2));
        button4.onClick.AddListener(() => OnButtonClicked(3));

        skipBtn.onClick.AddListener(() => NextSet());
        LoadQuizImages();
    }
    public void NextSet()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveDownAndReturn());
        }
    }

    private IEnumerator MoveDownAndReturn()
    {
        isMoving = true;

        Vector3 target1Down = new Vector3(-36.6f, 0, 0f);
        Vector3 target2Down = new Vector3(-30f, 0, 0f);
        Vector3 target3Down = new Vector3(-24.6f, 0, 0f);

        button1.GetComponent<Image>().color = Color.white;
        button2.GetComponent<Image>().color = Color.white;
        button3.GetComponent<Image>().color = Color.white;
        button4.GetComponent<Image>().color = Color.white;
        

        while ((quizObjects.transform.position - target1Down).sqrMagnitude > 0.01f ||
               (quizObjects2.transform.position - target2Down).sqrMagnitude > 0.01f ||
               (quizObjects3.transform.position - target3Down).sqrMagnitude > 0.01f)
        {
            quizObjects.transform.position = Vector3.SmoothDamp(
                quizObjects.transform.position, target1Down, ref velocity1, smoothTime);

            quizObjects2.transform.position = Vector3.SmoothDamp(
                quizObjects2.transform.position, target2Down, ref velocity2, smoothTime);

            quizObjects3.transform.position = Vector3.SmoothDamp(
                quizObjects3.transform.position, target3Down, ref velocity3, smoothTime);

            yield return null;
        }

        userAnswers[stageNum - 1] = currentFilterType;
        ChangePhotos(); // 사진 변경
        Debug.Log($"스프라이트를 찾을 수 없음: {currentFilterType}");
        StartCoroutine(BlendBetweenFilters(quizObjects2.transform.GetChild(0).GetComponent<SpriteRenderer>().material, currentFilterType, 0));

        yield return new WaitForSeconds(0.01f);

        Vector3 target1Up = new Vector3(-6.6f, 0f, 0f);
        Vector3 target2Up = new Vector3(0f, 0f, 0f);
        Vector3 target3Up = new Vector3(6.6f, 0f, 0f);

        // velocity 초기화
        velocity1 = velocity2 = velocity3 = Vector3.zero;

        while ((quizObjects.transform.position - target1Up).sqrMagnitude > 0.01f ||
               (quizObjects2.transform.position - target2Up).sqrMagnitude > 0.01f ||
               (quizObjects3.transform.position - target3Up).sqrMagnitude > 0.01f)
        {
            quizObjects.transform.position = Vector3.SmoothDamp(
                quizObjects.transform.position, target1Up, ref velocity1, smoothTime);

            quizObjects2.transform.position = Vector3.SmoothDamp(
                quizObjects2.transform.position, target2Up, ref velocity2, smoothTime);

            quizObjects3.transform.position = Vector3.SmoothDamp(
                quizObjects3.transform.position, target3Up, ref velocity3, smoothTime);

            yield return null;
        }

        isMoving = false;
    }

    private void ChangePhotos()
    {
        var photo1 = quizObjects.transform.GetChild(0).GetComponent<SpriteRenderer>();
        var photo2 = quizObjects2.transform.GetChild(0).GetComponent<SpriteRenderer>();
        var photo3 = quizObjects3.transform.GetChild(0).GetComponent<SpriteRenderer>();

        stageNum++;
        if (stageNum > 10)
        {
            SceneManager.LoadScene("EpilogueScene");
            return;
        }
        photo1.sprite = quizImages[stageNum - 1, 0];
        photo2.sprite = quizImages[stageNum - 1, 1];
        photo3.sprite = quizImages[stageNum - 1, 2];
        Debug.Log($"스프라이트를 찾을 수 없음: {stageNum}");
        stageNumText.text = $"{stageNum}/10";
        currentFilterType = 0;
        targetFilterType = 0;
    }

    private void LoadQuizImages()
    {
        for (int y = 0; y < 10; y++) // 세로 방향: Quiz 1 ~ Quiz 10
        {
            string folderName = $"Art/Quiz {y + 1}";

            for (int x = 0; x < 3; x++) // 가로 방향: 0.png ~ 2.png
            {
                string path = $"{folderName}/{x + 1}";
                Sprite sprite = Resources.Load<Sprite>(path);

                if (sprite != null)
                {
                    quizImages[y, x] = sprite;
                }
                else
                {
                    Debug.LogWarning($"스프라이트를 찾을 수 없음: {path}");
                }
            }
        }
    }

    void OnButtonClicked(int index)
    {
        // 모든 상태 초기화
        for (int i = 0; i < isOnClicked.Length; i++)
        {
            isOnClicked[i] = false;
        }

        // 해당 버튼만 true로 설정
        isOnClicked[index] = true;

        UpdateButtonStates();
    }

    void UpdateButtonStates()
    {
        SetButtonVisual(button1, isOnClicked[0]);
        SetButtonVisual(button2, isOnClicked[1]);
        SetButtonVisual(button3, isOnClicked[2]);
        SetButtonVisual(button4, isOnClicked[3]);
    }

    void SetButtonVisual(Button btn, bool isOn)
    {
        var colors = btn.colors;
        btn.GetComponent<Image>().color = isOn ? Color.green : Color.white;
        btn.colors = colors;

        if (isOn)
        {
            setPhotoColor(btn, true);
        }
    }



    void setPhotoColor(Button btn, bool isOn)
    {
        var photo2 = quizObjects2.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (!isOn) return;

        // 필터 타입 설정
        switch (btn.name)
        {
            case "Button1": targetFilterType = 1; break; // 흑백
            case "Button2": targetFilterType = 2; break; // 세피아
            case "Button3": targetFilterType = 3; break; // 빨강 강조
            case "Button4": targetFilterType = 6; break; // 원래 상태
        }

        if (photo2.material != null)
        {
            photo2.material.SetFloat("_FilterType", targetFilterType);
        }

        Material mat = photo2.material;
        StartCoroutine(BlendBetweenFilters(mat, currentFilterType, targetFilterType));
        currentFilterType = targetFilterType; // 필터 애니메이션 시작
    }

    IEnumerator BlendBetweenFilters(Material mat, int fromFilter, int toFilter, float duration = 0.5f)
    {
        mat.SetFloat("_PrevFilterType", fromFilter);
        mat.SetFloat("_NextFilterType", toFilter);
        mat.SetFloat("_FilterStrength", 1f);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            mat.SetFloat("_BlendAmount", t);
            time += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_BlendAmount", 1f);
    }
}
