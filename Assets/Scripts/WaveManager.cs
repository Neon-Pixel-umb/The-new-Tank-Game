using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public enum Challenge
{
    Healthy,       // Double health for all enemies
    Swarm,         // Increase wave amount and reduce spawn time
    Exhaust,       // Disable active item
    Speed,         // Increase enemy speed
    Stress,        // Drop to one heart on hit, recover after 5 seconds
    CircleInvasion,// Circle enemies spawn more frequently and don't count towards wave
    DoubleDamage   // Double damage received and extended stun for Circle enemies
}


public class WaveManager : MonoBehaviour
{
    public GameObject itemSlotPanel;
    public GameObject[] itemSlots;
    public TMP_Text instructionText;
    public TMP_Text descriptionText;
    public TMP_Text waveText;  // UI Text for displaying the current wave
    public Item[] itemOptions;
    public GameObject postWavePanel; // Panel for showing post-wave options
    public GameObject startPanel; // Start panel with start button
    public EnemySpawner[] enemySpawners;

    public int currentWave = 0;
    private bool isChallengeWave = false;
    private float healthMultiplier = 1.1f; // Initial enemy health multiplier per wave
    private float spawnRateMultiplier = 0.95f; // Faster spawn rate with each wave
    private int baseEnemyCount = 10; // Base number of enemies per wave
    private int enemiesRemaining;

    public Button nextWaveButton;
    public Button shopButton;
    public Button challengeButton;
    public Button startButton;

    public GameObject shopPanel; // Panel for the shop

    public GameObject[] shopItemSlots; // Shop item buttons for 7 items
    public List<Item> shopItems = new List<Item>();
    public Button backButton; // Button to return to postWavePanel
    private int waveCounterForShopRestock = 0; // Track waves to restock shop every 3 waves

    private bool isChallengeActive = false;

    private List<Item> currentItems = new List<Item>();
    public int standardItemCount = 3;
    public int specialEventItemCount = 5;
    private int itemsPickedCount = 0;
    private int maxItemsToPick = 1;
    private float rareChance = 0.01f;
    private float legendaryChance = 0f;
    private const float uncommonChance = 0.3f;
    private int activeEnemies = 0;
    public TMP_Text challengeText; // Text UI to show active challenges
    private List<Challenge> activeChallenges = new List<Challenge>();
    public TankController tankController;


    private void Start()
    {
        itemSlotPanel.SetActive(false);
        postWavePanel.SetActive(false);
        startPanel.SetActive(true); // Show start panel on game load
        startButton.onClick.AddListener(StartGame); // Set up the Start button

        nextWaveButton.onClick.AddListener(StartNextWave);
        shopButton.onClick.AddListener(OpenShop);
        challengeButton.onClick.AddListener(StartChallenge);
        backButton.onClick.AddListener(CloseShop);

        PopulateShopItems(); // Populate shop initially without waiting for 3 waves
    }

    private void StartGame()
{
    FindObjectOfType<TankController>().StartNewGame();
    currentWave = 0;
    UpdateWaveText(); // Initialize the wave display
    startPanel.SetActive(false); // Hide start panel
    ResetChallenges();
    StartNextWave(); // Begin the first wave  
}


    private void StartNextWave()
    {
        isChallengeActive = false;
        challengeText.gameObject.SetActive(false);
        postWavePanel.SetActive(false);
        currentWave++;
        waveCounterForShopRestock++; // Increment restock counter after each wave
        UpdateWaveText();
        StartWave(currentWave);
    }

    private void StartChallenge()
    {
        isChallengeActive = true;
        postWavePanel.SetActive(false);
        currentWave++;
        waveCounterForShopRestock++; // Increment restock counter after each wave
        UpdateWaveText();
        activeChallenges.Clear(); // Clear previous challenges
        int challengeCount = Mathf.Min(1 + currentWave / 10, 3); // Scale up challenges by wave count

        List<Challenge> allChallenges = new List<Challenge>((Challenge[])System.Enum.GetValues(typeof(Challenge)));

        for (int i = 0; i < challengeCount; i++)
        {
            Challenge selectedChallenge = allChallenges[Random.Range(0, allChallenges.Count)];
            allChallenges.Remove(selectedChallenge);
            activeChallenges.Add(selectedChallenge);
        }

        ApplyChallenges();
        DisplayActiveChallenges();

        StartWave(currentWave);
    }

