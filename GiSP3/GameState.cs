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
        GS_SETTINGS,
        GS_CREDITS
    }

    class GameState
    {
        //To change state anyhow you need to set theese enums
        public States nextstate;
        public StateActions stateaction;
        public Color backgroundColor;

        public List<IMouseInteraction> mouseInteractionList;

        public GameState()
        {
            backgroundColor = new Color(0, 100, 30);
            nextstate = States.NONE;
            stateaction = StateActions.NONE;
            mouseInteractionList = new List<IMouseInteraction>();
        }

        public virtual void onClick(MouseButtonEventArgs e)
        {
            for (int i = 0; i < mouseInteractionList.Count; i++)
            {
                mouseInteractionList[i].Clicked(e.X, e.Y);
            }
        }

        public virtual void onRelease(MouseButtonEventArgs e)
        {
            for (int i = 0; i < mouseInteractionList.Count; i++)
            {
                mouseInteractionList[i].Released(e.X, e.Y);
            }
        }

        public virtual void onMouseMove(MouseMoveEventArgs e)
        {
            for (int i = 0; i < mouseInteractionList.Count; i++)
            {
                mouseInteractionList[i].MouseMove(e.X, e.Y);
            }
        }

        //for taking the input
        public virtual void Update() { }

        //for showing the output
        public virtual void Render(RenderWindow window) { }

        public virtual void DrawMouseInteractive(RenderWindow window)
        {
            mouseInteractionList.ForEach(x => window.Draw(x));
        }
    }
}
