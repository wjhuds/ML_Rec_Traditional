using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MovieRecTraditional
{
    class Program
    {
        static List<Movie> movies = new List<Movie>();
        static List<Rating> ratings = new List<Rating>();
        static Movie selection = null;

        static void Main(string[] args)
        {
            //import the csv files for our dataset
            ImportDataset();

            //keep trying to get a valid user input until success
            var input = GetUserInput();
            while (!input)
            {
                input = GetUserInput();
            }

            //use the rules based system to try to find good recommendations
            GetSuggestions();

            Console.ReadLine();
        }

        static void ImportDataset()
        {
            Console.WriteLine("Importing movies...");
            var moviesFileLines = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\movies.csv");
            foreach (var line in moviesFileLines.Skip(1))
            {
                var elements = line.Split(',');

                var id = int.Parse(elements[0]);
                var title = elements[1];
                var genres = elements[2].Split('|').ToList();

                movies.Add(new Movie(id, title, genres));
            }
            Console.WriteLine("Done.");

            Console.WriteLine("Importing ratings...");
            var ratingsFileLines = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ratings.csv");
            foreach (var line in ratingsFileLines.Skip(1))
            {
                var elements = line.Split(',');

                var user = int.Parse(elements[0]);
                var movie = movies.Find(x => x.ID == int.Parse(elements[1]));
                var score = double.Parse(elements[2]);

                ratings.Add(new Rating(user, movie, score));
            }
            Console.WriteLine("Done.");
        }

        static bool GetUserInput()
        {
            Console.WriteLine("Enter a movie title to see suggested recommendations:");
            var input = Console.ReadLine();

            //try to find movie entries that match this input
            var movieSearch = movies.FindAll(x => x.Title.ToUpper().IndexOf(input.ToUpper()) > -1);
            if (!movieSearch.Any()) return false;

            Console.WriteLine("Which of these movies is correct?");

            //list out matching movie titles
            for (var i = 0; i < 5; i++)
            {
                if (movieSearch.Count > i)
                {
                    Console.WriteLine($"{i}: {movieSearch.ElementAt(i).Title}");
                }
            }

            var selectionInput = Console.ReadLine();
            try
            {
                //try to recieve user input of movie selection
                var num = int.Parse(selectionInput);
                if (num > 4 || num >= movieSearch.Count)
                {
                    throw new ApplicationException();
                }
                selection = movieSearch.ElementAt(num);
                return true;
            }
            catch (Exception ex)
            {
                //fail and return false if invalid input so that the loop starts again
                Console.WriteLine("Invalid Input.");
                return false;
            }
        }

        static void GetSuggestions()
        {
            var recommendations = new List<Recommendation>();

            //first find similar movies based on tags
            Console.WriteLine("Finding similar movies...");
            var tags = selection.Genres;
            foreach (var tag in tags)
            {
                var matches = movies.Where(x => x.Genres.IndexOf(tag) > -1);
                foreach (var match in matches)
                {
                    var matchInList = recommendations.FirstOrDefault(x => x.Movie == match);

                    if (matchInList == null) //not already added to rec list
                    {
                        recommendations.Add(new Recommendation(match, 5));
                    }
                    else //match already exists in rec list
                    {
                        matchInList.AddScore(5);
                    }
                }
            }
            //remove the entry for the movie the user chose as we dont want to recommend the same movie they input
            recommendations.Remove(recommendations.Find(x => x.Movie == selection));

            //now adjust the recommendations based on the average score
            Console.WriteLine("Adjusting recommendations based on review scores...");
            foreach (var rec in recommendations)
            {
                var recRatings = ratings.FindAll(x => x.Movie == rec.Movie).Select(y => y.Score);
                var avgRating = recRatings.Sum() / recRatings.Count();

                rec.AvgUserScore = avgRating;

                //ignore movies that dont have enough ratings
                if (recRatings.Count() < 10) continue;

                if (avgRating > 4) rec.AddScore(10);
                if (avgRating < 4 && avgRating > 3) rec.AddScore(5);
                if (avgRating < 3 && avgRating > 2) rec.AddScore(0);
                if (avgRating < 2 && avgRating > 1) rec.AddScore(-5);
                if (avgRating < 1) rec.AddScore(-10);
            }
            recommendations = recommendations.OrderByDescending(x => x.RecScore).ToList();

            //print out the recommendations for the user
            Console.WriteLine("Here are the top 10 recommendations you might like similar to this movie:");
            for (var i = 0; i < 10; i++)
            {
                if (recommendations.Count > i)
                {
                    var rec = recommendations.ElementAt(i);
                    Console.WriteLine($"{i+1}: {rec.Movie.Title} (Avg Rating: {rec.AvgUserScore.ToString("0.0")})");
                }
            }
        }
    }
}
