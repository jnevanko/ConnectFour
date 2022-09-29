namespace ConnectFour;

public class Statistics
{
    private class Stat<TYPE>
    {
        public delegate TYPE ParseDelegate(string str);

        private Settings m_Settings = null;
        private ParseDelegate m_Parse = null;
        private TYPE m_OnePlayerValue;
        private TYPE m_TwoPlayerValue;

        public Stat(string name, Settings settings, ParseDelegate parse)
        {
            Name = name;
            m_Settings = settings;
            m_Parse = parse;
        }

        public string Name { get; private set; }

        public TYPE Value
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
                }
                else
                {
                    m_TwoPlayerValue = value;
                }
            }
        }

        public void Load(string[] data)
        {
            if (data[0] == Name)
            {
                m_OnePlayerValue = m_Parse(data[1]);
            }
        }

        public string[] GetData()
        {
            return new string[] { Name, m_OnePlayerValue.ToString() };
        }

        public override string ToString()
        {
            return string.Join(",", GetData());
        }
    }

    private Stat<ulong> m_YourRedWins = null;
    private Stat<ulong> m_PcBlackWins = null;
    private Stat<ulong> m_Ties = null;
    private Stat<ulong> m_Hints = null;
    private Stat<ulong> m_Undos = null;
    private Stat<ulong> m_Clears = null;

    public Statistics(Settings settings)
    {
        m_YourRedWins = new Stat<ulong>("Win", settings, ulong.Parse);
        m_PcBlackWins = new Stat<ulong>("Loss", settings, ulong.Parse);
        m_Ties = new Stat<ulong>("Tie", settings, ulong.Parse);
        m_Hints = new Stat<ulong>("Hint", settings, ulong.Parse);
        m_Undos = new Stat<ulong>("Undo", settings, ulong.Parse);
        m_Clears = new Stat<ulong>("Clear", settings, ulong.Parse);
    }

    public Statistics(Settings settings, string[] statStrings)
        : this(settings)
    {
        foreach (string statString in statStrings)
        {
            string[] valueStrings = statString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            // NOTE: Will only load if the name string matches the stat
            m_YourRedWins.Load(valueStrings);
            m_PcBlackWins.Load(valueStrings);
            m_Ties.Load(valueStrings);
            m_Hints.Load(valueStrings);
            m_Undos.Load(valueStrings);
            m_Clears.Load(valueStrings);
        }
    }

    public ulong YourRedWins
    {
        get { return m_YourRedWins.Value; }
        set { m_YourRedWins.Value = value; }
    }

    public ulong PcBlackWins
    {
        get { return m_PcBlackWins.Value; }
        set { m_PcBlackWins.Value = value; }
    }

    public ulong Ties
    {
        get { return m_Ties.Value; }
        set { m_Ties.Value = value; }
    }

    public ulong Hints
    {
        get { return m_Hints.Value; }
        set { m_Hints.Value = value; }
    }

    public ulong Undos
    {
        get { return m_Undos.Value; }
        set { m_Undos.Value = value; }
    }

    public ulong Clears
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

    public string[][] GetData()
    {
        string[][] data = new string[6][];

        data[0] = m_YourRedWins.GetData();
        data[1] = m_PcBlackWins.GetData();
        data[2] = m_Ties.GetData();
        data[3] = m_Hints.GetData();
        data[4] = m_Undos.GetData();
        data[5] = m_Clears.GetData();

        return data;
    }
}
