using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://cmueityyxnpdpsrgdbpv.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNtdWVpdHl5eG5wZHBzcmdkYnB2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTk5NTA3MDIsImV4cCI6MjAzNTUyNjcwMn0.GAgGLxEq_rWy_HLxlkkuNyegotlM6Zs2N5UzidRfn9w"; //COMPLETAR

    Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public static int SelectedTriviaId { get; private set; } // Variable para almacenar el id de la trivia seleccionada
    public static TriviaSelection Instance { get; private set; } // Propiedad estática para acceder a la instancia única de TriviaSelection

    public DatabaseManager databaseManager;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        await SelectTrivias();
        PopulateDropdown();

        Instance = this;
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
            //Debug.Log("Trivias seleccionadas: " + trivias.Count);
            //foreach (var trivia in trivias)
            //{
            //    Debug.Log("ID: " + trivia.id + ", Categor�a: " + trivia.category);
            //}
        }

    }

    void PopulateDropdown()
    {
        
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }

    public void OnStartButtonClicked()
    {
      

        int selectedIndex = _dropdown.value;

        SelectedTriviaId = trivias[selectedIndex].id;
        string selectedTrivia = _dropdown.options[selectedIndex].text;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex+1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
