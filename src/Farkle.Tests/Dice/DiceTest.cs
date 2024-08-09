using Farkle.Rules.DiceTypes;
using Farkle.Services;
using Moq;


namespace Farkle.Tests.Dice;

public class DiceTest
{
    DiceManager _diceManager;

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
        _diceManager.AddDice(typeof(EvensDice));

        //Act
        _diceManager.Roll();

        //Assert
        var dice = _diceManager.GetDiceSpritesExact();
        Assert.True((dice[0].Dice.Value % 2) == 0);

    }

    [Fact]
    public void Lucky_Dice_Rolls_1s_Or_5s_Statistically_Higher()
    {
        //Arrange
        Setup();
        _diceManager.AddDice(typeof(LuckyDice));
        var diceOutcomes = new List<int>();

        //Act
        for (int i = 0; i < 10000; i++)
        {
            _diceManager.Roll();
            diceOutcomes.Add(_diceManager.GetDiceSpritesExact()[0].Value);
        }

        //Assert
        var luckyRollsCount = diceOutcomes.Where(d => d == 5 || d == 1).Count();
        decimal luckyRollsPercentage = ((luckyRollsCount / 10000) * 100);

        Assert.True(luckyRollsCount > 50);

    }
}

//GameStateManager.ScoreSelectedDice

//ScoringService.CalculateScore(IList<DiceBase> dice)