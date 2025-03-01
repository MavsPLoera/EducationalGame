using TMPro;
using UnityEngine;

public class ParticleSystem_Controller : MonoBehaviour
{
    public ParticleSystem starsParticles;
    ParticleSystem.MainModule main;
    ParticleSystem.EmissionModule emission;

    public AudioClip barrierActive;
    public AudioSource audioSource;
    public float slowDownTime = 3.0f;
    public float warpTime = 1.0f;
    public float time;
    public float currentTime = 0.0f;
    public bool warpStars = false;
    public bool swappingInProgress = false;
    public int swappingPhase = 2;
    public float startGravityValue;
    public float endGravityValue;
    public float startEmission;
    public float endEmission;

    void Start()
    {
        main = starsParticles.main;
        emission = starsParticles.emission;
    }

    void Update()
    {
        //Will not do anything unless swappingInProgess is set by swapPhases()
        if(swappingInProgress)
        {
            currentTime += Time.deltaTime;

            //if we are in are still swapping the phases call swapPhases again to go to the next phase of the stars we want.
            if (currentTime >= time && swappingPhase != 0)
            {
                swapPhases();
            }

            //LERP
            main.gravityModifier = Mathf.Lerp(startGravityValue, endGravityValue, currentTime / time);
            emission.rateOverTime = Mathf.Lerp(startEmission, endEmission, currentTime / time);


            //Complete the cycle once swappingPhase is 0. once it is set swapping in progress to false and reset the swapping phase back to what it was before calling.
            if(swappingPhase == 0 && currentTime >= time)
            {
                swappingInProgress = false;
                Problem_Controller.instance.warping = false;
                EnemySpawn_Controller.instance.dontSpawn = false;
                FakeEnemySpawn_Controller.instance.dontSpawn = false;
                PlayerBarrel_Controller.instance.barrierActive = false;
                PlayerBarrel_Controller.instance.playerSprite.color = PlayerBarrel_Controller.instance.baseColor;

                Problem_Controller.instance.generateProblem();
                swappingPhase = 2;
            }
        }
    }

    public void swapPhases()
    {
        /*
         * Method is called to be able to "warp" th star particles
         * affects some of the settings on the particle system
         * When a timer is done we swap the particle system settings to what they were before then thats it
         * Probably not the best way to do it but I am learning and lazy :)
         */
        Problem_Controller.instance.warping = true;
        if (!warpStars)
        {
            startGravityValue = 0.0f;
            endGravityValue = 15.0f;
            startEmission = 25.0f;
            endEmission = 250.0f;
            main.startLifetime = 2f;
            time = warpTime;
            audioSource.PlayOneShot(barrierActive);

            //Maybe this is when we can change the gravity of the enemies.
            //Find all the enemies and call a method that will flip a bool and modify the enemies to be able to change their behavior based on bool.
            //Need to also call the problem controller to be able to stop the player from clicking the buttons and change the text and set up next problem on completion.
        }
        else
        {
            startGravityValue = 15.0f;
            endGravityValue = 0.0f;
            startEmission = 250.0f;
            endEmission = 25.0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(5f, 10f);
            time = slowDownTime;
        }

        /*
         * Kind of using a latch like system to be able to know how many times to swap
         * swappingInprogress will be set back to false once we go from
         * 
         * Stars at a stand still -> moving stars -> back to stand still
         * 
         * warpStars gets us the right values we need for the phase and inverses the boolean to get the opposite values in the next call.
         * 
         * we will keep track of what phase we are in using swappingPhase. 0 = Done, 1 = warp.
         * 
         * once this is equal to 0 we then know that we are done.
         */
        Problem_Controller.instance.questionText.text = "Commence Warp";
        EnemySpawn_Controller.instance.dontSpawn = true;
        FakeEnemySpawn_Controller.instance.dontSpawn = true;
        PlayerBarrel_Controller.instance.barrierActive = true;
        PlayerBarrel_Controller.instance.playerSprite.color = PlayerBarrel_Controller.instance.barrierColor; //Added to prevent the player from dying during warp

        currentTime = 0.0f;
        swappingInProgress = true;
        warpStars = !warpStars;
        swappingPhase--;

        for (int i = 0; i < Problem_Controller.instance.buttons.Length; i++)
        {
            Problem_Controller.instance.buttons[i].GetComponentInChildren<TMP_Text>().text = " ";
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.TryGetComponent<Enemy_Controller>(out Enemy_Controller op))
            {
                op.changeMovement = true;
            }
            else if (enemy.TryGetComponent<FakeEnemy_Controller>(out FakeEnemy_Controller op1))
            {
                op1.changeMovement = true;
            }
        }
    }
}
