using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameMenu : MonoBehaviour
{
    [SerializeField] private LevelChooser _levelChooser;
    [SerializeField] private MapLoader _mapLoader;
    [SerializeField] private Image _background;
    [SerializeField] private Button _skipLevelButton;
    [SerializeField] private Button _againButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _startLevelButton;


    public bool CanStartNextLevel => _levelChooser.LastLevelLoaded + 1 < _levelChooser.LevelsCount;

    private void Start()
    {
        ShowStartMenu();
    }

    private void OnEnable()
    {
        _mapLoader.LevelCompleted.AddListener(ShowWonMenu);
        _mapLoader.LevelFailed.AddListener(ShowLostMenu);
    }

    private void OnDisable()
    {
        _mapLoader.LevelCompleted.RemoveListener(ShowWonMenu);
        _mapLoader.LevelFailed.RemoveListener(ShowLostMenu);
    }

    [ContextMenu("Hide menu")]
    public void HideAll()
    {
        _background.enabled = false;
        _skipLevelButton.gameObject.SetActive(false);
        _againButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
        _startLevelButton.gameObject.SetActive(false);
    }

    [ContextMenu("Show start menu")]
    public void ShowStartMenu()
    {
        _background.enabled = true;
        _skipLevelButton.gameObject.SetActive(false);
        _againButton.gameObject.SetActive(false);
        _continueButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
        _startLevelButton.gameObject.SetActive(true);
        _startLevelButton.interactable = CanStartNextLevel;
    }

    [ContextMenu("Show pause menu")]
    public void ShowPauseMenu()
    {
        _background.enabled = true;
        _skipLevelButton.gameObject.SetActive(false);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(true);
        _nextLevelButton.gameObject.SetActive(false);
        _startLevelButton.gameObject.SetActive(false);
    }

    [ContextMenu("Show won menu")]
    public void ShowWonMenu()
    {
        _background.enabled = true;
        _skipLevelButton.gameObject.SetActive(false);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(false);

        _nextLevelButton.gameObject.SetActive(true);
        _nextLevelButton.interactable = CanStartNextLevel;
    }

    [ContextMenu("Show lost menu")]
    public void ShowLostMenu()
    {
        _background.enabled = true;
        _skipLevelButton.gameObject.SetActive(true);
        _againButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
    }
}
