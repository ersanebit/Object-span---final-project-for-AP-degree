                                                                                                                                                                                                                                                                 using System.Collections.Generic;
using DateTime = System.DateTime;
public class LevelState
{
    public List<int> solution = new List<int>();
    public List<int> input = new List<int>();
    public float digitDuration = 1f;
    public bool allowUserInput = false;
    public GameMode gameMode = GameMode.Forwards;
    public DateTime startedTimestamp = DateTime.UtcNow;
    public DateTime endedTimestamp;

    
        
}
                                                                                                                                                                        