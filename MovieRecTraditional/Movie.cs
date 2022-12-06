using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRecTraditional
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; }

        public Movie(int id, string title, List<string> genres)
        {
            ID = id;
            Title = title;
            Genres = genres;
        }
    }
}
