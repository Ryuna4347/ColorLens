using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Stage_Number : MonoBehaviour
{
    public Text Stage_Text;
    void Start()
    {
        Stage_Text.text = SceneManager.GetActiveScene().name;
    }
}
