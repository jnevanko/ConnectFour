namespace ConnectFour;

public partial class MainPage : ContentPage
{
    Settings m_Settings = new Settings();
    readonly Game m_Game = new Game();
    Statistics m_Statistics;
    Agent m_Agent;

	public MainPage()
	{
        m_Statistics = new Statistics(m_Settings);
        m_Agent = new Agent(m_Game, new Random());

        InitializeComponent();

        GridDrawing drawing = GridView.Drawable as GridDrawing;

        drawing.Game = m_Game;

        UpdateSettingsView();
        UpdateStatusView();
        UpdateStatisticsView();
    }

    private void HandleGridViewSizeChanged(object sender, EventArgs e)
    {
        GridDrawing drawing = GridView.Drawable as GridDrawing;

        drawing.Resize((float)GridView.Width, (float)GridView.Height);
    }

    private async void HandleGridViewTapped(object sender, TappedEventArgs e)
	{
        ClearHint();

        Point? point = e.GetPosition(GridView);

        if (!point.HasValue)
        {
            return;
        }
        GridDrawing drawing = GridView.Drawable as GridDrawing;

        int columnIndex = drawing.TapToColumnIndex(point.Value);

        if (await InsertChecker(columnIndex) && m_Settings.IsOnePlayer)
        {
            await InsertChecker(m_Agent.ChooseColumn(GameStatus.BlackWins, m_Settings.Difficulty));
        }
    }

    private async void HandleChangeMode(object sender, EventArgs e)
    {
        string modeString = m_Settings.IsOnePlayer ? "two" : "one";

        if (!await DisplayAlert(
                "Change Mode?",
                $"Would you like to switch to {modeString} player mode?",
                "Yes",
                "No"))
        {
            return;
        }

        NewGame();
        m_Settings.IsOnePlayer = !m_Settings.IsOnePlayer;
        UpdateAllViews();
    }

    private void HandleUndo(object sender, EventArgs e)
    {
        Undo();
    }

    private void HandleHint(object sender, EventArgs e)
    {
        if (m_Game.Status == GameStatus.RedsTurn ||
            (m_Game.Status == GameStatus.BlacksTurn && !m_Settings.IsOnePlayer))
        {
            ++m_Statistics.Hints;

            StatusLabel.Text = "Wait";  // TODO

            ShowHint();
        }
    }

    private async void HandleClearGrid(object sender, EventArgs e)
    {
        if (!await DisplayAlert(
                "Clear Grid?",
                $"Would you like to clear the grid?",
                "Yes",
                "No"))
        {
            return;
        }

        NewGame();
        UpdateAllViews();
    }

    private async void HandleResetStats(object sender, EventArgs e)
    {
        if (!await DisplayAlert(
                "Reset Statistics?",
                $"Would you like to reset statistics?",
                "Yes",
                "No"))
        {
            return;
        }

        m_Statistics.Reset();
        UpdateStatisticsView();
    }

    private async Task<bool> InsertChecker(int column)
    {
        if (!m_Game.InsertChecker(column))
        {
            // No checkers added, hence no state change.
            return false;
        }

        GridView.Invalidate();

        string popupText = "";
        bool endOfGame = true;

        switch (m_Game.Status)
        {
            case GameStatus.RedWins:
                if (m_Settings.IsOnePlayer)
                {
                    popupText = "Congrats! You win.";
                }
                else
                {
                    popupText = "Red wins!";
                }
                ++m_Statistics.YourRedWins;
                break;

            case GameStatus.BlackWins:
                if (m_Settings.IsOnePlayer)
                {
                    popupText = "Computer wins.";
                }
                else
                {
                    popupText = "Black wins!";
                }
                ++m_Statistics.PcBlackWins;
                break;

            case GameStatus.Tie:
                popupText = "Tie game.";
                ++m_Statistics.Ties;
                break;

            default:
                endOfGame = false;
                break;
        }

        UpdateStatusView();

        if (endOfGame)
        {
            UpdateStatisticsView();

            bool isOk = await DisplayAlert(
                "Finished game",
                popupText,
                "OK",
                "Cancel");

            if (isOk)
            {
                m_Game.Reset(false);

                if (m_Settings.IsOnePlayer && m_Game.Status == GameStatus.BlacksTurn)
                {
                    m_Game.InsertChecker(m_Agent.ChooseColumn(GameStatus.BlackWins, m_Settings.Difficulty));
                }

                GridView.Invalidate();
                UpdateStatusView();
            }
            else
            {
                switch (m_Game.Status)
                {
                    case GameStatus.RedWins:
                        --m_Statistics.YourRedWins;
                        break;

                    case GameStatus.BlackWins:
                        --m_Statistics.PcBlackWins;
                        break;

                    case GameStatus.Tie:
                        --m_Statistics.Ties;
                        break;
                }

                Undo();
            }
        }

        return !endOfGame;
    }

