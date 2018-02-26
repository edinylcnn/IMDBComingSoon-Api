namespace IMDBService.Controllers
{
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.Web.Http;
    public class ComingSoonController : ApiController
    {
        public IHttpActionResult Get()
        {
            List<Movie> movies = new List<Movie>();

            var url = "http://www.imdb.com/movies-coming-soon/?ref_=nv_mv_cs_4";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);


            var liste = htmlDoc.DocumentNode.SelectNodes("//div[@class='list detail']/div");
            foreach (var item in liste)
            {
                var htmlSubDoc = new HtmlDocument();
                htmlSubDoc.LoadHtml(item.InnerHtml);


                Movie movie = new Movie();
                movie.MovieName = htmlSubDoc.DocumentNode.SelectSingleNode("//h4[@itemprop='name']/a").InnerHtml;
                movie.ImageUrl = htmlSubDoc.DocumentNode.SelectSingleNode("//img[@class='poster shadowed']").Attributes["src"].Value;
                movie.Description = htmlSubDoc.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerHtml;

                var s = htmlSubDoc.DocumentNode.SelectSingleNode("//time");
                movie.Duration = s != null ? s.InnerHtml : ":(";
                try
                {
                    #region Stars
                    var NodesStar = htmlSubDoc
                        .DocumentNode
                        .SelectNodes("//div[@class='txt-block']/span[@itemprop='actors']/span/a");


                List<Star> stars = new List<Star>();

                foreach (var str in NodesStar)
                {
                    Star star = new Star();
                    star.FullName = str.InnerHtml;
                    stars.Add(star);
                }
                movie.Stars = stars;
                    #endregion

                }
                catch (System.Exception)
                {
                    continue;
                    
                }
                #region Directors
                var NodesDirector = htmlSubDoc
                        .DocumentNode
                        .SelectNodes("//div[@class='txt-block']/span[@itemprop='director']/span/a");

                List<Director> directors = new List<Director>();

                foreach (var drc in NodesDirector)
                {
                    Director director = new Director
                    {
                        FullName = drc.InnerHtml
                    };
                    directors.Add(director);
                }
                movie.Directors = directors;
                #endregion

                #region Genre
                var NodesGenre = htmlSubDoc
                        .DocumentNode
                        .SelectNodes("//span[@itemprop='genre']");

                List<Genre> genres = new List<Genre>();

                foreach (var gnr in NodesGenre)
                {
                    Genre genre = new Genre
                    {
                        Name = gnr.InnerHtml
                    };
                    genres.Add(genre);
                }
                movie.Genre = genres;
                #endregion

                movies.Add(movie);
            }

            return Json(movies);
        }


        public class Movie
        {
            public string MovieName { get; set; }
            public string ImageUrl { get; set; }
            public string Duration { get; set; }
            public string Description { get; set; }
            public List<Star> Stars { get; set; }
            public List<Director> Directors { get; set; }
            public List<Genre> Genre { get; set; }
        }


        public class Star
        {
            public string FullName { get; set; }

        }
        public class Director
        {
            public string FullName { get; set; }
        }

        public class Genre
        {
            public string Name { get; set; }
        }
    }
}

