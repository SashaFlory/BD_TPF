using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{
  
    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _timerText;
    
    [SerializeField] GameObject _gameoverPanel;

    string _correctAnswer;

    public Button[] _buttons = new Button[3];

    [SerializeField] Button _backButton;

    private List<string> _answers = new List<string>();

    public bool queryCalled;

    private Color _originalButtonColor;

    public static UIManagment Instance { get; private set; }

    public float currentTime = 0f;
    public float startingTime = 10f;

    public int score;
    public int scoreTime;


    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

    }


    private void Start()
    {
        queryCalled = false;

        _originalButtonColor = _buttons[0].GetComponent<Image>().color;

        _gameoverPanel.SetActive(false);

        currentTime = startingTime;
        score = 0;
        scoreTime = 0;
    }

    void Update()
    {
        _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;

        GameManager.Instance.CategoryAndQuestionQuery(queryCalled);

        currentTime -= 1 * Time.deltaTime;
        if (currentTime <= 0){
            currentTime = 0;

        }
        //print(currentTime);
        _scoreText.text = score.ToString("f0");
        _timerText.text = currentTime.ToString("f0"); 

    }
    public void OnButtonClick(int buttonIndex)
    {
        
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;

        _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

        if (selectedAnswer == _correctAnswer)
        {
            //scoreTime = Mathf.RoundToInt(currentTime);
            score += Mathf.RoundToInt(currentTime);
            currentTime = 0;
            print(score);
            //Debug.Log("score:", score);
            Debug.Log("�Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            Invoke("RestoreButtonColor", 2f);
            GameManager.Instance._answers.Clear();
            Invoke("NextAnswer", 2f);
            
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Int�ntalo de nuevo.");
            
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("RestoreButtonColor", 2f);
            _gameoverPanel.SetActive(true);

        }


    }

    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    private void NextAnswer()
    {
        queryCalled = false;
        //scoreTime = 0;
        currentTime = startingTime;
    }

    public void PreviousScene()
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


}
