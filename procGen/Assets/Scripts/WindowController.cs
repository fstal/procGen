using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowController : MonoBehaviour
{

    [SerializeField]
    private Text modalWindowText;

    [SerializeField]
    private GameObject modalWindow;

    [SerializeField]
    private GameObject menuWindow;

    [SerializeField]
    private Button closeButton;

    string header = "How to navigate the airplane";
    string instruction = "By using the keys\nW, A, S and D";

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Show();
    }

    public void Show()
    {
        modalWindowText.text = header + "\n" + "\n" + instruction;
        modalWindow.SetActive(true);
    }

    public void Hide()
    {
        modalWindow.SetActive(false);
        menuWindow.SetActive(true);
    }
}
