using MyProject.Database;
using System;
using System.IO;
using MyProject.Database;
using System.Linq;

public static class DataSeeder
{
    public static void Seed()
    {
        var ctx = Core.Context;
        if (ctx.Books.Any()) return; // уже засеяно

        var author = ctx.Users.First(u => u.Roles.Name == "Author");

        var book = new Books
        {
            Name = "Мастер и Маргарита",
            CoverPath = "/Assets/Covers/master.jpg",
            AuthorId = author.Id,
            IsFrozen = false
        };
        ctx.Books.Add(book);
        ctx.SaveChanges();

        // Папка с главами лежит где-то на диске
        string[] files = Directory.GetFiles(@"C:\Books\MasterMargarita", "*.txt")
                                    .OrderBy(f => f).ToArray();

        for (int i = 0; i < files.Length; i++)
        {
            ctx.Chapters.Add(new Chapters
            {
                BookId = book.Id,
                Number = i + 1,
                Name = $"Глава {i + 1}",
                Path = files[i]
            });
        }
        ctx.SaveChanges();
    }
}