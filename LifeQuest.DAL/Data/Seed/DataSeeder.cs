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
            try 
            {
                // 0. Hard Reset for Challenges (As requested: "امسح كل التحديات وهنبدأ من جديد")
                context.DailyLogs.RemoveRange(context.DailyLogs);
                context.UserChallenges.RemoveRange(context.UserChallenges);
                context.Challenges.RemoveRange(context.Challenges);
                await context.SaveChangesAsync();

                // 1. Seed Test User
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
                    var result = await userManager.CreateAsync(heroUser, "Hero@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(heroUser, "Admin");
                    }
                    else
                    {
                        Console.WriteLine("USER SEEDING FAILED: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // Ensure existing Hero is also an Admin
                    if (!await userManager.IsInRoleAsync(heroUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(heroUser, "Admin");
                    }
                }
                int heroId = heroUser?.Id ?? 0;

                // 2. Seed Levels (Critical for badges)
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
                    await context.SaveChangesAsync();
                }

                // 3. Seed Badges (Aggrerssive: Add if missing by name)
                var levelsInDb = await context.Levels.ToListAsync();
                var noviceLevel = levelsInDb.FirstOrDefault(l => l.LevelName == "Novice");
                var apprenticeLevel = levelsInDb.FirstOrDefault(l => l.LevelName == "Apprentice");
                var warriorLevel = levelsInDb.FirstOrDefault(l => l.LevelName == "Warrior");
                var legendLevel = levelsInDb.FirstOrDefault(l => l.LevelName == "Legend");

                var seededBadges = new List<Badges>
                {
                    new Badges { Name = "First Step", Description = "Complete your first challenge", Points = 100, Image = "fa-shoe-prints", RequiredLevelId = noviceLevel?.Id, CriteriaType = BadgeCriteriaType.ChallengeCount, CriteriaValue = 1 },
                    new Badges { Name = "Explorer", Description = "Join 3 different challenges", Points = 150, Image = "fa-compass", RequiredLevelId = noviceLevel?.Id, CriteriaType = BadgeCriteriaType.ChallengeCount, CriteriaValue = 3 },
                    new Badges { Name = "Apprentice Rising", Description = "Reach Apprentice level", Points = 300, Image = "fa-graduation-cap", RequiredLevelId = apprenticeLevel?.Id, CriteriaType = BadgeCriteriaType.Level, CriteriaValue = 0 },
                    new Badges { Name = "Consistency King", Description = "Log activities for 7 consecutive days", Points = 500, Image = "fa-crown", RequiredLevelId = apprenticeLevel?.Id, CriteriaType = BadgeCriteriaType.Streak, CriteriaValue = 7 },
                    new Badges { Name = "Warrior Spirit", Description = "Reach Warrior level", Points = 800, Image = "fa-shield-halved", RequiredLevelId = warriorLevel?.Id, CriteriaType = BadgeCriteriaType.Level, CriteriaValue = 0 },
                    new Badges { Name = "Quest Master", Description = "Complete 10 challenges", Points = 1000, Image = "fa-trophy", RequiredLevelId = warriorLevel?.Id, CriteriaType = BadgeCriteriaType.ChallengeCount, CriteriaValue = 10 },
                    new Badges { Name = "Living Legend", Description = "Reach Legend level", Points = 2000, Image = "fa-dragon", RequiredLevelId = legendLevel?.Id, CriteriaType = BadgeCriteriaType.Level, CriteriaValue = 0 },
                    new Badges { Name = "Unstoppable", Description = "Complete 20 challenges", Points = 3000, Image = "fa-fire-flame-curved", RequiredLevelId = legendLevel?.Id, CriteriaType = BadgeCriteriaType.ChallengeCount, CriteriaValue = 20 }
                };

                foreach (var badge in seededBadges)
                {
                    if (!await context.Badges.AnyAsync(b => b.Name == badge.Name))
                    {
                        await context.Badges.AddAsync(badge);
                    }
                }
                await context.SaveChangesAsync();

                // 4. Seed Categories
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
                    await context.SaveChangesAsync();
                }

                // 5. Seed Challenges
                if (!await context.Challenges.AnyAsync() && heroId != 0)
                {
                    var healthCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Health & Fitness");
                    var learningCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Learning");

                    if (healthCategory != null || learningCategory != null)
                    {
                        var challenges = new List<Challenge>
                        {
                            new Challenge 
                            { 
                                Title = "5K Run", 
                                Description = "Run a total of 5 kilometers to boost your cardio.", 
                                Difficulty = ChallengeDifficulty.Medium, 
                                Duration = 300, 
                                Points = 200,
                                CategoryId = healthCategory?.Id ?? (await context.Categories.FirstAsync()).Id,
                                ApplicationUserId = heroId,
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(30),
                                IsPublic = true
                            },
                            new Challenge 
                            { 
                                Title = "Read 20 Pages", 
                                Description = "Read 20 pages of any educational book every day.", 
                                Difficulty = ChallengeDifficulty.Easy, 
                                Duration = 300, 
                                Points = 150,
                                CategoryId = learningCategory?.Id ?? (await context.Categories.FirstAsync()).Id,
                                ApplicationUserId = heroId,
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(30),
                                IsPublic = true
                            }
                        };
                        await context.Challenges.AddRangeAsync(challenges);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRITICAL SEEDING ERROR: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("INNER EXCEPTION: " + ex.InnerException.Message);
                }
            }
        }
    }
}
