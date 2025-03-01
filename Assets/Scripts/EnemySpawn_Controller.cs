using UnityEngine;

public class EnemySpawn_Controller : MonoBehaviour
{
    public static EnemySpawn_Controller instance;
    private float maxRadiusSpawn = 15f;
    public int maxEnemiesToSpawn = 4;
    public float minimumAngleToSpawn = -15f;
    public float maximumAngleToSpawn;
    public bool dontSpawn = false;

    //Be VERY CAREFUL with these values for difficulty increasing
    [Header("Increased Diccifulty Enemy Values")]
    public float increasedDifficultyEnemySpawnerRadius = .02f;
    public float increasedDifficultyEnemySpawnTime = .2f;
    public float increasedDifficultydEnemyMoveSpeed = .10f;
    public float increasedDifficultyEnemyScale = .1f;

    [Header("EnemyInformation")]
    public GameObject enemy;
    public Enemy_Controller enemy_controller;
    public float spawnTime = 5f;
    private float countDownTime = 2f;

    private void Start()
    {
        maximumAngleToSpawn = 180f + (-minimumAngleToSpawn * 2);
        enemy_controller = enemy.GetComponent<Enemy_Controller>();
        instance = this;

        //Reset stats is a band-aid solution to enemies remembering their increased difficulty stats from the previous play
        //Most likely has to do with me changing the script stats so the values never went back to how they were before.
        resetStats();
    }

    // Update is called once per frame
    void Update()
    {
        //reset the time when dont spawn is set to true IMPORTANT 
        countDownTime -= Time.deltaTime;

        //Spawn enemies once the timer is below or equal to 0. Dont spawn enemies if dontSpawn is True
        if (countDownTime <= 0f && dontSpawn == false)
        {
            countDownTime = spawnTime;

            int temp = 0;
            int numEnimiesToSpawn = Random.Range(1, maxEnemiesToSpawn);
            while (temp != numEnimiesToSpawn)
            {
                spawnEnimies();
                temp++;
            }
        }
    }

    public void spawnEnimies()
    {
        /*
         * We have a radius we want enemies to spawn away from.
         * We generate a rangle angle between minimumAngleToSpawn and maximumAngleToSpawn
         * Convert this angle in radians and we will then get the opposite and adjacent sides for the x and y spawn positon of the enemy.
         * 
         * Do this using right triangles
         * 
         *      |\
         *      | \
         *      |  \  Hyp
         * opp  |   \
         *      |_   \
         *      |_|___\ <- angle we found
         *         adj
         * 
         * opp = hyp * sin(angle)
         * adj = hyp * cos(angle)
         * 
         * Once we get this we can spawn in a enemy and rotate based on these positions
         */
        float randomAngle = Random.Range(minimumAngleToSpawn, maximumAngleToSpawn) * Mathf.Deg2Rad; 
        //float isPositive = Random.Range()
        float xposition = Mathf.Cos(randomAngle) * maxRadiusSpawn;
        float yposition = Mathf.Sin(randomAngle) * maxRadiusSpawn;

        Vector3 enemySpawnLocation = new Vector3(xposition, yposition, 0f);

        //Instantiate enemy at the spawnLocation and rotate enemy twoards the player.
        Instantiate(enemy, enemySpawnLocation, Quaternion.Euler(0f, 0f, Mathf.Atan2(yposition, xposition) * Mathf.Rad2Deg));
    }

    public void increaseDifficulty()
    {
        //Update Enemy Spawner information
        maxRadiusSpawn -= increasedDifficultyEnemySpawnerRadius;
        spawnTime -= increasedDifficultyEnemySpawnTime;

        //Update enemy stats
        enemy_controller.maxMoveSpeed += increasedDifficultydEnemyMoveSpeed;
        enemy_controller.maxScale += increasedDifficultyEnemyScale;
    }

    public void resetStats()
    {
        maxRadiusSpawn = 15f;
        spawnTime = 5f;

        enemy_controller.resetStats();
    }
}
