/// Ersan - 150918 - Here is the core functionality for the test. This file contains the logic behind the game mechanics.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Guid = System.Guid;

    
public class SequenceDisplayer : MonoBehaviour
{
	[Header("Config")]
	[SerializeField]
	int maxDifficulty = 5;
	[SerializeField]
	bool conditionForBackwardsGameMode; 
	[SerializeField]
	int reverselevelFailsInARow = 0;    
	[SerializeField]
	int gameOverCount = 0;
	[SerializeField]
	int seed = 0;		// chortorn will implement this or show and tell Ersan how to do it.
    public int successCounter = 0; // how many answers were correct per gameMode

	[Header("Config - Time")]
	[SerializeField]
	float whatWasThisFor = 2f;

	[Header("Bindings")]
    public Image label;
	public Text checkMark;  
    public Text noOfDigits;
    public Text failMessage;
    public Text reverseMessage;
    public Text gameOverMessage;

    [Header("Images")]
    public Sprite Image1;
    public Sprite Image2;
    public Sprite Image3;
    public Sprite Image4;
    public Sprite Image5;
    public Sprite Image6;
    public Sprite Image7;
    public Sprite Image8;
    public Sprite Image9;
    public Sprite Image10;
    public Sprite empty;
    public Sprite x;
    public Sprite v;
    public Sprite[] pics;
    

    SessionState sessionState = new SessionState();

    void ShowNumberOfDigitsPerSequence(SessionState sessionState)
    {
        int numberOfDigits = sessionState.difficulty + 1;
        noOfDigits.text = numberOfDigits.ToString();             
    }

    void Start()
    {
        sessionState.sessionGuid = Guid.NewGuid(); // only once per play
       // Debug.LogFormat(this, "[SequenceDisplayer] Start");
        Debug.LogFormat("Session GUID: {0}", sessionState.sessionGuid);     
        var levelState = sessionState.CreateNewLevelState();
        StartLevel(levelState);

        pics = new Sprite[10];
            
        pics[1] = Image1;
        pics[2] = Image2;
        pics[3] = Image3;
        pics[4] = Image4;
        pics[5] = Image5;
        pics[6] = Image6;
        pics[7] = Image7;
        pics[8] = Image8;
        pics[9] = Image9;
        pics[0] = Image10;
        
    }

    void StartLevel(LevelState levelState)
    {
        ShowNumberOfDigitsPerSequence(sessionState);
        Debug.LogFormat("Level started at: {0}", levelState.startedTimestamp);
        Debug.LogFormat(this, "[SequenceDisplayer] Start Level");
        levelState.input.Clear();
        int solutionLength = GetSolutionLengthForLevel(sessionState.difficulty);
		GenerateSolution(levelState, solutionLength);
		StartCoroutine(DoShowSolutionSequence(levelState));
    }

    int GetSolutionLengthForLevel(int level)
    {
        return level + 1;
    }

    void GenerateSolution(LevelState levelState, int solutionLength)
    {
        Random rand = new Random();
        Debug.LogFormat(this, "[SequenceDisplayer] GenerateSolution({0})", solutionLength);
        levelState.solution.Clear();

        for (int i = 0; i < solutionLength; i++)
        {
            int digit = Random.Range(0, 10);
            if (successCounter > 9) {
                levelState.solution.Add(digit);
                continue;
            }

            else if (levelState.solution.Contains(digit))
            {
                i--;
                continue; // this forces the next iteration to take place, skipping any code in between
            }

            levelState.solution.Add(digit);
        }
    }

    IEnumerator ShowReverseMessage()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Now you ll be redirected to the reverse order game");
        label.sprite = empty;
        reverseMessage.text = "Reverse \nmode";

        Debug.LogFormat(this, "[SequenceDisplayer] Showing solution in reverse order");
        yield return new WaitForSeconds(1f);
        int digitSpanResult = successCounter + 1;

