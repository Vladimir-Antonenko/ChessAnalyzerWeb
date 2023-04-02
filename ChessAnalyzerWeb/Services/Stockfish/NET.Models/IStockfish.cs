﻿namespace ChessAnalyzerApi.Services.Stockfish.NET.Models;

public interface IStockfish
{
    int Depth { get; set; }
    int SkillLevel { get; set; }
    void SetPosition(params string[] move);
    string GetBoardVisual();
    string GetFenPosition();
    void SetFenPosition(string fenPosition);
    string GetBestMove();
    string GetBestMoveTime(int time = 1000);
    bool IsMoveCorrect(string moveValue);
    Evaluation GetEvaluation();
}
