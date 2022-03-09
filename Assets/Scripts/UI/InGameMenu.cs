using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameMenu : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Button _backToMenuButton;
    [SerializeField] private Button _againButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _anotherChanceButton;
    [SerializeField] private Button _nextLevelButton;

    private void Start()
    {
        HideAll();
    }

    [ContextMenu("Hide menu")]
    public void HideAll()
    {
        _background.enabled = false;
        _backToMenuButton.gameObject.SetActive(false);
        _againButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        _anotherChanceButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
    }

    [ContextMenu("Show pause menu")]
    public void ShowPauseMenu()
    {
        _background.enabled = true;
        _backToMenuButton.gameObject.SetActive(true);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(true);
        _anotherChanceButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
    }

    [ContextMenu("Show won menu")]
    public void ShowWonMenu()
    {
        _background.enabled = true;
        _backToMenuButton.gameObject.SetActive(true);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(false);
        _anotherChanceButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(true);
    }

    [ContextMenu("Show lost menu")]
    public void ShowLostMenu()
    {
        _background.enabled = true;
        _backToMenuButton.gameObject.SetActive(true);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(false);
        _anotherChanceButton.gameObject.SetActive(true);
        _nextLevelButton.gameObject.SetActive(false);
    }
}
