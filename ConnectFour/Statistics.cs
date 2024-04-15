using Microsoft.Maui.Storage;
namespace ConnectFour;

public class Statistics
{
    private class Stat
    {
        private Settings m_Settings = null;
        private long m_OnePlayerValue;
        private long m_TwoPlayerValue;

        public Stat(string name, Settings settings)
        {
            Name = name;
            m_Settings = settings;
            m_OnePlayerValue = Preferences.Get(Name, 0L);
        }

        public string Name { get; private set; }

        public long Value
        {
            get
            {
                if (m_Settings.IsOnePlayer)
                {
                    return m_OnePlayerValue;
                }
                else
                {
                    return m_TwoPlayerValue;
                }
            }
            set
            {
                if (m_Settings.IsOnePlayer)
                {
                    m_OnePlayerValue = value;
                    Preferences.Set(Name, value);
                }
                else
                {
                    m_TwoPlayerValue = value;
                }
            }
        }
    }

    private Stat m_YourRedWins = null;
    private Stat m_PcBlackWins = null;
    private Stat m_Ties = null;
    private Stat m_Hints = null;
    private Stat m_Undos = null;
    private Stat m_Clears = null;

    public Statistics(Settings settings)
    {
        m_YourRedWins = new Stat("Win", settings);
        m_PcBlackWins = new Stat("Loss", settings);
        m_Ties = new Stat("Tie", settings);
        m_Hints = new Stat("Hint", settings);
        m_Undos = new Stat("Undo", settings);
        m_Clears = new Stat("Clear", settings);
    }

    public long YourRedWins
    {
        get { return m_YourRedWins.Value; }
        set { m_YourRedWins.Value = value; }
    }

    public long PcBlackWins
    {
        get { return m_PcBlackWins.Value; }
        set { m_PcBlackWins.Value = value; }
    }

    public long Ties
    {
        get { return m_Ties.Value; }
        set { m_Ties.Value = value; }
    }

    public long Hints
    {
        get { return m_Hints.Value; }
        set { m_Hints.Value = value; }
    }

    public long Undos
    {
        get { return m_Undos.Value; }
        set { m_Undos.Value = value; }
    }

    public long Clears
    {
        get { return m_Clears.Value; }
        set { m_Clears.Value = value; }
    }

    public void Reset()
    {
        YourRedWins = 0;
        PcBlackWins = 0;
        Ties = 0;
        Hints = 0;
        Undos = 0;
        Clears = 0;
    }
}
