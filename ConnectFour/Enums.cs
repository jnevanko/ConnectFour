namespace ConnectFour;

public enum HoleStatus
{
    Empty,
    Red,
    Black
}

public enum GameStatus
{
    RedsTurn,
    BlacksTurn,
    RedWins,
    BlackWins,
    Tie
}

public enum GameMode
{
    OnePlayer,
    TwoPlayer
}