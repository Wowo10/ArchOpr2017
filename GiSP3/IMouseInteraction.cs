using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace DiceWars
{
    interface IMouseInteraction: IClickable, IMouseMove, Drawable
    {
    }
}
