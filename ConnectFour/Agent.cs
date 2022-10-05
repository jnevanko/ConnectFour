using System.Diagnostics;

namespace ConnectFour;

public class Agent
{
    public const int MIN_DIFFICULTY = 0;
    public const int MAX_DIFFICULTY = 10;
    public const int HINT_DIFFICULTY = 10;

    private Game m_Game = null;
    private Random m_Random = null;

    public Agent(Game game, Random random)
    {
        m_Game = game;
        m_Random = random;
    }

    private class Choice
    {
        public Choice(int column)
        {
            Column = column;
            WinCount = 0;
            TieCount = 0;
        }

        public int Column;
        public int WinCount;
        public int TieCount;
    }

    private List<Choice> GetChoices(Game game)
    {
        List<Choice> choices = new List<Choice>();

        for (int c = 0; c < Game.NUM_COLS; ++c)
        {
            if (game.Hole(Game.NUM_ROWS - 1, c) == HoleStatus.Empty)
            {
                choices.Add(new Choice(c));
            }
        }

        return choices;
    }

    public int ChooseColumn(GameStatus winningStatus, int difficulty)
    {
        List<Choice> choices = GetChoices(m_Game);

        if (choices.Count == 0)
        {
            throw new InvalidOperationException("No columns available to choose");
        }

        if (choices.Count == 1)
        {
            return choices[0].Column;
        }

        int numSamples = 1;

        for (int p = 0; p < difficulty; ++p)
        {
            numSamples *= 2;
        }

        return GetBestChoice(choices, winningStatus, numSamples).Column;
    }

    private Choice GetBestChoice(
        List<Choice> choices,
        GameStatus winningStatus,
        int numSamples)
    {
        Choice bestChoice = null;

        do
        {
            GenerateMultipleSamples(choices, winningStatus, numSamples);
            numSamples /= 2;
            bestChoice = FindBestChoice(choices, numSamples <= 0);
        }
        while (bestChoice == null);

        return bestChoice;
    }

    private void GenerateMultipleSamples(List<Choice> choices, GameStatus winningStatus, int numSamples)
    {
        for (int n = 0; n < numSamples; ++n)
        {
            foreach (Choice choice in choices)
            {
                GenerateSingleSample(choice, winningStatus);
            }
        }
    }

    private void GenerateSingleSample(Choice choice, GameStatus winningStatus)
    {
        Game subGame = new Game(m_Game);

        subGame.InsertChecker(choice.Column);

        GameStatus status = FinishGame(subGame);

        if (status == winningStatus)
        {
            ++choice.WinCount;
        }
        else if (status == GameStatus.Tie)
        {
            ++choice.TieCount;
        }
    }

    private Choice FindBestChoice(List<Choice> choices, bool finalRound)
    {
        Choice bestChoice = null;

        int bestWins = 0;
        int bestTies = 0;

        foreach (Choice choice in choices)
        {
            if (choice.WinCount > bestWins)
            {
                // Best case has only one choice.
                bestWins = choice.WinCount;
                bestChoice = choice;
            }
            else if (bestWins > 0) // Looking for wins, not ties.
            {
                if (choice.WinCount == bestWins)
                {
                    // Best case has multiple choices.
                    bestChoice = null;
                }

                // Otherwise, we have a better choice already.
            }
            else if (choice.TieCount > bestTies) // Looking for ties.
            {
                bestTies = choice.TieCount;
                bestChoice = choice;
            }
            else if (choice.TieCount == bestTies)
            {
                bestChoice = null;
            }
        }

        if (finalRound && bestChoice == null)
        {
            return NarrowChoice(choices, bestWins, bestTies);
        }

        return bestChoice;
    }

    private Choice NarrowChoice(List<Choice> availableChoices, int bestWins, int bestTies)
    {
        List<Choice> bestChoices = new List<Choice>();

        if (bestWins == 0)
        {
            // Basing decision on best number of ties.

            foreach (Choice availableChoice in availableChoices)
            {
                if (availableChoice.TieCount == bestTies)
                {
                    bestChoices.Add(availableChoice);
                }
            }
        }
        else
        {
            // Basing decision on best number of wins.

            foreach (Choice availableChoice in availableChoices)
            {
                if (availableChoice.WinCount == bestWins)
                {
                    bestChoices.Add(availableChoice);
                }
            }
        }

        return bestChoices[m_Random.Next(0, bestChoices.Count)];
    }

    private GameStatus FinishGame(Game game)
    {
        List<int> availableColumns = new List<int>();

        while (game.Status == GameStatus.RedsTurn || game.Status == GameStatus.BlacksTurn)
        {
            availableColumns.Clear();

            for (int c = 0; c < Game.NUM_COLS; ++c)
            {
                if (game.Hole(Game.NUM_ROWS - 1, c) == HoleStatus.Empty)
                {
                    availableColumns.Add(c);
                }
            }

            game.InsertChecker(availableColumns[m_Random.Next(availableColumns.Count)]);
        }

        return game.Status;
    }
}
