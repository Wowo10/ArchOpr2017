using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceWars
{
    //When Creating new state you need to add value here
    //and at the StateManager to handle it.
    public enum States
    {
        NONE = 0,
        GS_MENU,
        GS_GAMEPLAY,
        GS_LOBBY,
        GS_SETTINGS
    }

    class GameState
    {
        //To change state anyhow you need to set theese enums
        public States nextstate;
        public StateActions stateaction;

        public GameState()
        {
            nextstate = States.NONE;
            stateaction = StateActions.NONE;
        }

        //for taking the input
        public virtual void Update() { }

        //for showing the output
        public virtual void Render(RenderWindow window) { }
    }
}
