using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _scoreText.text = UIManagment.Instance.score.ToString("f0");
        
    }

     public void BotonCambioEscena(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void BotonPerderGanar(string name)
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);

        SceneManager.LoadScene(name);
    }
}
