using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;

namespace Project2
{
    /// <summary>
    /// Implements a Finite State Machine to control game state flow.
    /// State should not be directly accessable or assignable - state transitions should be communicated via events only.
    /// </summary>
    class GameStateManager : GameSystem, IUpdateable
    {   
        enum GameState
        {
            Paused,
            Playing,
            Gameover       
        }

        public enum Event
        { 
            Pause,
            Resume,
            Begin,
            EndLevel,
            EndGame
        }

        private GameState _state;

        private static GameStateManager _instance = null;
        private Project2Game game;
        protected GameStateManager(Project2Game game) : base(game) {
            this.game = game;
        }

        /// <summary>
        /// This isn't really the singleton pattern, but eh.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static GameStateManager Instance(Project2Game game)
        {
            if (_instance == null) {
                return new GameStateManager(game);
            }
            return _instance;
        }


        void IUpdateable.Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

    }
}
