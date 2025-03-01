using TMPro;
using UnityEngine;
using UnityEngine.Audio;


//Main functionality for the player. Why is it called player barrel controller? Becuase I am dumb and dont know how to name things properlly.
public class PlayerBarrel_Controller : MonoBehaviour
{
    public static PlayerBarrel_Controller instance;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public GameObject gameOverPanel;
    public GameObject youWinPanel;
    public SpriteRenderer playerSprite;
    public Color barrierColor;
    public Color baseColor;
    public bool barrierActive;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI answerStreakText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI problemsSolvedWinText;
    public TextMeshProUGUI finalScoreWinText;
    public TextMeshProUGUI totalProblemsSolved;
    public bool alreadyPlayed = false; //prevents maxMultiplierSound from playing repeatedly

    //[SerializeField]
    [Header("Player Score Information")]
    public int ammo = 0;
    public int correctTotalAnswers = 0;
    public float score = 0;
    public float answerStreak = 0; //Increases by increments of .1
    public float answerstreakIncrement = .2f;
    public float prevSoundPitch;
    private Camera mainCamera;
    private Vector3 mousePosition;

    [Header("Audio Clips")]
    public AudioClip buttonClickSound;
    public AudioClip maxAmmoNoise;
    public AudioClip increaseDifficulty;
    public AudioClip maxMultiplierSound;
    public AudioClip multiplierBreakpointSound;
    public AudioClip noAmmoSound;

    //Might want to move these two to the button or problem script
    public AudioClip correctAnswerSound;
    public AudioClip incorrectAnswerSound;

    //Might want to remove this.
    public float[] soundPitches; //adjust some of the sound pitches to make game feel more dynamic

    [Header("Audio Sources")]
    public AudioSource audioSource; //AudioSource for things that deal with the player.
    public AudioSource guiaudioSource; //AudioSource for UI while playing
    
    void Start()
    {
        instance = this;
        barrierActive = false;
        playerSprite = GetComponent<SpriteRenderer>();
        baseColor = playerSprite.color;
        barrierColor = new Color(0f, 255f, 255f, 255f);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        ammoText.text = "Ammo: " + ammo;
        scoreText.text = "Score:" + score;
        answerStreakText.text = "X" + answerStreak;
    }


    void Update()
    {
        /*
         * Gets mouse positon on the screen and converts it to worldCordinates
         * Gets vector from mouse position to player position
         * Gets degrees from x and y component 
         * Takes angles and sets rotation of object
        */

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;
        float rotationZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotation.z = 0;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        //Shoot projectile if player has ammo play a no ammo sound to let the player know they are empty.
        if((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && ammo != 0)
        {
            shootProjectile(rotation, rotationZ);
        }
        else if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && ammo == 0)
        {
            //Change audio Sound
            audioSource.PlayOneShot(noAmmoSound);
        }
    }

    public void shootProjectile(Vector3 direction, float rotation)
    {
        /*
         * Creates a bullet and face it towards the direction of the mouse using SetUp
         * updates ammo and ammoText
         */

        GameObject spawnedBullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
        spawnedBullet.GetComponent<Bullet_Controller>().setUp(direction.normalized, rotation);
        audioSource.PlayOneShot(buttonClickSound);
        ammo--;
        ammoText.text = "Ammo: " + ammo;
    }

    public void correctAnswer()
    {
        ammo++;
        answerStreak += answerstreakIncrement;
        correctTotalAnswers++;

        //Limit ammo to only 10 bullets
        if (ammo > 10)
            ammo = 10;

        //Limit answer streak to only X3
        if (answerStreak >= 3f)
            answerStreak = 3f;

        if (ammo == 10)
        {
            ammoText.text = "Ammo: MAX";
            audioSource.PlayOneShot(maxAmmoNoise);
        }
        else
        {
            ammoText.text = "Ammo: " + ammo;
        }
        answerStreakText.text = "X" + answerStreak.ToString("f1");

        //Might want to make this a method where we can send an audio clip and allow the method to generate a pitch
        float newPitch = soundPitches[Random.Range(0, soundPitches.Length)];

        while (newPitch == prevSoundPitch)
        {
            newPitch = soundPitches[Random.Range(0, soundPitches.Length)];
        }

        prevSoundPitch = newPitch;
        guiaudioSource.pitch = newPitch;
        guiaudioSource.PlayOneShot(correctAnswerSound);


        if (string.Equals("1.0", answerStreak.ToString("f1")) || string.Equals("2.0", answerStreak.ToString("f1")))
        {
            audioSource.PlayOneShot(multiplierBreakpointSound);
        }
        else if (string.Equals("3.0", answerStreak.ToString("f1")) && alreadyPlayed != true)
        {
            audioSource.PlayOneShot(maxMultiplierSound);
            alreadyPlayed = true; //Prevents maxMultiplierSound from playing multiple times if the player retains the X3 streak.
        }

    }

    //Only called if the player gets past multiples of 12
    public void playerWin()
    {
        //Reset stats of the enemies and deactivate spawning enemies
        GameObject enemySpawner = GameObject.Find("EnemyManager");
        GameObject fakeEnemySpawner = GameObject.Find("FakeEnemyManager");
        EnemySpawn_Controller.instance.resetStats();
        FakeEnemySpawn_Controller.instance.resetStats();
        enemySpawner.SetActive(false);
        fakeEnemySpawner.SetActive(false);
        finalScoreWinText.text = "Score:" + score.ToString();
        problemsSolvedWinText.text = "Total problems solved: " + correctTotalAnswers.ToString();

        youWinPanel.SetActive(true);

        //Maybe stop audioSources from playing
        Destroy(gameObject);
    }

    public void incorrectAnswer()
    {
        answerStreak = 0f;
        alreadyPlayed = false;
        answerStreakText.text = "X" + answerStreak;
        guiaudioSource.PlayOneShot(incorrectAnswerSound);
    }

    public void enemyDestroyed()
    {
        if (answerStreak > 0f)
        {
            score += 100 * answerStreak;
        }
        score += 100;

        scoreText.text = "Score: " + score.ToString("f0");
    }

    //Triggers game over sequence if enemy collides with player
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected " + collision.tag);
        //Trigger game over sequence if the player is hit by asteroid
        //If the player is warping and an asteroid collides with them destroy the asteroid
        if(collision.tag == "Enemy" && barrierActive == false)
        {
            GameObject enemySpawner = GameObject.Find("EnemyManager");
            GameObject fakeEnemySpawner = GameObject.Find("FakeEnemyManager");
            EnemySpawn_Controller.instance.resetStats();
            FakeEnemySpawn_Controller.instance.resetStats();
            enemySpawner.SetActive(false);
            fakeEnemySpawner.SetActive(false);
            finalScoreText.text = "Score: " + score.ToString();
            totalProblemsSolved.text = "Total problems solved: " + correctTotalAnswers.ToString();

            //Play sound

            gameOverPanel.SetActive(true);

            //Maybe stop audioSources from playing
            Destroy(gameObject);
        }
        else if(collision.tag == "Enemy" && barrierActive == true)
        {
            if (collision.TryGetComponent<Enemy_Controller>(out Enemy_Controller enemy))
            {
                enemy.death();
            }
            else if (collision.TryGetComponent<FakeEnemy_Controller>(out FakeEnemy_Controller fakeEnemy))
            {
                fakeEnemy.death();
            }
        }
    }
}
