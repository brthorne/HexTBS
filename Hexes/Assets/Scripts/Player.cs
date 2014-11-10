using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Player
    {
        public int Speed { get; private set; }
        public int HealthMax { get; private set; }
        public int HealthCurrent { get; private set; }

        public Player(int speed, int healthMax, int healthCurrent)
        {
            this.Speed = speed;
            this.HealthMax = healthMax;
            this.HealthCurrent = healthCurrent;
        }

        //apply armor and dodginess, extra effects &c
        public void ApplyDamage(int amount)
        {

        }
    }
}