    private void ApplyChallenges()
{
    foreach (var challenge in activeChallenges)
    {
        switch (challenge)
        {
            case Challenge.Healthy:
                foreach (var spawner in enemySpawners)
                {
                    spawner.challengeHealth *= 2;
                }
                break;

            case Challenge.Swarm:
                spawnRateMultiplier *= 0.7f;
                baseEnemyCount = Mathf.CeilToInt(baseEnemyCount * 1.5f);
                break;

            case Challenge.Exhaust:
                if (tankController.activeItem != null)
                    tankController.Exhaust = true; // Disables active item usage
                break;

            case Challenge.Speed:
                foreach (var spawner in enemySpawners)
                {
                    spawner.enemySpeedModifier *= 1.5f;
                }
                break;

            case Challenge.Stress:
                tankController.Stress = true;
                
                break;

            case Challenge.CircleInvasion:
                foreach (var spawner in enemySpawners)
                {
                    spawner.circleSpawnChance = 0.4f;
                }
                break;

            case Challenge.DoubleDamage:
                tankController.damageMultiplier = 2f;
                tankController.circleStunDurationMultiplier = 2f;
                break;
        }
    }
}



private void DisplayActiveChallenges()
{
    challengeText.text = "Challenges: " + string.Join(", ", activeChallenges);
    challengeText.gameObject.SetActive(true);
}

private void ResetChallenges()
{
    // Remove all temporary modifiers
    foreach (var spawner in enemySpawners)
    {
        spawner.challengeHealth = 1;
        spawner.enemySpeedModifier = 1;
        spawner.circleSpawnChance = spawner.defaultCircleSpawnChance;
    }
    tankController.Stress = false;
    tankController.Exhaust = false;
    tankController.damageMultiplier = 1f;
    tankController.circleStunDurationMultiplier = 1f;
    spawnRateMultiplier = 0.95f;

    challengeText.gameObject.SetActive(false); // Hide challenge text
}

    private void StartWave(int waveNumber)
{
    ScaleEnemyAttributes(); 

    int enemiesToSpawn = CalculateEnemiesForWave(waveNumber);
    float healthScale = CalculateEnemyHealthScale(waveNumber);

    enemiesRemaining = enemiesToSpawn; // Track the total enemies for this wave
    activeEnemies = 0;
    

    foreach (var spawner in enemySpawners)
    {
        
        spawner.wave = currentWave;
        int spawnerEnemyCount = enemiesToSpawn / enemySpawners.Length;
        spawner.StartSpawning(spawnerEnemyCount); // Start spawning in each spawner
        activeEnemies += spawnerEnemyCount;  // Track total active enemies
    }

    
}

private void UpdateWaveText()
{
    if (waveText != null)
    {
        waveText.text = "Wave: " + currentWave;
    }
}

    private int CalculateEnemiesForWave(int waveNumber)
    {
        // Formula to increase enemy count with each wave
        return Mathf.CeilToInt(5 * Mathf.Log(waveNumber + 1));
    }

    private float CalculateEnemyHealthScale(int waveNumber)
    {
        // Formula to scale health exponentially but gradually
        return Mathf.Pow(1.1f, waveNumber);
    }

