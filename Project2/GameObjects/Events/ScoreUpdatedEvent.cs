using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.GameObjects.Events
{
    class ScoreUpdatedEvent : EventArgs
    {
        private double score;
        public ScoreUpdatedEvent(double score)
        {
            this.score = score;
        }
        public double getScore()
        {
            return score;
        }
    }
}
