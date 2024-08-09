using Farkle.Rules.DiceTypes;
using Farkle.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farkle.Tests.Dice
{
    public class DiceManagerTests
    {
        private DiceManager _diceManager;
        private void Setup()
        {
            var gameMain = new Mock<GameMain>();
            _diceManager = new DiceManager(gameMain.Object);
        }

        [Fact]
        public void Dice_Manager_Add_Only_Standard_Dice()
        {
            Setup();

            _diceManager.AddDice<StandardDice>(6);

            foreach (var d in _diceManager.GetDiceSprites())
            {
                Assert.True(d.Dice.GetType() == typeof(StandardDice));
            }
        }

        [Fact]
        public void Dice_Manager_Add_Only_Lucky_Dice()
        {
            Setup();
            _diceManager.AddDice<LuckyDice>(6);

            foreach (var d in _diceManager.GetDiceSprites())
            {
                Assert.True(d.Dice.GetType() == typeof(LuckyDice));
            }
        }

        [Fact]
        public void Dice_Manager_Add_Only_Odds_Dice()
        {
            Setup();
            _diceManager.AddDice<OddsDice>(6);

            foreach (var d in _diceManager.GetDiceSprites())
            {
                Assert.True(d.Dice.GetType() == typeof(OddsDice));
            }
        }

        [Fact]
        public void Dice_Manager_Add_Only_Evens_Dice()
        {
            Setup();
            _diceManager.AddDice<EvensDice>(6);

            foreach (var d in _diceManager.GetDiceSprites())
            {
                Assert.True(d.Dice.GetType() == typeof(EvensDice));
            }
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Lucky_To_Standard() 
        {
            Setup();
            _diceManager.AddDice<LuckyDice>(1);

            _diceManager.ChangeDiceType<StandardDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(StandardDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Lucky_To_Odds()
        {
            Setup();
            _diceManager.AddDice<LuckyDice>(1);

            _diceManager.ChangeDiceType<OddsDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(OddsDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Lucky_To_Evens()
        {
            Setup();
            _diceManager.AddDice<LuckyDice>(1);

            _diceManager.ChangeDiceType<EvensDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(EvensDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Standard_To_Lucky()
        {
            Setup();
            _diceManager.AddDice<StandardDice>(1);

            _diceManager.ChangeDiceType<LuckyDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(LuckyDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Standard_To_Odds()
        {
            Setup();
            _diceManager.AddDice<StandardDice>(1);

            _diceManager.ChangeDiceType<OddsDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(OddsDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Standard_To_Evens()
        {
            Setup();
            _diceManager.AddDice<StandardDice>(1);

            _diceManager.ChangeDiceType<EvensDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(EvensDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Evens_To_Standard()
        {
            Setup();
            _diceManager.AddDice<EvensDice>(1);

            _diceManager.ChangeDiceType<StandardDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(StandardDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Evens_To_Odds()
        {
            Setup();
            _diceManager.AddDice<EvensDice>(1);

            _diceManager.ChangeDiceType<OddsDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(OddsDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Evens_To_Lucky()
        {
            Setup();
            _diceManager.AddDice<EvensDice>(1);

            _diceManager.ChangeDiceType<LuckyDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(LuckyDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Odds_To_Standard()
        {
            Setup();
            _diceManager.AddDice<OddsDice>(1);

            _diceManager.ChangeDiceType<StandardDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(StandardDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Odds_To_Evens()
        {
            Setup();
            _diceManager.AddDice<OddsDice>(1);

            _diceManager.ChangeDiceType<EvensDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(EvensDice));
        }

        [Fact]
        public void Dice_Manager_Change_Die_From_Odds_To_Lucky()
        {
            Setup();
            _diceManager.AddDice<OddsDice>(1);

            _diceManager.ChangeDiceType<LuckyDice>(_diceManager.GetDiceSpritesExact()[0]);

            var die = _diceManager.GetDiceSpritesExact().FirstOrDefault();

            Assert.True(die.GetType() == typeof(LuckyDice));
        }

    }
}
