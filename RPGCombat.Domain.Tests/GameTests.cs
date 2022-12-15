﻿using RPGCombat.Domain.Characters;
using RPGCombat.Domain.Things;

namespace RPGCombat.Domain.Tests
{
    public class GameTests
    {
        private const decimal StartingHealth = 1000m;

        [Fact]
        public void can_deal_damage()
        {
            var attacker = Character.Create();
            var damage = 100m;
            var target = Character.Create();
            Game.Attack(attacker, damage, target);


            Assert.Equal(StartingHealth - damage, target.Health);
            Assert.True(attacker.Alive());
        }

        [Fact]
        public void can_NOT_deal_damage_to_self()
        {
            var attacker = Character.Create();
            var damage = 100m;


            Game.Attack(attacker, damage, attacker);

            Assert.Equal(StartingHealth, attacker.Health);
            Assert.True(attacker.Alive());
        }

        [Fact]
        public void can_apply_damage_bonus()
        {
            var attacker = Character.Create();
            attacker.Level = 10;
            var target = Character.Create();
            var damage = 100m;

            Game.Attack(attacker, damage, target);

            Assert.Equal(StartingHealth - damage * 1.5m, target.Health);
            Assert.True(target.Alive());
        }

        [Fact]
        public void can_apply_damage_reduction()
        {
            var attacker = Character.Create();
            var target = Character.Create();
            target.Level = 10;
            var damage = 100m;

            Game.Attack(attacker, damage, target);

            Assert.Equal(StartingHealth - damage * 0.5m, target.Health);
            Assert.True(target.Alive());
        }

        [Fact]
        public void MeleeFighter_can_NOT_damage_OUT_OF_RANGE_target()
        {
            var attacker = new MeleeFighter();
            var target = Character.Create();
            target.Position = new Position(10, 10);
            target.Level = 10;
            var damage = 100m;

            Game.Attack(attacker, damage, target);

            Assert.Equal(StartingHealth, target.Health);
            Assert.True(target.Alive());
        }

        [Fact]
        public void RangeFighter_can_NOT_damage_OUT_OF_RANGE_target()
        {
            var attacker = new RangedFighter();
            var target = Character.Create();
            target.Position = new Position(100, 100);
            target.Level = 10;
            var damage = 100m;

            Game.Attack(attacker, damage, target);

            Assert.Equal(StartingHealth, target.Health);
            Assert.True(target.Alive());
        }

        [Fact]
        public void Allies_can_Heal_each_other()
        {
            var allie1 = new MeleeFighter();
            const string faction = "faction";
            allie1.Join(faction);
            var allie2 = new MeleeFighter();
            allie2.Join(faction);

            var fighter = new MeleeFighter();

            Game.Attack(fighter, 100m, allie2);

            Game.Heal(allie1, 100m, allie2);

            Assert.Equal(StartingHealth, allie2.Health);
        }

        [Fact]
        public void NON_Allies_can_NOT_Heal_each_other()
        {
            var allie1 = new MeleeFighter();
            allie1.Join("faction");
            var allie2 = new MeleeFighter();
            allie2.Join("faction1");

            var fighter = new MeleeFighter();

            var damage = 100m;
            Game.Attack(fighter, damage, allie2);

            Game.Heal(allie1, damage, allie2);

            Assert.Equal(StartingHealth - damage, allie2.Health);
        }

        [Fact]
        public void can_attack_a_tree()
        {
            var attacker = new RangedFighter();
            var damage = 100000m;
            var tree = new Tree();
            Game.Attack(attacker, damage, tree);

            Assert.Equal(0, tree.Health);
            Assert.False(tree.Alive);
        }
    }
}