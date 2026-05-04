using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Events.Implementations;
using Game.Scripts.Rolls.Events;
using UnityEngine;

namespace Game.Scripts.Rolls.Data
{
    public class RollDiceParameters
    {
        public readonly List<DiceParameters> Dices = new();
        public int Bonus;
        public int Advantages;
        public int Disadvantages;
        public int CritAvailability;
        public readonly AsyncQueryableObservable<RolledDicesEvent> RolledEvent = new();

        public RollDiceParameters Init(int dice)
        {
            Clear();
            Dices.Add(new DiceParameters(1, dice));

            return this;
        }

        public RollDiceParameters Init(int count, int dice)
        {
            Clear();
            Dices.Add(new DiceParameters(count, dice));

            return this;
        }

        public RollDiceParameters Clear()
        {
            Dices.Clear();
            Bonus = 0;
            Advantages = 0;
            Disadvantages = 0;
            RolledEvent.Clear();

            return this;
        }

        public RollDiceParameters AddAdvantage()
        {
            ++Advantages;
            return this;
        }

        public RollDiceParameters AddDisadvantage()
        {
            ++Disadvantages;
            return this;
        }

        public RollDiceParameters AddDices(int count, int dice)
        {
            int index = Dices.FindIndex(x => x.Dice == dice);
            if (index == -1)
            {
                Dices.Add(new (count, dice));
            }
            else
            {
                var parameters = Dices[index];
                parameters.Count += count;
                Dices[index] = parameters;
            }
            
            return this;
        }

        public RollDiceParameters RemoveDices(int count, int dice)
        {
            int index = Dices.FindIndex(x => x.Dice == dice);
            if (index == -1)
                return this;
            
            var parameters = Dices[index];
            parameters.Count = Mathf.Clamp(parameters.Count - count, 0, parameters.Count);
            Dices[index] = parameters;

            return this;
        }

        public RollDiceParameters AddRolledDiceEventHandler(Func<RolledDicesEvent, CancellationToken, UniTask> handler)
        {
            RolledEvent.Subscribe(handler);
            return this;
        }

        public RollDiceParameters AddBonus(int bonus)
        {
            Bonus += bonus;
            return this;
        }

        public RollDiceParameters AddCritAvailability(int critAvailability)
        {
            CritAvailability += critAvailability;
            return this;
        }
    }
}