﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    interface IClickable
    {
        bool Clicked(float x, float y);
        void Released(float x, float y);
    }
}