        // Results of the user game
        failMessage.text = "Your digit span is: " + digitSpanResult.ToString();   
        yield return new WaitForSeconds(2f);
        failMessage.text = " ";
        successCounter = 0;
        reverseMessage.text = "";
    }

    IEnumerator ShowCheckmark() // shows a checkmark symbol after user completed the level succesfully 
    {
        checkMark.text = "V";		// temporary implementation, will be changed with picture  
        yield return new WaitForSeconds(1f);
        checkMark.text = "";
    }

    IEnumerator DoShowSolutionSequence (LevelState levelState)
    {
        List<int> copySolution = new List<int>(levelState.solution);
		if (conditionForBackwardsGameMode) 
		{
            yield return new WaitForSeconds(2f);
	        if (sessionState.levelFailsInARow == 0)
	        {
	            Debug.Log("No fails and should be reversed gamemode");
	            gameOverCount = 0;
	            levelState.gameMode = GameMode.Reversed;
	            copySolution.Reverse();
	        }
	        
	        else
	        {
	            if (sessionState.levelFailsInARow == 3) // in case the number of fails in a row is 3, I want to start incrementing the gameOverCount after first play in reverse mode
	            {                                       // if I don`t do this, gameOverCount increments the first time it enters the reversed order without even failing/playing    
	                gameOverCount = 1;
	                levelState.gameMode = GameMode.Reversed;
	                copySolution.Reverse();
	            }
	            else if (sessionState.levelFailsInARow != 3)
	            {
	                gameOverCount++;
	                levelState.gameMode = GameMode.Reversed;
	                copySolution.Reverse();

	                if (gameOverCount == 2)
	                {
                        int digitSpanResult = successCounter + 1;
                        // Results of the user game
                        failMessage.text = "Your digit span is: " + digitSpanResult.ToString();
                        label.sprite = empty;
                        gameOverMessage.text = "Game\nOver";
                        yield return new WaitForSeconds(2f);
	                    Debug.Log("The end");
	                    ExitApplication();
	                }
	            }
	        }
		}     

        Debug.LogFormat(this, "[SequenceDisplayer] Show solution sequence coroutine, game mode: {0}", levelState.gameMode);
        SetAllowUserInput(levelState, false); // Do not allow user input while we show the sequence
        yield return new WaitForSeconds(whatWasThisFor);
        foreach (var digit in copySolution)
        {
            ShowDigit(digit); // Show a single digit from the solution
            yield return new WaitForSeconds(levelState.digitDuration); // Wait until showing next digit
        }
        ClearDigitLabel(); // Dont show the last digit after the wait
        SetAllowUserInput(levelState, true); // After showing the solution, allow user input again
    }



    void ShowDigit(int digit)   
    {
        label.sprite = pics[digit];
        Debug.LogFormat(this, "[SequenceDisplayer] ShowDigit: {0}", digit);
    }

    void ClearDigitLabel()
    {
        label.sprite = empty;
        // Debug.LogFormat(this, "[SequenceDisplayer] Clear Digit Label");
    }

    void SetAllowUserInput(LevelState levelState, bool inputAllowed)
    {
        levelState.allowUserInput = inputAllowed;
      //  Debug.LogFormat(this, "[SequenceDisplayer] ChangeUserInput: Allowed = {0}", levelState.allowUserInput);
    }

    public void HandleDigitPressed(int digitPressed)
    {
        LevelState levelState = sessionState.GetCurrentLevelState();
        if (levelState == null || levelState.allowUserInput == false)
        {
         //   Debug.LogFormat(this, "[SequenceDisplayer] Blocked Press Digit, user input is not allowed!");
            return; // User not allowed to press yet
        }
        PressDigit(levelState, digitPressed);
    }

    public void PressDigit(LevelState levelState, int digitPressed)
    {
        string solutionString = string.Join("", levelState.solution.Select(d => d.ToString()).ToArray());
        string inputString = string.Join("", levelState.input.Select(d => d.ToString()).ToArray());
       // Debug.LogFormat(this, "Solution: {0}, Input: {1}", solutionString, inputString);

        // the stored sequences from the solutions
        string solutionStoredString = string.Join("", levelState.solution.Select(d => d.ToString()).ToArray());
        //Debug.Log(solutionStoredString);

        levelState.input.Add(digitPressed);
        
      //  Debug.LogFormat(this, "[SequenceDisplayer] Press digit: {0}", digitPressed);
        if (RequireAdditionalInputDigits(levelState) == false)
        {
            bool solutionIsCorrect = CheckSolution(levelState);

            if (solutionIsCorrect)
            {
                // Transforms the lists of integers into arrays of strings (so that we can use string.Join() to collapse list into a single string)
                // [1,2,3,4] => "1234"

                Debug.LogFormat(this, "[SequenceDisplayer] Input is correct! Solution: {0}, Input: {1}", solutionString, inputString);
                StartCoroutine(ShowCheckmark());
                LevelSuccess(levelState);
            }
            else
            {
                LevelFail(levelState);
            }
        }
    }

 
    void LevelFail(LevelState levelState)
    {
        levelState.endedTimestamp = System.DateTime.UtcNow;
       
        // first try on this level
        sessionState.levelFailsInARow++;

        if (sessionState.levelFailsInARow >= 2)
        {
            if (sessionState.levelFailsInARow == 2 && !conditionForBackwardsGameMode)
            {
                StartCoroutine("ShowReverseMessage");
                sessionState.difficulty = 1; // once the player goes to reverse mode, we want them to start from first level with 2 digits
            }
            conditionForBackwardsGameMode = true;
           
        }
        

        Debug.Log("Fail.");
        Debug.LogFormat("Level ended at: {0}", levelState.endedTimestamp);
        var nextLevelState = sessionState.CreateNewLevelState(); 
        StartLevel(nextLevelState);
    } 
     

    bool RequireAdditionalInputDigits(LevelState levelState)
    {
        if (levelState.input.Count < levelState.solution.Count) // We havent given enough input digits yet
        {
         //   Debug.Log("Not enough input digits.");
            return true; // We need more
        }
        return false;
    }


    IEnumerator showXmark()  // show "X" mark after a failed level
    {
        label.sprite = x;
        yield return new WaitForSeconds(1f);
        label.sprite = empty;
    }

    bool CheckSolution(LevelState levelState)
    {
        if (levelState.solution.Count != levelState.input.Count) // Solution and input arent even the same length
        {
            Debug.Log("Not match.");
            return false; // They can never match   // record time till finish input
        }
        
        for (int i = 0; i < levelState.solution.Count; i++)
        {
            int solutionDigit = levelState.solution[i];
            int inputDigit = levelState.input[i];
            if (inputDigit != solutionDigit) // Input digit doesnt match solution digit
            {
                // Debug.Log("Input does not match the solution.");
                StartCoroutine(showXmark());
                return false; // Input doesnt match
            }
        }
        return true;
    }

    void LevelSuccess(LevelState levelState)
    {
        levelState.endedTimestamp = System.DateTime.UtcNow;
        levelState.levelSuccess = true;
        
        successCounter++;
        Debug.LogFormat("Level ended at: {0}", levelState.endedTimestamp);
        Debug.LogFormat(this, "[SequenceDisplayer] Level Success");
        SetAllowUserInput(levelState, true);
        sessionState.difficulty++; // Advance to next level
        
		sessionState.levelFailsInARow = 0;

        var nextLevelState = sessionState.CreateNewLevelState();
        StartLevel(nextLevelState); // Start the next level
    }

    public void ExitApplication()
    {
        Application.Quit();
		#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
		#endif
    }
}
