using LifeQuest.DAL.Data;
using LifeQuest.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LifeQuest.DAL.Data.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Seed Test User
            var heroUser = await userManager.FindByNameAsync("Hero");
            if (heroUser == null)
            {
                heroUser = new ApplicationUser
                {
                    UserName = "Hero",
                    Email = "Hero@lifequest.com",
                    EmailConfirmed = true,
                    Name = "System Hero",
                    Country = "Egypt"
                };
                await userManager.CreateAsync(heroUser, "Hero@123");
            }
            int heroId = heroUser?.Id ?? 0;

            // Seed Levels first (badges need level IDs)
            if (!await context.Levels.AnyAsync())
            {
                var levels = new List<Level>
                {
                    new Level { LevelName = "Novice", Point = 0, LevelsCount = 1 },
                    new Level { LevelName = "Apprentice", Point = 500, LevelsCount = 2 },
                    new Level { LevelName = "Warrior", Point = 1500, LevelsCount = 3 },
                    new Level { LevelName = "Legend", Point = 5000, LevelsCount = 4 }
                };
                await context.Levels.AddRangeAsync(levels);
                await context.SaveChangesAsync(); // Save levels first to get IDs
            }

            // Seed Badges
            if (!await context.Badges.AnyAsync())
            {
                var noviceLevel = await context.Levels.FirstOrDefaultAsync(l => l.LevelName == "Novice");
                var apprenticeLevel = await context.Levels.FirstOrDefaultAsync(l => l.LevelName == "Apprentice");
                var warriorLevel = await context.Levels.FirstOrDefaultAsync(l => l.LevelName == "Warrior");
                var legendLevel = await context.Levels.FirstOrDefaultAsync(l => l.LevelName == "Legend");

                var badges = new List<Badges>
                {
                    // Novice level badges
                    new Badges { Name = "First Step", Description = "Complete your first challenge", Points = 100, Image = "fa-shoe-prints", RequiredLevelId = noviceLevel?.Id, CriteriaType = "ChallengeCount", CriteriaValue = 1 },
                    new Badges { Name = "Explorer", Description = "Join 3 different challenges", Points = 150, Image = "fa-compass", RequiredLevelId = noviceLevel?.Id, CriteriaType = "ChallengeCount", CriteriaValue = 3 },

                    // Apprentice level badges
                    new Badges { Name = "Apprentice Rising", Description = "Reach Apprentice level", Points = 300, Image = "fa-graduation-cap", RequiredLevelId = apprenticeLevel?.Id, CriteriaType = "Level", CriteriaValue = 0 },
                    new Badges { Name = "Consistency King", Description = "Log activities for 7 consecutive days", Points = 500, Image = "fa-crown", RequiredLevelId = apprenticeLevel?.Id, CriteriaType = "Streak", CriteriaValue = 7 },

                    // Warrior level badges
                    new Badges { Name = "Warrior Spirit", Description = "Reach Warrior level", Points = 800, Image = "fa-shield-halved", RequiredLevelId = warriorLevel?.Id, CriteriaType = "Level", CriteriaValue = 0 },
                    new Badges { Name = "Quest Master", Description = "Complete 10 challenges", Points = 1000, Image = "fa-trophy", RequiredLevelId = warriorLevel?.Id, CriteriaType = "ChallengeCount", CriteriaValue = 10 },

                    // Legend level badges
                    new Badges { Name = "Living Legend", Description = "Reach Legend level", Points = 2000, Image = "fa-dragon", RequiredLevelId = legendLevel?.Id, CriteriaType = "Level", CriteriaValue = 0 },
                    new Badges { Name = "Unstoppable", Description = "Complete 20 challenges", Points = 3000, Image = "fa-fire-flame-curved", RequiredLevelId = legendLevel?.Id, CriteriaType = "ChallengeCount", CriteriaValue = 20 }
                };
                await context.Badges.AddRangeAsync(badges);
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Health & Fitness" },
                    new Category { Name = "Productivity" },
                    new Category { Name = "Learning" },
                    new Category { Name = "Mindfulness" },
                    new Category { Name = "Social" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync(); // Save categories first to get IDs
            }

            // Seed Challenges
            if (!await context.Challenges.AnyAsync())
            {
                var healthCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Health & Fitness");
                var learningCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Learning");

                var challenges = new List<Challenge>
                {
                    new Challenge 
                    { 
                        Title = "5K Run", 
                        Description = "Run a total of 5 kilometers to boost your cardio.", 
                        Difficulty = "Medium", 
                        Duration = 300, 
                        Points = 200,
                        CategoryId = healthCategory?.Id ?? 0,
                        ApplicationUserId = heroId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(30),
                        IsPublic = true
                    },
                    new Challenge 
                    { 
                        Title = "Read 20 Pages", 
                        Description = "Read 20 pages of any educational book every day.", 
                        Difficulty = "Easy", 
                        Duration = 300, 
                        Points = 150,
                        CategoryId = learningCategory?.Id ?? 0,
                        ApplicationUserId = heroId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(30),
                        IsPublic = true
                    }
                };
                foreach (var challenge in challenges)
                {
                    if (challenge.CategoryId == 0) // Fallback if no category found
                    {
                        var firstCat = await context.Categories.FirstOrDefaultAsync();
                        if (firstCat != null) challenge.CategoryId = firstCat.Id;
                    }
                }
                
                await context.Challenges.AddRangeAsync(challenges);
            }

            try 
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Simple logging to console for debugging
                Console.WriteLine("SEEDING ERROR: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("INNER ERROR: " + ex.InnerException.Message);
                }
                throw; // Rethrow to see the full stack trace in logs anyway
            }
        }
    }
}
