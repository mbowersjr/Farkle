using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using Farkle.Rules.DiceTypes;
using Farkle.Rules.Scoring;

namespace Farkle.Services;

public enum GameState
{
    TurnBegin    = 0,  // Pre-first roll
    RollActive   = 1,  // After roll, before scoring selection
    RollComplete = 2,  // After scoring selection
    TurnEnd      = 3,  // No roll available, end of turn
    GameOver     = 4   // No more turns
}

public class GameStateManager : SimpleGameComponent
{
    private DiceManager _diceManager;
    private InterfaceManager _interfaceManager;
    private InputManager _inputManager;
    private ScoringService _scoringService;
    private readonly GameMain _game;
    public DiceManager DiceManager => _diceManager;
                
    private List<ScoredSet> _scoredSets = new List<ScoredSet>();
    public IList<ScoredSet> ScoredSets => _scoredSets;
    public int Turn { get; set; }

    public GameState CurrentState { get; set; } = GameState.GameOver;

    public GameStateManager(GameMain game)
    {
        _game = game;

        _diceManager = _game.Services.GetService<DiceManager>();
        _interfaceManager = _game.Services.GetService<InterfaceManager>();
        _inputManager = _game.Services.GetService<InputManager>();
        _scoringService = _game.Services.GetService<ScoringService>();
    }

    public IList<ScoredSet> GetScoredSets()
    {
        return _scoredSets.OrderBy(x => x.Turn).ToList();
    }

    private Dictionary<string, Type> _diceTypes = new()
    {
        { "Standard", typeof(StandardDice) },
        { "Lucky", typeof(LuckyDice) },
        { "Evens", typeof(EvensDice) },
        { "Odds", typeof(OddsDice) }
    };

    public Dictionary<string, Type> GetDiceTypes()
    {
        return _diceTypes;
    }

    public void Log(string message)
    {
        _interfaceManager.Log(message);
    }

    public void ClearLog()
    {
        _interfaceManager.ClearLog();
    }

    public bool HasSelectedDice { get; private set; }
    public bool HasAvailableDice { get; private set; }
    public bool HasScoredDice { get; private set; }

    public IList<DiceSprite> GetDiceSprites([NotNull]params DiceState[] states)
    {
        ArgumentNullException.ThrowIfNull(states);
        return _diceManager.GetDiceSprites(states);
    }

    public void Roll()
    {
        _diceManager.Roll();
        CurrentState = GameState.RollActive;
    }

    public int GetTotalScore()
    {
        int score = _scoredSets.Sum(x => x.Score);
        return score;
    }

    public ScoredSet SelectedPossibleScoredSet { get; private set; } = ScoredSet.None;
    public bool UpdateCurrentSelectionCombinations()
    {
        SelectedPossibleScoredSet = ScoredSet.None;
        
        var selectedDice = GetDiceSprites(DiceState.Selected);

        if (selectedDice.Count == 0)
            return false;
        
        var selectedScoredSet = _scoringService.CalculateScore(selectedDice);
        
        if (selectedScoredSet.Combination == ScoredCombination.None)
            return false;

        SelectedPossibleScoredSet = selectedScoredSet;
        return true;
    }

    public void ScoreSelectedDice()
    {
        if (CurrentState != GameState.RollActive)
            throw new InvalidOperationException($"Can only score dice when in {nameof(GameState.RollActive)}");

        var selected = _diceManager.GetDiceSprites(DiceState.Selected);
            
        if (selected.Count == 0)
            return;
        
        var scoredSet = _scoringService.CalculateScore(selected);
        if (scoredSet.Combination == ScoredCombination.None)
        {
            EndTurn();
            return;
        }

        AddScoredSet(scoredSet);

        if (scoredSet.Combination == ScoredCombination.SixOfAKind && scoredSet.Values[0] == 1 && _scoringService.Rules.SixOnesWins)
        {
            GameOver();
            return;
        }

        foreach (var die in selected)
        {
            die.State = DiceState.Scored;
        }

        var stillAvailable = GetDiceSprites(DiceState.Available);
        if (stillAvailable.Count == 0)
        {
            _diceManager.SetAllDiceStates(DiceState.Available);
        }

        CurrentState = GameState.RollComplete;
    }

    public void NewGame()
    {
        ResetDiceStates();
        ResetScoredSets();
        Turn = 1;

        StartTurn();
    }

    public void GameOver()
    {
        if (CurrentState == GameState.GameOver)
            return;

        CurrentState = GameState.GameOver;
        _diceManager.SetAllDiceStates(DiceState.None);
    }

    public void StartTurn()
    {
        CurrentState = GameState.TurnBegin;
        _diceManager.SetAllDiceStates(DiceState.Available);
    }

    public void EndTurn(bool failedTurn = false)
    {
        if (CurrentState == GameState.TurnEnd)
            return;

        if (CurrentState == GameState.RollActive)
        {
            failedTurn = true;
        }

        if (failedTurn)
        {
            _scoredSets.RemoveAll(x => x.Turn == Turn);
        }

        CurrentState = GameState.TurnEnd;
        _diceManager.SetAllDiceStates(DiceState.Scored);
    }

    public void AddScoredSet(ScoredSet scoredSet)
    {
        ArgumentNullException.ThrowIfNull(scoredSet);

        if (scoredSet.Score == 0 || scoredSet.Combination == ScoredCombination.None)
            return;

        scoredSet.Turn = Turn;
        _scoredSets.Add(scoredSet);
    }

    public void ResetScoredSets()
    {
        _scoredSets.Clear();
    }

    public void ResetDiceStates()
    {
        _diceManager.SetAllDiceStates(DiceState.None);
    }

    public override void Initialize()
    {
        _diceManager = _game.Services.GetService<DiceManager>();

        _diceManager.AddDice<StandardDice>(6);

        NewGame();

        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        var currentDice = GetDiceSprites();
        
        HasAvailableDice = currentDice.Any(x => x.Available);
        HasSelectedDice = currentDice.Any(x => x.Selected);
        HasScoredDice = currentDice.Any(x => x.State == DiceState.Scored);
    }
}