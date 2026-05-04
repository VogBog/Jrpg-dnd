using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Rolls.Data;
using Game.Scripts.Rolls.Events;
using Game.Scripts.Rolls.Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Scripts.Rolls.Implementations
{
    public class DiceRoller : IDiceRoller
    {
        public async UniTask<RollResult> Roll(RollDiceParameters parameters, CancellationToken ct)
        {
            var rollsResult = RollWithAdvantages(parameters, out var rolledEvent);
            await parameters.RolledEvent.Invoke(rolledEvent, ct);

            if (rolledEvent.MustReRoll)
            {
                ClearRolledEventData(rolledEvent);
                rollsResult = RollWithAdvantages(parameters, out rolledEvent);
                await parameters.RolledEvent.Invoke(rolledEvent, ct);
            }
            
            ct.ThrowIfCancellationRequested();

            bool firstRoll = true;
            foreach (var rolledDices in rolledEvent.Rolls)
            {
                foreach (var rolledSingleDice in rolledDices.Rolls)
                {
                    if (rolledSingleDice.MustReRoll)
                    {
                        int prevNum = rolledSingleDice.Result;
                        int newNum = RollSingleDice(rolledSingleDice.Dice).Result;
                        rollsResult.Result += newNum - prevNum;

                        if (firstRoll && newNum == rolledSingleDice.Dice)
                            rollsResult.CriticalHigh = true;
                        else if (firstRoll && newNum == 1)
                            rollsResult.CriticalOne = true;
                    }

                    firstRoll = false;
                }
            }
            
            ClearRolledEventData(rolledEvent);

            return rollsResult;
        }

        private RollResult RollWithAdvantages(
            RollDiceParameters parameters,
            out RolledDicesEvent rolledEvent)
        {
            //First roll
            var result1 = RollOneTime(parameters, out rolledEvent);
            
            if (parameters.Advantages == 0 && parameters.Disadvantages == 0 ||
                parameters.Advantages > 0 && parameters.Disadvantages > 0)
            {
                return result1;
            }

            //Second roll
            var result2 = RollOneTime(parameters, out var rolledEvent2);

            //Advantage/Disadvantage
            if (parameters.Advantages > 0 && result2.Result > result1.Result ||
                parameters.Disadvantages > 0 && result2.Result < result1.Result)
            {
                ClearRolledEventData(rolledEvent);
                result1 = result2;
                rolledEvent = rolledEvent2;
            }
            else
            {
                ClearRolledEventData(rolledEvent2);
            }

            return result1;
        }

        private RollResult RollOneTime(
            RollDiceParameters parameters,
            out RolledDicesEvent rolledEvent)
        {
            var result = RollAllDices(parameters, out var allRollsResults);
            result.Result += parameters.Bonus;

            rolledEvent = new RolledDicesEvent(parameters, result.Result, allRollsResults);
            
            return result;
        }

        private RollResult RollAllDices(
            RollDiceParameters parameters,
            out List<RolledDicesEvent.RolledDicesResult> allRollsResults)
        {
            allRollsResults = ListPool<RolledDicesEvent.RolledDicesResult>.Get();
            var result = new RollResult();
            foreach (var diceParameters in parameters.Dices)
            {
                var diceResults = ListPool<RolledDicesEvent.RolledSingleDiceResult>.Get();
                
                for (int i = 0; i < diceParameters.Count; i++)
                {
                    var singleRoll = RollSingleDice(diceParameters.Dice);
                    result.Result += singleRoll.Result;
                    diceResults.Add(singleRoll);

                    if (singleRoll.Result >= diceParameters.Dice - parameters.CritAvailability && i == 0)
                        result.CriticalHigh = true;
                    else if (singleRoll.Result == 1 && i == 0)
                        result.CriticalOne = true;
                }

                allRollsResults.Add(new(diceResults));
            }

            return result;
        }

        private RolledDicesEvent.RolledSingleDiceResult RollSingleDice(int dice)
        {
            int roll = Random.Range(1, dice + 1);
            return new RolledDicesEvent.RolledSingleDiceResult(dice, roll);
        }

        private void ClearRolledEventData(RolledDicesEvent e)
        {
            if (e.Rolls == null)
                return;
            
            for (int i = 0; i < e.Rolls.Count; i++)
            {
                var list = e.Rolls[i].Rolls;
                list.Clear();
                ListPool<RolledDicesEvent.RolledSingleDiceResult>.Release(list);
            }
            
            e.Rolls.Clear();
            ListPool<RolledDicesEvent.RolledDicesResult>.Release(e.Rolls);

            e.Rolls = null;
        }
    }
}