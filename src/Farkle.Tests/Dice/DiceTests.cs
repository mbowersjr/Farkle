using Farkle.Rules.DiceTypes;
using Farkle.Services;
using Moq;


namespace Farkle.Tests.Dice;

public class DiceTests
{
    private DiceManager _diceManager;

    private void Setup() 
    {
        var gameMain = new Mock<GameMain>();
        _diceManager = new DiceManager(gameMain.Object);
    }

    [Fact]
    public void Even_Dice_Rolls_Even()
    {
        //Arrange
        Setup();
        _diceManager.AddDice<EvensDice>(1);

        //Act
        _diceManager.Roll();

        //Assert
        var dice = _diceManager.GetDiceSpritesExact();
        Assert.True((dice[0].Dice.Value % 2) == 0);

    }

    [Fact]
    public void Odds_Dice_Rolls_Odd()
    {
        //Arrange
        Setup();
        _diceManager.AddDice<OddsDice>(1);

        //Act
        _diceManager.Roll();

        //Assert
        var dice = _diceManager.GetDiceSpritesExact();
        Assert.True((dice[0].Dice.Value % 2) != 0);

    }
}

//GameStateManager.ScoreSelectedDice

//ScoringService.CalculateScore(IList<DiceBase> dice)