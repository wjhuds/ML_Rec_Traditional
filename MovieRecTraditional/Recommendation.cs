using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRecTraditional
{
    public class Recommendation
    {
        public Movie Movie { get; set; }
        public int RecScore { get; set; }
        public double AvgUserScore { get; set; }

        public Recommendation(Movie movie, int score)
        {
            Movie = movie;
            RecScore = score;
        }

        public void AddScore(int score)
        {
            RecScore += score;
        }
    }
}
