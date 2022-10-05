namespace ConnectFour;

public class Settings
{
    const string IS_ONE_PLAYER_KEY = "IsOnePlayer";
    const string DIFFICULTY_KEY = "Difficulty";

    bool m_IsOnePlayer;
    int m_Difficulty;

    public Settings()
    {
        m_IsOnePlayer = Preferences.Get(IS_ONE_PLAYER_KEY, true);
        m_Difficulty = Preferences.Get(DIFFICULTY_KEY, 7);
    }

    public bool IsOnePlayer
    {
        get
        {
            return m_IsOnePlayer;
        }
        set
        {
            m_IsOnePlayer = value;
            Preferences.Set(IS_ONE_PLAYER_KEY, value);
        }
    }

    public int Difficulty
    {
        get
        {
            return m_Difficulty;
        }
        set
        {
            m_Difficulty = value;
            Preferences.Set(DIFFICULTY_KEY, value);
        }
    }
}