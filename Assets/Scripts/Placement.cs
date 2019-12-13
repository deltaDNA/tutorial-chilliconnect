using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Placement : MonoBehaviour
{

    public string type { get; set; }
    public string position { get; set; }
    public int frequency { get; set; }
    public int limit { get; set; }
    public int promoID { get; set; }
    public bool isConsoleVisible = false;

    public Text txtpromoType;
    public GameManager gameManager;
    public Button bttnWatchAd;
    public Button bttnShowStore;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        SetConsoleVisibility(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hide()
    {
        isConsoleVisible = false;
        SetConsoleVisibility(false);
    }
    public void Show()
    {
        isConsoleVisible = true;
        SetConsoleVisibility(isConsoleVisible);
    }

    private void SetConsoleVisibility(bool isVisible)
    {

        //UpdateConsole();
        gameObject.SetActive(isVisible);
        gameManager.readyToStart = !isVisible;
    }

    public void UpdateConsole()
    {

    }

    private void SetAsIAP()
    {
        txtpromoType.text = "IAP"; 
        
    }
    public void SetAsAd()
    {
        txtpromoType.text = "Ad";
    }


    public void WatchAd()
    {
        this.Hide();
        gameManager.readyToStart = true;
    }
    public void ShowStore()
    {
        this.Hide();
        gameManager.readyToStart = true;
    }

}
