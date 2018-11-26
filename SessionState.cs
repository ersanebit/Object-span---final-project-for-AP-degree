using System.Collections.Generic;
using System.Linq;
using Guid = System.Guid;

public class SessionState
{
    public int difficulty = 1;
    public string userId; // this will come from backend later
    public int levelFailsInARow = 0; // set to 0 whenever you win level
    public Guid sessionGuid;
    public List<LevelState> levelStates = new List<LevelState>();

    public LevelState GetCurrentLevelState()
    {
        return levelStates.LastOrDefault();
    }

    public LevelState CreateNewLevelState()
    {
        var levelState = new LevelState();
        levelStates.Add(levelState);
        return levelState;
    }
}