    private void SpawnEnemies(int totalEnemies, float healthScale)
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.StartSpawning(totalEnemies / spawners.Length);
            // Pass healthScale to enemies if required
        }
    }

    private void ScaleEnemyAttributes()
{
    // Increase modifiers for all spawners, based on healthMultiplier and spawnRateMultiplier.
    foreach (var spawner in enemySpawners)
    {
        spawner.spawnCooldown *= Mathf.Pow(spawnRateMultiplier, currentWave);
    }
}


    private IEnumerator SpawnWaveEnemies()
    {
        foreach (var spawner in enemySpawners)
        {
            spawner.StartSpawning(enemiesRemaining / enemySpawners.Length); // Divide enemies among spawners
        }
        yield return null;
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;
        activeEnemies--;

        // Check if all enemies are defeated and spawning is complete
        if (activeEnemies <= 0 && AllSpawnersFinished() && FindObjectOfType<TankController>().health > 0)
        {
            if(tankController.LeftOvers){
                SoundManager.Instance.PlayGood();
                tankController.health += 1;
            }
            ResetChallenges();
            StartCoroutine(ShowItemSelectionScreen());
        }
    }

    private bool AllSpawnersFinished()
    {
        foreach (var spawner in enemySpawners)
        {
            if (!spawner.HasFinishedSpawning)
                return false;
        }
        return true;
    }

    private IEnumerator ShowItemSelectionScreen()
    {
        yield return new WaitForSeconds(2f); // Brief delay before showing item selection
        ShowItemSelection(isChallengeActive);
    }

    public void ShowItemSelection(bool specialEvent = false)
    {
        itemSlotPanel.SetActive(true);
        FindObjectOfType<TankController>().SetPaused(true);

        maxItemsToPick = specialEvent ? 2 : 1;
        itemsPickedCount = 0;
        instructionText.text = specialEvent ? "Choose 2" : "Choose 1";

        GenerateItems(specialEvent ? specialEventItemCount : standardItemCount);
        DisplayItems(specialEvent ? specialEventItemCount : standardItemCount);
    }

    public void HideItemSelection()
    {
        itemSlotPanel.SetActive(false);
        currentItems.Clear();
        FindObjectOfType<TankController>().SetPaused(false);

        if (descriptionText != null)
        {
            descriptionText.text = "";
        }

        // Show the post-wave options panel
        postWavePanel.SetActive(true);
    }

    public void ItemPicked()
    {
        itemsPickedCount++;
        if (itemsPickedCount >= maxItemsToPick)
        {
            HideItemSelection();
        }
    }

    private void GenerateItems(int itemCount)
    {
        currentItems.Clear();

        Item healthItem = GetRandomItemOfCategory(ItemCategory.Health);
        if (healthItem != null)
        {
            currentItems.Add(healthItem);
        }

        for (int i = 1; i < itemCount; i++)
        {
            Item selectedItem = GetUniqueRandomItem();
            if (selectedItem != null)
            {
                currentItems.Add(selectedItem);
            }
        }
    }

    private void DisplayItems(int itemCount)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < itemCount && i < currentItems.Count)
            {
                itemSlots[i].SetActive(true);
                ItemSlot itemSlot = itemSlots[i].GetComponent<ItemSlot>();
                itemSlot.SetItem(currentItems[i], this);
                itemSlot.descriptionText = descriptionText;
            }
            else
            {
                itemSlots[i].SetActive(false);
            }
        }
    }

    private Item GetUniqueRandomItem()
    {
        List<Item> availableItems = new List<Item>(itemOptions);

        ItemRarity rarity = GetNextItemRarity();

        List<Item> filteredItems = availableItems.FindAll(item => item.rarity == rarity);

        if (filteredItems.Count == 0)
        {
            filteredItems = availableItems.FindAll(item => item.rarity == ItemRarity.Common);
        }

        return filteredItems.Count > 0 ? filteredItems[Random.Range(0, filteredItems.Count)] : null;
    }

    private ItemRarity GetNextItemRarity()
    {
        float roll = Random.value;

        if (roll < uncommonChance)
        {
            return ItemRarity.Uncommon;
        }
        else if (roll < uncommonChance + rareChance)
        {
            ResetRarityChances();
            return ItemRarity.Rare;
        }
        else if (roll < uncommonChance + rareChance + legendaryChance)
        {
            ResetRarityChances();
            return ItemRarity.Legendary;
        }
        else
        {
            IncreaseRarityChances();
            return ItemRarity.Common;
        }
    }

    private void ResetRarityChances()
    {
        rareChance = 0.01f;
        legendaryChance = 0f;
    }

    private void IncreaseRarityChances()
    {
        rareChance += 0.02f;
        legendaryChance += 0.01f;
    }

    private Item GetRandomItemOfCategory(ItemCategory category)
    {
        List<Item> categoryItems = new List<Item>(itemOptions);
        categoryItems.RemoveAll(item => item.category != category);
        return categoryItems.Count > 0 ? categoryItems[Random.Range(0, categoryItems.Count)] : null;
    }

    private void OpenShop()
{
    shopPanel.SetActive(true);
    postWavePanel.SetActive(false); // Hide the post-wave panel
    FindObjectOfType<TankController>().SetPaused(true);

    // Only restock the shop if 3 waves have passed since last restock
    if (waveCounterForShopRestock >= 3)
    {
        PopulateShopItems(); // Restock shop items
        waveCounterForShopRestock = 0; // Reset the counter after restocking
    }
}

    private void CloseShop()
    {
        shopPanel.SetActive(false);
        postWavePanel.SetActive(true); // Return to post-wave panel
        FindObjectOfType<TankController>().SetPaused(false);
    }

    private void PopulateShopItems()
{
    // Clear previous items
    shopItems.Clear();

    // Generate 7 new items with varying prices for the shop
    for (int i = 0; i < shopItemSlots.Length; i++)
    {
        // Get a random item and calculate its price based on rarity
        Item item = GetUniqueShopItem();
        int price = CalculateItemPrice(item.rarity);

        // Apply a 50% discount to the sale item in slot 7
        if (i == 6) price /= 2;

        // Add item to shop items and set up slot UI
        shopItems.Add(item);
        SetShopItemSlot(i, item, price);
    }
}

    private Item GetUniqueShopItem()
{
    // Use itemOptions for shop items
    List<Item> availableShopItems = new List<Item>(itemOptions); // Full item pool

    ItemRarity rarity = GetNextItemRarity();
    List<Item> filteredItems = availableShopItems.FindAll(item => item.rarity == rarity);

    // Default to common if no match found
    if (filteredItems.Count == 0)
        filteredItems = availableShopItems.FindAll(item => item.rarity == ItemRarity.Common);

    return filteredItems[Random.Range(0, filteredItems.Count)];
}
    private int CalculateItemPrice(ItemRarity rarity)
    {
        // Price range based on item rarity
        switch (rarity)
        {
            case ItemRarity.Common: return Random.Range(30, 50);
            case ItemRarity.Uncommon: return Random.Range(60, 90);
            case ItemRarity.Rare: return Random.Range(150, 250);
            case ItemRarity.Legendary: return Random.Range(400, 600);
            default: return 50;
        }
    }

    private void SetShopItemSlot(int slotIndex, Item item, int price)
{
    // Find ShopItemSlot component in each shop item slot
    var shopItemSlot = shopItemSlots[slotIndex].GetComponent<ShopItemSlot>();
    
    if (shopItemSlot != null)
    {
        // Determine if this is the sale item (slot 7 in the array)
        bool isOnSale = slotIndex == 6;

        // Set item in slot with sale status
        shopItemSlot.SetItem(item, price, isOnSale, this);
    }
    else
    {
        Debug.LogWarning($"Shop item slot {slotIndex} does not have a ShopItemSlot component attached.");
    }
}

    public void DisableGameActions()
{
    // Stop all spawners
    foreach (var spawner in enemySpawners)
    {
        spawner.enabled = false;
    }

    // Destroy all active enemies
    Enemy[] activeEnemies = FindObjectsOfType<Enemy>();
    foreach (var enemy in activeEnemies)
    {
        Destroy(enemy.gameObject);
    }

    postWavePanel.SetActive(false);
    shopPanel.SetActive(false);
}

public void ResetSpawners()
{
    // Reactivate all spawners for the new game
    foreach (var spawner in enemySpawners)
    {
        spawner.enabled = true;
    }

    // Reset wave counter and item selection UI
    currentWave = 0;
    waveCounterForShopRestock = 0;
    PopulateShopItems(); // Repopulate shop with new items
}

    

}
