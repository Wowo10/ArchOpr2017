using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace DiceWars
{
    enum StateActions
    {
        NONE = 0,
        PUSH,
        POP,
        POPNPUSH
    }

    class StateManager //singleton!
    {
        Stack<GameState> statesqueue;

        /* potential singleton protection for future
        private static StateManager instance;

        private StateManager()
        {

            //set menu for start
            //statesqueue.Enqueue()

        }

        public static StateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StateManager();
                }
                return instance;
            }
        }*/

        public StateManager()
        {
            statesqueue = new Stack<GameState>();

            //pushing menu state
            statesqueue.Push(new GS_Menu());
        }

        public void EventsUpdate(RenderWindow window)
        {
            window.MouseButtonPressed += onMousePress;
            window.MouseMoved += onMousMove;
            window.MouseButtonReleased += onMouseButtonRelease;
        }

        private void onMouseButtonRelease(object sender, MouseButtonEventArgs e)
        {
            statesqueue.Peek().onRelease(e);
        }

        private void onMousMove(object sender, MouseMoveEventArgs e)
        {
            statesqueue.Peek().onMouseMove(e);
        }

        private void onMousePress(object sender, MouseButtonEventArgs e)
        {
            statesqueue.Peek().onClick(e);
        }

        public void PushState()
        {
            GameState helpstate = new GameState();
            if (statesqueue.Peek().nextstate != States.NONE)
            {                
                if (statesqueue.Peek().nextstate == States.GS_MENU)
                {
                    helpstate = new GS_Menu();
                }
                else if (statesqueue.Peek().nextstate == States.GS_GAMEPLAY)
                {
                    helpstate = new GS_Gameplay();
                }
                else if (statesqueue.Peek().nextstate == States.GS_LOBBY)
                {
                    helpstate = new GS_Lobby();
                }
                else if (statesqueue.Peek().nextstate == States.GS_CREDITS)
                {
                    helpstate = new GS_Credits();
                }
                else if (statesqueue.Peek().nextstate == States.GS_SETTINGS)
                {
                    helpstate = new GS_Settings();
                }
                else
                {
                    helpstate = new GS_Menu(); //menu by default
                }
            }
            statesqueue.Peek().nextstate = States.NONE;
            statesqueue.Peek().stateaction = StateActions.NONE;

            statesqueue.Push(helpstate);            
        }

        public void PopState()
        {
            statesqueue.Peek().nextstate = States.NONE;
            statesqueue.Peek().stateaction = StateActions.NONE;

            statesqueue.Pop();
        }

        public void MainLoop(RenderWindow window)
        {
            if (statesqueue.Peek().stateaction != StateActions.NONE)
            {
                if(statesqueue.Peek().stateaction == StateActions.PUSH)
                {
                    statesqueue.Peek().ReleaseGui();
                    PushState();
                }
                else if(statesqueue.Peek().stateaction == StateActions.POP)
                {
                    PopState();
                }
                else if (statesqueue.Peek().stateaction == StateActions.POPNPUSH)
                {
                    PopState();
                    PushState();
                }
            }

            if (statesqueue.Count == 0)
            {
                Program.exit = true;
                return;
            }

            statesqueue.Peek().Update();
            statesqueue.Peek().Render(window);
        }
        
    }
}
