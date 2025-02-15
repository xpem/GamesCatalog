namespace Models.Resps
{
    public record IGDBGame
    {
        public string id { get; set; }

        public Cover cover { get; set; }

        public string name { get; set; }

        public string first_release_date { get; set; }

        public List<Platform> platforms { get; set; }
    }

    public record Cover
    {
        public string id { get; set; }

        public string url { get; set; }

        public string image_id { get; set; }
    }

    public record Platform
    {
        public string id { get; set; }

        public string abbreviation { get; set; }
    }
}
