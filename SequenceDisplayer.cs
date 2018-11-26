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
    const int MAX_DIFFICULTY = 5;

    public Text label;

    public Text checkMark;
    public bool conditionForBackwardsGameMode;
    public int reverselevelFailsInARow = 0;
    public int gameOverCount = 0;

    SessionState sessionState = new SessionState();

    void Start()
    {
        sessionState.sessionGuid = Guid.NewGuid(); // only once per play
       // Debug.LogFormat(this, "[SequenceDisplayer] Start");
        Debug.LogFormat("Session GUID: {0}", sessionState.sessionGuid);     
        var levelState = sessionState.CreateNewLevelState();
        StartLevel(levelState);
    }


    void StartLevel(LevelState levelState)
    {

      //  Debug.LogFormat("Level started at: {0}", levelState.startedTimestamp);
      //  Debug.LogFormat(this, "[SequenceDisplayer] Start Level");
        levelState.input.Clear();
        int solutionLength = GetSolutionLengthForLevel(sessionState.difficulty);
        GenerateSolution(levelState, solutionLength);
        StartCoroutine(ShowSolutionSequenceCoroutine(levelState));
    }

    int GetSolutionLengthForLevel(int level)
    {
        return level + 1;
    }

    void GenerateSolution(LevelState levelState, int solutionLength)
    {
        Debug.LogFormat(this, "[SequenceDisplayer] GenerateSolution({0})", solutionLength);
        levelState.solution.Clear();

        for (int i = 0; i < solutionLength; i++)
        {
            int digit = Random.Range(0, 10);
            levelState.solution.Add(digit);
        }
    }

    void ShowReverseMessage()
    {
        Debug.Log("Now you ll be redirected to the reverse order game");
        label.text = "Reverse";
        Debug.LogFormat(this, "[SequenceDisplayer] Showing solution in reverse order");
    }

    IEnumerator ShowSolutionSequenceCoroutine(LevelState levelState)
    {
        List<int> copySolution = new List<int>(levelState.solution);
        if (sessionState.levelFailsInARow == 0 && conditionForBackwardsGameMode)
        {
            Debug.Log("No fails and should be backwards gamemode");
            gameOverCount = 0;
            levelState.gameMode = GameMode.Backwards;
            copySolution.Reverse();
        }
        
        else if (sessionState.levelFailsInARow != 0 && conditionForBackwardsGameMode)
        {
            if (sessionState.levelFailsInARow == 3)
            {
                gameOverCount = 0;
                gameOverCount++;
                levelState.gameMode = GameMode.Backwards;
                copySolution.Reverse();
                Debug.Log("2 fails in a row or condition for backwards gamemode");
            }
            else if (sessionState.levelFailsInARow != 3)
            {
                gameOverCount++;
                levelState.gameMode = GameMode.Backwards;
                copySolution.Reverse();
                Debug.Log("2 fails in a row or condition for backwards gamemode");

                if (gameOverCount == 2)
                {
                    label.text = "GameOver";
                    Debug.Log("The end");
                    ExitGame();
                }
            }
        }
        

        Debug.LogFormat(this, "[SequenceDisplayer] Show solution sequence coroutine, game mode: {0}", levelState.gameMode);
        SetAllowUserInput(levelState, false); // Do not allow user input while we show the sequence
        yield return new WaitForSeconds(2f);
        foreach (var digit in copySolution)
        {
            ShowDigit(digit); // Show a single digit from the solution
            yield return new WaitForSeconds(levelState.digitDuration); // Wait until showing next digit
        }
        ClearDigitLabel(); // Dont show the last digit after the wait
        SetAllowUserInput(levelState, true); // After showing the solution, allow user input again
    }

    IEnumerator ShowCheckmark() // shows a checkmark symbol after user completed the level succesfully
    {
        checkMark.text = "V";
        yield return new WaitForSeconds(1f);
        checkMark.text = "";
    }

    void ShowDigit(int digit)   
    {
        label.text = digit.ToString();
        Debug.LogFormat(this, "[SequenceDisplayer] ShowDigit: {0}", digit);
    }

    void ClearDigitLabel()
    {
        label.text = "";
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

    // Bugs to be fixed here, still trying to make the reverse version work
    // **************************************************************************************************
    void LevelFail(LevelState levelState)
    {
        // first try on this level
        sessionState.levelFailsInARow++;

        if (sessionState.levelFailsInARow >= 2)
        {
            if (sessionState.levelFailsInARow == 2)
            {
                ShowReverseMessage();
                sessionState.difficulty = 1; // once the player goes to reverse mode, we want them to start from first level with 2 digits
            }

            conditionForBackwardsGameMode = true;
                       
        }
        

        Debug.Log("Fail.");
        var nextLevelState = sessionState.CreateNewLevelState();
        StartLevel(nextLevelState);
    } 
    // ***************************************************************************************************

    bool RequireAdditionalInputDigits(LevelState levelState)
    {
        if (levelState.input.Count < levelState.solution.Count) // We havent given enough input digits yet
        {
         //   Debug.Log("Not enough input digits.");
            return true; // We need more
        }
        return false;
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
                label.text = "X";
                return false; // Input doesnt match
            }
        }
        return true;
    }

    void LevelSuccess(LevelState levelState)
    {
        Debug.LogFormat(this, "[SequenceDisplayer] Level Success");
        SetAllowUserInput(levelState, true);

        if (sessionState.difficulty != MAX_DIFFICULTY)
        {
            sessionState.difficulty++; // Advance to next level
        }

        sessionState.levelFailsInARow = 0;

        var nextLevelState = sessionState.CreateNewLevelState();
        StartLevel(nextLevelState); // Start the next level
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
