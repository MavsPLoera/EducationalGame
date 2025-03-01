using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;


public class Problem_Controller : MonoBehaviour
{
    public static Problem_Controller instance;
    public Button[] buttons;
    public GameObject starParticles;
    private ParticleSystem_Controller starController;
    public bool warping = false;

    [Header("Problem Controller UI Text")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI multiplesOfText;
    //public TextMeshProUGUI icreasingDifficultyText; //Optional for added effect
    public int multiplesOf = 1; //Player must solve 10 problems in order to go from multiples of 1 -> 2 -> etc...

    [Header("Incorrect Answer Variables")]
    public float timeTillAnswerAgain = 1.5f;
    public float countdownTime = 0f;
    public bool waitingForTimer = false;
    public Color cantAnswerColor;
    public Color baseColor;
    public AudioClip backOnline2;
    public AudioClip systemAlertSound; //Might want to change this sound
    public AudioSource problemControllerAudioSource;

    //Varibles that are used to generate problems and check solutions
    private List<int> answerChoiceInProblem = new List<int>();
    private List<int> availableNumbers = new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    private List<int> usedNumbers = new List<int>();
    private int rightMostNumber;
    private int solution;

    void Start()
    {
        instance = this;
        starController = starParticles.GetComponent<ParticleSystem_Controller>();
        generateProblem();
        multiplesOfText.text = "Multiples of " + multiplesOf.ToString();
        baseColor = new Color(0f, 255f, 247f, 100f);
        cantAnswerColor = new Color(255f, 0f, 0f, 100f);
        foreach (Button button in buttons)
        {
            button.GetComponent<Image>().color = baseColor;
        }
    }

    void Update()
    {
        //Timer will only countdown if the player gets a problem wrong
        if(waitingForTimer == true)
        {
            countdownTime -= Time.deltaTime;

            /*
             * Once the timer is done allow the player to answer again 
             * and revert buttons colors back to what they were before
             */
            if (countdownTime <= 0f)
            {
                waitingForTimer = false;
                countdownTime = timeTillAnswerAgain;
                changeColor(baseColor);

                //Stop alarm sound from playing and play the systemBackOnline AudioClip
                if(problemControllerAudioSource.isPlaying)
                {
                    problemControllerAudioSource.Stop();
                }
                problemControllerAudioSource.PlayOneShot(backOnline2);
            }
        }
    }

    public void generateProblem()
    {
        //Generate a left number for the equation. We will treat this number as our solution to our problem.
        //Add this number to stack answerChoiceInProblem to make each answer for the buttons unique
        //To prevent duplicate questions we will remove the number generated from avalible numbers and add it to used numbers to remember it
        int leftMostNumber = availableNumbers[Random.Range(0, availableNumbers.Count())];
        answerChoiceInProblem.Add(leftMostNumber);
        availableNumbers.Remove(leftMostNumber);
        usedNumbers.Add(leftMostNumber);

        /*
         * Generate a right number for the equation and multiply the number by the left number to display "# * # = solution. 
         * Store numbers into variables the class has to be able to check when user clicks on a button.
         */
        rightMostNumber = multiplesOf;
        solution = leftMostNumber * rightMostNumber;
        questionText.text = "__ * " + rightMostNumber.ToString() + " = " + solution;


        /*
         * For loop through each button
         * generate a random number
         * if that number is in the stack answerChoiceInProblem generate another random number
         * until the number is unique keep generating
         * if the answer is unique add it to the stack
         */
        int temp;
        bool uniqueAnswer;
        for (int i = 0; i < buttons.Length; i++)
        {
            uniqueAnswer = false;
            temp = Random.Range(1, 13);

            //Do more testing to check if this works.
            while (uniqueAnswer == false)
            {
                if(answerChoiceInProblem.Contains(temp) == false)
                {
                    answerChoiceInProblem.Add(temp);
                    uniqueAnswer = true;
                }
                else
                {
                    temp = Random.Range(1, 13);
                }
            }

            TMP_Text buttonStringTemp = buttons[i].GetComponentInChildren<TMP_Text>();
            buttonStringTemp.text = temp.ToString();
        }

        //Assign a random button with the correct answer
        temp = Random.Range(0, buttons.Length);
        buttons[temp].GetComponentInChildren<TMP_Text>().text = leftMostNumber.ToString();

        //Clear out stack for the next problem.
        answerChoiceInProblem.Clear();

        //if the avalible numbers list is empty repopulate the list with the used numbers and clear it.
        if(!availableNumbers.Any())
        {
            availableNumbers.AddRange(usedNumbers);
            usedNumbers.Clear();
        }
    }

    public void increaseDifficulty()
    {
        multiplesOf++;

        //Ends the game if the multiples of is equal to 13
        //Otherwise will increase the difficulty by calling the spawners to affect the enemies stats and trigger warp sequence
        if (multiplesOf == 13)
        {
            PlayerBarrel_Controller.instance.playerWin();
        }
        else
        {
            timeTillAnswerAgain += .06f;
            multiplesOfText.text = "Multiples of " + multiplesOf.ToString();

            if (EnemySpawn_Controller.instance != null)
                EnemySpawn_Controller.instance.increaseDifficulty();

            starController.swapPhases();

            //Only change the stats of the fake enemies when the main problem number multiple is of 4 
            if (multiplesOf % 4 == 0)
            {
                FakeEnemySpawn_Controller.instance.increaseDifficulty();
            }
        }
    }

    //When user clicks button checkAnswer checks if the submitted button answer * rightMostNumber is equal to the solution.
    private void checkAnswer(int submitted)
    {
        /*
         * If statement logic has to be alittle strange due to the way ParticleSystem_Controller is implemented.
         * We will first check if the user selected the right button if they did increase player score
         * We will then determine if the correctTotalAnswers is a multiple of 10
         * if it is we will ONLY call increaseDifficulty() this is because the particlesystem script calls generate() problem once the warp sequence ends
         * if it is not we will call generate problem.
         * 
         * in the case the player got the problem wrong trigger wrong answer sequence and do not allow the player to answer
         */ 
        if (rightMostNumber * submitted == solution)
        {
            PlayerBarrel_Controller.instance.correctAnswer();

            if (PlayerBarrel_Controller.instance.correctTotalAnswers % 10 == 0 && PlayerBarrel_Controller.instance.correctTotalAnswers != 0)
            {
                increaseDifficulty();
            }
            else
            {
                generateProblem();
            }
        }
        else
        {
            PlayerBarrel_Controller.instance.incorrectAnswer();
            problemControllerAudioSource.PlayOneShot(systemAlertSound);
            countdownTime = timeTillAnswerAgain;
            changeColor(cantAnswerColor);
            waitingForTimer = true;
            generateProblem();
        }
    }

    public void playerClickedButton()
    {
        //Do not allow the player to select an answer if they get one incorrect for a short period of time or if the player is warping
        if (waitingForTimer == true)
        {
            Debug.Log("Waiting on timer");
            return;
        }
        else if(warping == true)
        {
            Debug.Log("Warping dont allow player to hit button");
            return;
        }

        //Uses the event system in unity to retrieve the current selected button name
        //Can also use on pointer click (look this up)
        //Way I implemented the buttons is scuffed because it is brute force but it works I guess
        string answer = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>().text;

        //Convert answer from string to int
        int answerConverted = int.Parse(answer);
        checkAnswer(answerConverted);
    }

    //Will change the color of all the buttons when called
    public void changeColor(Color color)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = color;
        }
    }
}
