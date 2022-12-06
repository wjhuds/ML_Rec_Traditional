using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRecTraditional
{
    public class Rating
    {
        public int UserID { get; set; }
        public Movie Movie { get; set; }
        public double Score { get; set; }

        public Rating(int userID, Movie movie, double score)
        {
            UserID = userID;
            Movie = movie;
            Score = score;
        }
    }
}
