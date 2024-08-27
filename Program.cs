namespace BiblionetkaScraper2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var output = Path.GetDirectoryName(args[0]) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(args[0]) + ".csv";
            foreach(var a in args)
                Scrap(a, output);
        }

        private static void Scrap(string inputFile, string outputFile)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(inputFile);

            var notes = doc.DocumentNode.SelectSingleNode("/html/body/form/div[3]/section[4]/div[2]/div[1]/div[2]/div/div/div[5]")
                .Descendants()
                .Where(node => node.GetAttributeValue("class", "").Contains("row forum__list"))
                .ToList();

            var path = outputFile;
            using var file = File.Exists(path) ? File.Open(path, FileMode.Append) : File.Open(path, FileMode.CreateNew);
            using var stream = new StreamWriter(file);
            foreach (var n in notes)
            {
                var note = n.SelectSingleNode("div[1]/a[1]");
                var bookName = '"' + note.InnerText.Replace("\"", "'") + '"';

                var bookNote = note.GetAttributeValue("title", "").Replace("Twoja ocena: ", "").Replace(",", ".");
                
                var authors = n.SelectNodes("div[1]/a")
                    .Skip(1)
                    .Where(node => !node.GetAttributeValue("class", "").Contains("icon"))
                    .Select(n => n.InnerText);

                var authorsJoined = authors.Count() > 1 ? string.Join(",", authors) : authors.FirstOrDefault();
                var bookAuthors = '"' + authorsJoined + '"';

                string line = bookName + "," + bookAuthors + "," + bookNote;
                Console.WriteLine(line);
                stream.WriteLine(line);
            }
        }
    }
}
