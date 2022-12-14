using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    // get the other scripts
    //public DisplayText displayText;
    public WriteCSV writeCSV;
    public SelectionManager selectionManager;

    // Nr. of blocks --> private var that can be changed in the Inspector
    [SerializeField] private int nrBlocks = 3;

    // Nr. trials in block --> public var that also other scripts can access
    public int nrTrials = 4; // variable will not be updated during the experiment
    private int _nrTrials = 4; // variable to be updated

    // Max. Time of stimulus presentation
    public float stimInterval = 5f; // variable will not be updated during the experiment
    private float _stimInterval = 5f; // variable to be updated
    private bool stimCurr = false;

    //Interstimulua Interval
    public float breakInterval = 3f; // variable will not be updated during the experiment
    private float _breakInterval = 3f; // variable to be updated
    private bool breakCurr = false; // set to True if you are in this interval

    // time at the end of the experiment
    private float _endTime = 2f;
    private bool end = false;

    //prefabs for the emotional expression avatar & the neutral avatar
    GameObject neutralExp;
    GameObject emotionExp;

    //List of all randomization trials
    List<int> allRandomizedCond;
    List<int> allRandomizedPos;
    List<int> allRandomizedNeutralPerson;
    int currentTrial = 0;

    [SerializeField] GameObject neutral1ExpLeft;
    [SerializeField] GameObject neutral2ExpLeft;
    [SerializeField] GameObject happyExpLeft;
    [SerializeField] GameObject angryExpLeft;
    [SerializeField] GameObject fearExpLeft;
    [SerializeField] GameObject sadExpLeft;


    [SerializeField] GameObject neutral1ExpRight;
    [SerializeField] GameObject neutral2ExpRight;
    [SerializeField] GameObject happyExpRight;
    [SerializeField] GameObject angryExpRight;
    [SerializeField] GameObject fearExpRight;
    [SerializeField] GameObject sadExpRight;

    //list of conditions & positions for the avatars
    private List<string> emotions = new List<string>() { "angry", "afraid", "happy", "sad" };
    private List<string> position = new List<string>() { "left", "right" };
    private List<string> neutral = new List<string>() { "young", "old" };

    //to randomize the trial and save data
    System.Random random = new System.Random();

    private List<string> _data = new List<string>();
    private string _header = "BlockNumber; TaskNumber; EmotionalExpression; PositionExpression; NeutralAvatar; ReactionTime; ResponsePosition";

    public string response;
    private int currCond;
    private int currPos;
    private int neutralPerson;

    // reaction time
    //private float _startTrialTime;
    private float _responseTime;
    private float _rt;

    // introduction text
    string text1 = "Welcome to our Experiment.";
    string text2 = "Choose the chair you would rather sit in.";
    string text3 = "Press Space to start.";

    // where text will be displayed
    public TextMeshProUGUI writenCol;

    // Start is called before the first frame update
    void Start()
    {
        int numberCondRepetitions = (nrTrials * nrBlocks) / emotions.Count;
        int numberPosRepetitions = (nrTrials * nrBlocks) / position.Count;
        int numberNeutralPersonRepetitions = (nrTrials * nrBlocks) / neutral.Count;

        allRandomizedCond = CreateList(emotions.Count, numberCondRepetitions);
        allRandomizedPos = CreateList(position.Count, numberPosRepetitions);
        allRandomizedNeutralPerson = CreateList(neutral.Count, numberNeutralPersonRepetitions);
        //Debug.Log("List: " + allRandomizedCond.Count);

        // show introduction text
        string introText = text1 + "\n" + text2 + "\n" + "\n" + text3;
        DisplayInstructions(introText, 36);

        // set all faces invisible
        invisibleEmotions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //end experiment
        if (nrBlocks < 1 && end == false)
        {
            resetTrial();
            DisplayInstructions("Thank you for participating in this experiment!", 30);
            _endTime -= Time.fixedDeltaTime; // count down end time
                                             // end the experiment
            if (_endTime < 0.0)
            {
                end = true;
                EndExp();
            }

        }

        // Or Pause after end of block
        if (_nrTrials < 1 && breakCurr == true && stimCurr == false)
        {
            DisplayInstructions("Time for a pause. Press Space to continue.", 30); // display pause screen
            breakCurr = false; // pause isi
            
            selectionManager.hasStarted = false;
            selectionManager.inTrial = false;

            _breakInterval = breakInterval; // reset isi time
            nrBlocks -= 1; // count down nr. of blocks
            _nrTrials = nrTrials; // reset nr. of trials

        }
        // Press Space to start isi (beginning and after block)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            breakCurr = true;

            selectionManager.hasStarted = true;
            selectionManager.inTrial = true;

            _breakInterval = breakInterval; // reset break time
            DisplayInstructions("", 100);

        }

        //Break
        if (breakCurr == true && stimCurr == false)
        {
            _breakInterval -= Time.fixedDeltaTime; // count down isi time

            // once time is up, switch to trial interval
            if (_breakInterval < 0.0)
            {
                _stimInterval = stimInterval; // reset trial time
                breakCurr = false;
                stimCurr = true;
                _nrTrials -= 1; // count down nr. of trials

                //_startTrialTime = Time.realtimeSinceStartup;
                selectionManager.startTrialTime = Time.realtimeSinceStartup;
                writeCSV.cnt += 1;

                //restart the trials
                randomizeTrials();
                switchTrials();

                //reset the selectionSettings
                selectionManager.objectSelected = false;
                selectionManager.inTrial = true;

            }
        }

        // Trial
        if (stimCurr == true && breakCurr == false)
        {

            _stimInterval -= Time.fixedDeltaTime; // count down trial time
                                                  // once time is up, switch to trial interval
            if (_stimInterval < 0.0)
            {
                _breakInterval = breakInterval; // reset isi time
                breakCurr = true;
                stimCurr = false;
                DisplayInstructions("", 100);
                resetTrial();

                selectionManager.inTrial = false; //reset selection (color)

                response = "noResponse";
                _rt = stimInterval;
                SaveTrialResponses();
            }
            // otherwise, if you answer, process the response
            //TODO: is this the best way to do it? is RT affected, because the click is registered in the selection manager?
            else if (selectionManager.objectSelected)
            {
                _breakInterval = breakInterval; // reset isi time
                breakCurr = true;
                stimCurr = false;
            
                selectionManager.inTrial = false; //reset selection (color)
                resetTrial();
            
                DisplayInstructions("", 100);
                response = selectionManager.objectHit; //get the selected object
                _rt = selectionManager.rt; // reaction time
                SaveTrialResponses(); // save the current response
            }
        }

    }

    private void DisplayInstructions(string text, int size)
    {
        writenCol.text = text;
        writenCol.fontSize = size;
        writenCol.color = new Color(1, 1, 1, 1); // instructions will always be played in white
    }

    void resetTrial()
    {
        invisibleEmotions();
        //Destroy(emotionExp);
        //Destroy(neutralExp);
    }

    // set emotion objects invisible
    void invisibleEmotions()
    {
        neutral1ExpLeft.SetActive(false);
        neutral2ExpLeft.SetActive(false);
        happyExpLeft.SetActive(false);
        angryExpLeft.SetActive(false);
        fearExpLeft.SetActive(false);
        sadExpLeft.SetActive(false);

        neutral1ExpRight.SetActive(false);
        neutral2ExpRight.SetActive(false);
        happyExpRight.SetActive(false);
        angryExpRight.SetActive(false);
        fearExpRight.SetActive(false);
        sadExpRight.SetActive(false);
    }

    void randomizeTrials()
    {
        //Randomize Condition
        currCond = allRandomizedCond[currentTrial];
        //currCond = random.Next(emotions.Count);

        //Randomize Position
        currPos = allRandomizedPos[currentTrial];
        //currPos = random.Next(position.Count);

        //Randomize Neutral Person
        neutralPerson = allRandomizedNeutralPerson[currentTrial];
        //neutralPerson = random.Next(neutral.Count);

        currentTrial++;

    }

    //update the conditions
    void switchTrials()
    {
        switch (currCond)
        {

            //emotion 1 (angry)
            case 0:
                //place EmotionalExpression on left & Neutral on Right
                if (currPos == 0)
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    angryExpLeft.SetActive(true);
                    emotionExp = angryExpLeft;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    angryExpRight.SetActive(true);
                    emotionExp = angryExpRight;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 2 (afraid)
            case 1:
                if (currPos == 0)
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    fearExpLeft.SetActive(true);
                    emotionExp = fearExpLeft;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    fearExpRight.SetActive(true);
                    emotionExp = fearExpRight;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 3 (happy)
            case 2:
                if (currPos == 0)
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    happyExpLeft.SetActive(true);
                    emotionExp = happyExpLeft;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    happyExpRight.SetActive(true);
                    emotionExp = happyExpRight;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 4 (sad)
            case 3:
                if (currPos == 0)
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    sadExpLeft.SetActive(true);
                    emotionExp = sadExpLeft;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    //emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    sadExpRight.SetActive(true);
                    emotionExp = sadExpRight;
                    neutralPersonPosition();

                    //emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    //neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;


        }
    }

    private void neutralPersonPosition()
    {
        // make sure that there is no person twice
        if (currCond == 2) neutralPerson = 1;
        else if (currCond == 3) neutralPerson = 0;


        if (currPos == 0 && neutralPerson == 0)
        {
            neutral1ExpRight.SetActive(true);
            neutralExp = neutral1ExpRight;
        }
        else if (currPos == 0 && neutralPerson == 1)
        {
            neutral2ExpRight.SetActive(true);
            neutralExp = neutral2ExpRight;
        }
        //place on EmotionalExpression on right & Neutral on left
        else if (currPos == 1 && neutralPerson == 0)
        {
            neutral1ExpLeft.SetActive(true);
            neutralExp = neutral1ExpLeft;
        }
        else if (currPos == 1 && neutralPerson == 1)
        {
            neutral2ExpLeft.SetActive(true);
            neutralExp = neutral2ExpLeft;
        }
    }

    private void SaveTrialResponses()
    {
        // The following could be written in one line but this makes it easier to see
        // add all the parameters as string to the list

        string currData = (nrBlocks.ToString() + ";" + _nrTrials.ToString() + ";" + emotions[currCond] + ";" + position[currPos] +
            ";" + neutral[neutralPerson] + ";" + _rt.ToString() + ";" + response);

        // add the new list to the list of lists that will become our output csv
        _data.Add(currData);
    }

    private void EndExp()
    {
        writeCSV.MakeCSV(_data, _header); // save Data
        Application.Quit(); // end application
    }

    private List<int> AddMultiple(List<int> list, int value, int n)
    {
        for(int i = 0; i < n; i++)
        {
            list.Add(value);
        }

        return list; 
    }

    private List<int> CreateList(int numberConditions, int repetitions)
    {
        List<int> list = new List<int>();

        for(int i = 0; i < numberConditions; i++)
        {
            list = AddMultiple(list, i, repetitions);
        }
        list = Shuffle<int>(list);
        return list;
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        //Random rnd = new Random();
        System.Random rnd = new System.Random();
        for (int i = 0; i < list.Count; i++)
        {
            int k = rnd.Next(0, i);
            T value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        return list;
    }

}
