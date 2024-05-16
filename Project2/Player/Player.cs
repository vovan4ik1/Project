﻿using System;

namespace Project2.Player
{
    public class Player
    {
        public EPlayerColor Color { get; set; }
        public EPlayerType Type { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public Player(EPlayerType type = EPlayerType.Computer)
        {
            if (type == EPlayerType.Computer)
            {
                Name = "Robot_ToBi";
            }

            Score = 0;
            Type = type;
        }
        
        public static EPlayerColor TossColor()
        {
            Random random = new Random();
            return random.Next() % 2 == 0 ? EPlayerColor.White : EPlayerColor.Black;
        }
    }
}