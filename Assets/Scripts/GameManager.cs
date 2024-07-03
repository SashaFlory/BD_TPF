using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public TriviaManager triviaManager;

    public List<question> responseList; //lista donde guardo la respuesta de la query hecha en la pantalla de selecci�n de categor�a

    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();
    public bool queryCalled;

    public int score;

    private int _maxAttempts = 10;
    public int _numQuestionAnswered = 0;
    string _correctAnswer;

    public static GameManager Instance { get; private set; }

    string supabaseUrl = "https://cmueityyxnpdpsrgdbpv.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNtdWVpdHl5eG5wZHBzcmdkYnB2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTk5NTA3MDIsImV4cCI6MjAzNTUyNjcwMn0.GAgGLxEq_rWy_HLxlkkuNyegotlM6Zs2N5UzidRfn9w"; //COMPLETAR

    Supabase.Client clientSupabase;

    private HashSet<int> shownQuestions = new HashSet<int>();


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

          // Inicialización de clientSupabase
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

    }

    void Start()
    {
        StartTrivia();
        queryCalled = false;

    }

    void StartTrivia()
    {
        // Cargar la trivia desde la base de datos
        //triviaManager.LoadTrivia(currentTriviaIndex);

        //print(responseList.Count);
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {
            if (shownQuestions.Count >= responseList.Count)
            {
                EndGame();
                return;
            }

            do
            {
                randomQuestionIndex = Random.Range(0, responseList.Count);
            } while (shownQuestions.Contains(randomQuestionIndex));

            shownQuestions.Add(randomQuestionIndex);

            _correctAnswer = responseList[randomQuestionIndex].CorrectOption;

            _answers.Clear();
            _answers.Add(responseList[randomQuestionIndex].Answer1);
            _answers.Add(responseList[randomQuestionIndex].Answer2);
            _answers.Add(responseList[randomQuestionIndex].Answer3);
            _answers.Shuffle();

            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];
                int index = i; // Captura el valor actual de i en una variable local
                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            UIManagment.Instance.queryCalled = true;
        }

    }

    void EndGame()
    {
        Debug.Log("Juego terminado. Puntos totales: " + UIManagment.Instance.score);

        // Obtener el id del usuario actualmente logueado desde SupabaseManager
        int userId = SupabaseManager.CurrentUserId;

        // Obtener el id de la categoría de trivia actual (usando el id de la trivia seleccionada)
        int categoryId = TriviaSelection.SelectedTriviaId;

        // Obtener el puntaje final
        int puntajeFinal = UIManagment.Instance.score;

        // Llamar al método para guardar en Supabase
        GuardarIntentoEnSupabase(userId, categoryId, puntajeFinal);

        Debug.Log("Has ganado");
        SceneManager.LoadScene("WinScene"); // Cambia "WinScene" por el nombre de tu escena de victoria
    }

    public async void GuardarIntentoEnSupabase(int userId, int categoryId, int puntajeFinal)
    {
        // Consultar el último id utilizado (ID = index)
        var ultimoId = await clientSupabase
            .From<intento>()
            .Select("id")
            .Order(intento => intento.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el último id
            .Get();

        int nuevoId = 1; // Valor predeterminado si la tabla está vacía

        if (ultimoId.Models.Count > 0)
        {
            nuevoId = ultimoId.Models[0].id + 1; // Incrementar el último id
        }

        // Crear un nuevo intento con los datos obtenidos
        var nuevoIntento = new intento
        {
            id = nuevoId,
            username = userId,
            category = categoryId,
            score = puntajeFinal
        };

        // Insertar el nuevo intento en Supabase
        var resultado = await clientSupabase
            .From<intento>()
            .Insert(new[] { nuevoIntento });

        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Intento guardado correctamente en Supabase.");
        }
        else
        {
            Debug.LogError("Error al guardar el intento en Supabase: " + resultado.ResponseMessage);
        }
    }

    public void Update()
    {
        
    }
}

