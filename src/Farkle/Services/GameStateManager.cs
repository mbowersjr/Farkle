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
    TurnActive,
    TurnComplete,
    GameOver
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

    public IList<DiceSprite> GetDiceSprites([NotNull]params DiceState[] states)
    {
        ArgumentNullException.ThrowIfNull(states);
        return _diceManager.GetDiceSprites(states);
    }

    public void Roll()
    {
        if (CurrentState != GameState.TurnActive)
            return;

        _diceManager.Roll();
    }

    public int GetTotalScore()
    {
        int score = _scoredSets.Sum(x => x.Score);
        return score;
    }

    public void ScoreSelectedDice()
    {
        if (CurrentState != GameState.TurnActive)
            throw new InvalidOperationException($"Can only score dice when in {nameof(GameState.TurnActive)}");

        var selected = _diceManager.GetDiceSprites(DiceState.Selected);
            
        if (selected.Count == 0)
            return;
        
        var scoredSet = _scoringService.CalculateScore(selected);
        if (scoredSet.Combination == ScoredCombination.None)
        {
            GameOver();
            return;
        }

        AddScoredSet(scoredSet);

        if (scoredSet.Combination == ScoredCombination.SixOfAKind && scoredSet.Dice[0].Value == 1 && _scoringService.Rules.SixOnesWins)
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
    }

    public void NewGame()
    {
        if (CurrentState != GameState.GameOver)
            return;

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
        if (CurrentState == GameState.TurnActive)
            return;

        CurrentState = GameState.TurnActive;
        _diceManager.SetAllDiceStates(DiceState.Available);
    }

    public void EndTurn()
    {
        if (CurrentState != GameState.TurnActive)
            return;

        CurrentState = GameState.TurnComplete;
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

        ResetDiceStates();
    }

    public void ResetDiceStates()
    {
        _diceManager.SetAllDiceStates(DiceState.Available);
    }

    public override void Initialize()
    {
        _diceManager = _game.Services.GetService<DiceManager>();

        _diceManager.AddDice<StandardDice>(6);


        NewGame();

        base.Initialize();
    }

    //private TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
    //private TimeSpan _updateElapsed = TimeSpan.Zero;
    public override void Update(GameTime gameTime)
    {
        //_updateElapsed += gameTime.ElapsedGameTime;
        //if (_updateElapsed >= _updateInterval)
        //{
        //    _updateElapsed -= _updateInterval;
        //}
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void UnloadContent()
    {            
        base.UnloadContent();
    }     


}