using System;

public static class GameEvents
{
    public static event Action<bool> OnGameOver;
    public static void GameOver(bool escape) => OnGameOver?.Invoke(escape);
}
