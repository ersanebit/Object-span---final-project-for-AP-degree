/// Ersan - 150918 - This class contains the data for every level such as: solution(which is the sequence of digits shown on the screen) , user`s input, the digit duration, 
/// a condition for allowing the user to input after the displaying of the solution has ended, the game mode, the time the level started, and the time the user finished the input

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using DateTime = System.DateTime;

public class LevelState
{
    public List<int> solution = new List<int>();
    public List<int> input = new List<int>();
    public float digitDuration = 1f;
    public bool levelSuccess = false;
    public bool allowUserInput = false;
    public GameMode gameMode = GameMode.Forwards;
    public DateTime startedTimestamp = DateTime.UtcNow;
    public DateTime endedTimestamp = DateTime.UtcNow;

	override public string ToString() {
		// TODO: Implement this for debugging purposes.
		return "LevelState.ToString() not implemented yet";
	}

}