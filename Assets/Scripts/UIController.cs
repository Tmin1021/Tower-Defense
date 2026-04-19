using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text liveText;
    [SerializeField] private TMP_Text resourceText;

    [SerializeField] private GameObject towerPanel;

    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();
    private Platform _currentPlatform;

    void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLiveText;
        GameManager.OnResourcesChanged += UpdateResourcesText; 
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
    }
    void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLiveText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }
    private void UpdateLiveText(int currentLives)
    {
        liveText.text = $"Lives: {currentLives}";
    }
    private void UpdateResourcesText(int currentResources)
    {
        resourceText.text = $"Resources: {currentResources}";
    }
    private void HandlePlatformClicked(Platform platform)
    {
        ShowTowerPanel();
        _currentPlatform = platform;
    }
    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f); // freeze
        PopulateTowerCards();
    }
    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        GameManager.Instance.SetTimeScale(1f); // continue
    }

    private void PopulateTowerCards()
    {
        // clear old cards
        foreach(var card in activeCards)
        {
            Destroy(card);
        }

        activeCards.Clear();

        foreach(var tower in towers)
        {
            GameObject towerCardObject = Instantiate(towerCardPrefab, cardsContainer);
            TowerCard towerCard = towerCardObject.GetComponent<TowerCard>();
            towerCard.Initiliazed(tower);
            activeCards.Add(towerCardObject);
        }
    }

    private void HandleTowerSelected(TowerData data) 
    {
        if(_currentPlatform == null)
        {
            return;
        }

        _currentPlatform.PlaceTower(data);
        _currentPlatform = null;
        HideTowerPanel();
    }
}
