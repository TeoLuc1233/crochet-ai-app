using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Patterns
        if (!await context.Patterns.AnyAsync())
        {
            var patterns = new List<Pattern>
            {
                new Pattern
                {
                    Title = "Amigurumi Bunny",
                    Description = "A cute little bunny perfect for beginners",
                    Difficulty = "Beginner",
                    Category = "Amigurumi",
                    Materials = "{\"yarn\":\"Worsted weight yarn\",\"hook\":\"5mm\",\"other\":[\"Safety eyes\",\"Fiberfill\",\"Yarn needle\"]}",
                    Instructions = "Row 1: Magic ring, 6 sc\nRow 2: 2 sc in each (12)\nRow 3: *sc, inc* repeat (18)\nContinue increasing...",
                    IsPremium = false,
                    ViewCount = 0
                },
                new Pattern
                {
                    Title = "Cozy Blanket",
                    Description = "A warm and cozy blanket for your home",
                    Difficulty = "Intermediate",
                    Category = "Home",
                    Materials = "{\"yarn\":\"Chunky yarn\",\"hook\":\"8mm\",\"other\":[\"Scissors\"]}",
                    Instructions = "Chain 120\nRow 1: Sc in second chain from hook, sc across\nRow 2: Chain 1, turn, sc across\nRepeat...",
                    IsPremium = true,
                    ViewCount = 0
                },
                new Pattern
                {
                    Title = "Elegant Cardigan",
                    Description = "A beautiful cardigan with intricate details",
                    Difficulty = "Advanced",
                    Category = "Clothing",
                    Materials = "{\"yarn\":\"DK weight yarn\",\"hook\":\"4mm\",\"other\":[\"Buttons\",\"Stitch markers\"]}",
                    Instructions = "Start with back panel:\nChain 80\nRow 1: Dc in fourth chain from hook...",
                    IsPremium = true,
                    ViewCount = 0
                },
                new Pattern
                {
                    Title = "Granny Square Bag",
                    Description = "A stylish bag made from granny squares",
                    Difficulty = "Beginner",
                    Category = "Accessories",
                    Materials = "{\"yarn\":\"Worsted weight yarn\",\"hook\":\"5mm\",\"other\":[\"Lining fabric\",\"Zipper\"]}",
                    Instructions = "Make 12 granny squares:\nChain 4, join\nRound 1: Chain 3, 2 dc, chain 2, 3 dc...",
                    IsPremium = false,
                    ViewCount = 0
                },
                new Pattern
                {
                    Title = "Floral Doily",
                    Description = "Delicate doily with floral pattern",
                    Difficulty = "Advanced",
                    Category = "Home",
                    Materials = "{\"yarn\":\"Thread weight yarn\",\"hook\":\"1.5mm\",\"other\":[\"Blocking pins\"]}",
                    Instructions = "Magic ring\nRound 1: 12 sc in ring\nRound 2: *sc, ch 3, sc* repeat...",
                    IsPremium = true,
                    ViewCount = 0
                }
            };

            await context.Patterns.AddRangeAsync(patterns);
            await context.SaveChangesAsync();
        }
    }
}
