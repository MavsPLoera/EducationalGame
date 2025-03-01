using JetBrains.Annotations;
using UnityEngine;

public class FakeEnemySpawn_Controller : MonoBehaviour
{
    /*
     * Fake enemies and real enemies function the same
     * but instead of moving to an inverse location the enemy will move to a set location
     * The implementation for fake and real enemies is scuffed but that is because
     * I am great programer! :)
     */

    public static FakeEnemySpawn_Controller instance;
    public GameObject[] spawnLocations;
    public GameObject fakeEnemy;
    public bool dontSpawn = false;
    public float timeToSpawn = 2.0f;
    public float countDownTime = 0f;

    [System.NonSerialized]
    public FakeEnemy_Controller fakeenemy_controller;

    //Be VERY CAREFUL with these values for difficulty increasing
    [Header("Increased Diccifulty FakeEnemy Values")]
    public float increasedDifficultyEnemySpawnerRadius = .02f;
    public float increasedDifficultyEnemySpawnTime = .2f;
    public float increasedDifficultydEnemyMoveSpeed = .08f;
    public float increasedDifficultyEnemyScale = .1f;

    void Start()
    {
        instance = this;
        fakeenemy_controller = fakeEnemy.GetComponent<FakeEnemy_Controller>();

        //Reset stats is a band-aid solution to enemies remembering their increased difficulty stats from the previous play
        //Most likely has to do with me changing the script stats so the values never went back to how they were before.
        resetStats();
    }

    void Update()
    {
        countDownTime -= Time.deltaTime;

        if (countDownTime <= 0f && dontSpawn == false)
        {
            countDownTime = timeToSpawn;

            int whichSpawn = Random.Range(0, spawnLocations.Length);
            spawnEnimies(spawnLocations[whichSpawn]);
        }
    }

    public void increaseDifficulty()
    {
        timeToSpawn -= increasedDifficultyEnemySpawnTime;

        
        //Change fake enemy stats
        //Decide not to change fake enemy stats and left them as is.

    }

    public void resetStats()
    {
        timeToSpawn = 2.0f;
    }

    public void spawnEnimies(GameObject spawnLocation)
    {
        Instantiate(fakeEnemy, spawnLocation.transform.position, Quaternion.identity).GetComponent<FakeEnemy_Controller>();
    }
}
