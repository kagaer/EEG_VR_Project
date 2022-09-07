using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    // get the other scripts
    //public DisplayText displayText;
    public WriteCSV writeCSV;
    //public SelectionManager selectionManager;

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

    //list of conditions & positions for the avatars
    private List<string> emotions = new List<string>() { "angry", "afraid", "happy", "sad" };
    private List<string> position = new List<string>() { "left", "right" };

    //to randomize the trial and save data
    System.Random random = new System.Random();

    private List<string> _data = new List<string>();
    private string _header = "BlockNumber, TaskNumber, EmotionalExpression, PositionExpression, ReactionTime, ResponsePosition";

    public string response;
    private int currCond;
    private int currPos;

    // reaction time
    private float _startTrial;
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
        // show introduction text
        string introText = text1 + "\n" + text2 + "\n" + "\n" + text3;
        DisplayInstructions(introText, 36);
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
            //selectionManager.hasStarted = false;
            //selectionManager.inTrial = false;

            _breakInterval = breakInterval; // reset isi time
            nrBlocks -= 1; // count down nr. of blocks
            _nrTrials = nrTrials; // reset nr. of trials

        }
        // Press Space to start isi (beginning and after block)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            breakCurr = true;
            //selectionManager.hasStarted = true;
            //selectionManager.inTrial = true;
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

                _startTrial = Time.realtimeSinceStartup;
                writeCSV.cnt += 1;

                //restart the trials
                randomizeTrials();
                switchTrials();

                //reset the selectionSettings
                //selectionManager.objectSelected = false;
                //selectionManager.inTrial = true;

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

                //selectionManager.inTrial = false; //reset selection (color)

                response = "noResponse";
                _rt = stimInterval;
                SaveTrialResponses();
            }
            // otherwise, if you answer, process the response
            //TODO: is this the best way to do it? is RT affected, because the click is registered in the selection manager?
            //else if (selectionManager.objectSelected)
            //{
            //    _breakInterval = breakInterval; // reset isi time
            //    breakCurr = true;
            //    stimCurr = false;
            //
            //    selectionManager.inTrial = false; //reset selection (color)
            //    resetTrial();
            //
            //    DisplayInstructions("", 100);
            //    response = selectionManager.objectHit; //get the selected object
            //    _rt = Time.realtimeSinceStartup - _startTrial; // reaction time
            //    SaveTrialResponses(); // save the current response
            //}
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
        Destroy(emotionExp);
        Destroy(neutralExp);
    }

    void randomizeTrials()
    {
        //Randomize Condition
        currCond = random.Next(emotions.Count);

        //Randomize Position
        currPos = random.Next(position.Count);

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
                    emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Angry", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 2 (afraid)
            case 1:
                if (currPos == 0)
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Afraid", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 3 (happy)
            case 2:
                if (currPos == 0)
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Happy", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;
            //emotion 4 (sad)
            case 3:
                if (currPos == 0)
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                }
                //place on EmotionalExpression on right & Neutral on left
                else
                {
                    emotionExp = Instantiate(Resources.Load("Prefabs/Sad", typeof(GameObject)), new Vector3(0.5f, 1, 0), Quaternion.identity) as GameObject;
                    neutralExp = Instantiate(Resources.Load("Prefabs/Neutral", typeof(GameObject)), new Vector3(-1, 1, 0), Quaternion.identity) as GameObject;
                }
                break;


        }
    }

    private void SaveTrialResponses()
    {
        // The following could be written in one line but this makes it easier to see
        // add all the parameters as string to the list
        string currData = (nrBlocks.ToString() + "," + _nrTrials.ToString() + "," + emotions[currCond] + "," + position[currPos] +
            "," + _rt.ToString() + "," + response);

        // add the new list to the list of lists that will become our output csv
        _data.Add(currData);
    }

    private void EndExp()
    {
        writeCSV.MakeCSV(_data, _header); // save Data
        Application.Quit(); // end application
    }
}
