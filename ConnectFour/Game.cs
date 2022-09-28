namespace ConnectFour;

public class Game
{
    private class UndoState
    {
        public UndoState(int row, int column, GameStatus status)
        {
            Row = row;
            Column = column;
            OldStatus = status;
        }

        public int Row { get; private set; }
        public int Column { get; private set; }
        public GameStatus OldStatus { get; private set; }
    }

    public const int NUM_ROWS = 6;
    public const int NUM_COLS = 7;
    public const int WIN_COUNT = 4;

    private const GameStatus FIRST_STATUS = GameStatus.RedsTurn;
    private int[] ROW_DELTA = new int[] { 1, 0, -1, 1 };
    private int[] COL_DELTA = new int[] { 1, 1, 1, 0 };

    private HoleStatus[,] mHoleStatus = new HoleStatus[NUM_ROWS, NUM_COLS];
    private GameStatus mNextStart;
    private Stack<UndoState> mUndoStack = new Stack<UndoState>();

    public Game()
    {
        Reset(true);
    }

    public Game(Game game)
    {
        Status = game.Status;

        for (int r = 0; r < NUM_ROWS; ++r)
        {
            for (int c = 0; c < NUM_COLS; ++c)
            {
                mHoleStatus[r, c] = game.mHoleStatus[r, c];
            }
        }
    }

    public GameStatus Status { get; private set; }

    public HoleStatus Hole(int row, int col)
    {
        return mHoleStatus[row, col];
    }

    public bool InsertChecker(int column)
    {
        if (column < 0 || NUM_COLS <= column)
        {
            return false;
        }

        HoleStatus newStatus = HoleStatus.Empty;

        switch (Status)
        {
            case GameStatus.RedsTurn:
                newStatus = HoleStatus.Red;
                break;

            case GameStatus.BlacksTurn:
                newStatus = HoleStatus.Black;
                break;

            case GameStatus.RedWins:
            case GameStatus.BlackWins:
            case GameStatus.Tie:
            default:
                return false;
        }

        for (int r = 0; r < NUM_ROWS; ++r)
        {
            if (mHoleStatus[r, column] == HoleStatus.Empty)
            {
                UpdateGameStatus(r, column, newStatus);
                return true;
            }
        }

        return false;
    }

    public int AvailableUndoCount
    {
        get { return mUndoStack.Count; }
    }

    public bool Undo()
    {
        if (mUndoStack.Count < 1)
        {
            return false;
        }

        UndoState undo = mUndoStack.Pop();
        mHoleStatus[undo.Row, undo.Column] = HoleStatus.Empty;
        Status = undo.OldStatus;

        return true;
    }

    public void Reset(bool likeNew)
    {
        mUndoStack.Clear();

        if (likeNew)
        {
            mNextStart = FIRST_STATUS;
        }

        Status = mNextStart;

        if (mNextStart == GameStatus.RedsTurn)
        {
            mNextStart = GameStatus.BlacksTurn;
        }
        else
        {
            mNextStart = GameStatus.RedsTurn;
        }

        for (int r = 0; r < NUM_ROWS; ++r)
        {
            for (int c = 0; c < NUM_COLS; ++c)
            {
                mHoleStatus[r, c] = HoleStatus.Empty;
            }
        }
    }

    private void UpdateGameStatus(int r, int c, HoleStatus holeStatus)
    {
        mUndoStack.Push(new UndoState(r, c, Status));

        mHoleStatus[r, c] = holeStatus;

        if (CheckForWin(r, c, holeStatus))
        {
            switch (holeStatus)
            {
                case HoleStatus.Red:
                    Status = GameStatus.RedWins;
                    return;

                case HoleStatus.Black:
                    Status = GameStatus.BlackWins;
                    return;

                case HoleStatus.Empty:
                default:
                    throw new ArgumentException("Must be a color state");
            }
        }

        if (CheckForFull())
        {
            Status = GameStatus.Tie;
            return;
        }

        switch (holeStatus)
        {
            case HoleStatus.Red:
                Status = GameStatus.BlacksTurn;
                return;

            case HoleStatus.Black:
                Status = GameStatus.RedsTurn;
                return;

            case HoleStatus.Empty:
            default:
                throw new ArgumentException("Must be a color state");
        }
    }

    private bool CheckForWin(int r, int c, HoleStatus state)
    {
        for (int i = 0; i < ROW_DELTA.Length; ++i)
        {
            if (CheckForWin(r, c, ROW_DELTA[i], COL_DELTA[i], state))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckForWin(int ir, int ic, int dr, int dc, HoleStatus state)
    {
        int r = ir - (WIN_COUNT - 1) * dr;
        int c = ic - (WIN_COUNT - 1) * dc;

        int matchCount = 0;

        for (int i = 0; i < 2 * WIN_COUNT - 1; ++i)
        {
            if (ValidRow(r) && ValidColumn(c) && mHoleStatus[r, c] == state)
            {
                if (++matchCount == WIN_COUNT)
                {
                    return true;
                }
            }
            else
            {
                matchCount = 0;
            }

            r += dr;
            c += dc;
        }

        return false;
    }

    private bool CheckForFull()
    {
        for (int c = 0; c < NUM_COLS; ++c)
        {
            if (mHoleStatus[NUM_ROWS - 1, c] == HoleStatus.Empty)
            {
                return false;
            }
        }

        return true;
    }

    private bool ValidRow(int r)
    {
        return 0 <= r && r < NUM_ROWS;
    }

    private bool ValidColumn(int c)
    {
        return 0 <= c && c < NUM_COLS;
    }
}