    private void Undo()
    {
        if (m_Settings.IsOnePlayer && m_Game.AvailableUndoCount < 2)
        {
            return;
        }

        if (m_Game.Undo())
        {
            ++m_Statistics.Undos;

            if (m_Settings.IsOnePlayer && m_Game.Status == GameStatus.BlacksTurn)
            {
                m_Game.Undo();
            }

            UpdateAllViews();
        }
    }

    private void ClearHint()
    {
        GridDrawing drawing = GridView.Drawable as GridDrawing;

        if (drawing.HintColumn >= 0)
        {
            drawing.HintColumn = -1;
            GridView.Invalidate();
        }
    }

    private void ShowHint()
    {
        ClearHint();

        GameStatus winningStatus = GameStatus.RedWins;

        if (m_Game.Status == GameStatus.BlacksTurn)
        {
            winningStatus = GameStatus.BlackWins;
        }

        (GridView.Drawable as GridDrawing).HintColumn =
            m_Agent.ChooseColumn(winningStatus, Agent.HINT_DIFFICULTY);

        UpdateAllViews();
    }

    private void NewGame()
    {
        if (m_Game.AvailableUndoCount > 0)
        {
            ++m_Statistics.Clears;
        }

        ClearHint();
        m_Game.Reset(true);
        GridView.Invalidate();
    }

    private void UpdateSettingsView()
    {
        SettingsLabel.Text =
            m_Settings.IsOnePlayer ?
                "One Player" :
                "Two Player";
    }

    private void UpdateStatusView()
    {
        switch (m_Game.Status)
        {
            case GameStatus.RedsTurn:
                if (m_Settings.IsOnePlayer)
                {
                    StatusLabel.Text = "You";
                }
                else
                {
                    StatusLabel.Text = "Red";
                }
                break;

            case GameStatus.BlacksTurn:
                if (m_Settings.IsOnePlayer)
                {
                    StatusLabel.Text = "Wait";
                }
                else
                {
                    StatusLabel.Text = "Black";
                }
                break;

            case GameStatus.RedWins:
                if (m_Settings.IsOnePlayer)
                {
                    StatusLabel.Text = "You Win!";
                }
                else
                {
                    StatusLabel.Text = "Red Wins!";
                }
                break;

            case GameStatus.BlackWins:
                if (m_Settings.IsOnePlayer)
                {
                    StatusLabel.Text = "Computer Wins";
                }
                else
                {
                    StatusLabel.Text = "Black Wins!";
                }
                break;

            case GameStatus.Tie:
                StatusLabel.Text = "Tie Game";
                break;

            default:
                StatusLabel.Text = "ERROR";
                break;
        }
    }

    private void UpdateStatisticsView()
    {
        if (m_Settings.IsOnePlayer)
        {
            long total = m_Statistics.YourRedWins + m_Statistics.PcBlackWins + m_Statistics.Ties;

            int successRate =
                m_Statistics.YourRedWins == 0 ?
                    0 :
                    (int)(m_Statistics.YourRedWins * 100 / total);

            StatisticsLabel.Text = $"{m_Statistics.YourRedWins} Win, {m_Statistics.PcBlackWins} Loss, {m_Statistics.Ties} Tie, {successRate}% Success";
        } 
        else
        {
            StatisticsLabel.Text = $"{m_Statistics.YourRedWins} Red, {m_Statistics.PcBlackWins} Black, {m_Statistics.Ties} Tie";
        }
    }

    private void UpdateAllViews()
    {
        GridView.Invalidate();
        UpdateSettingsView();
        UpdateStatusView();
        UpdateStatisticsView();
    }
}