**LifeQuest**

**LifeQuest** is a comprehensive, gamified productivity and self-improvement web application designed to help users build positive habits, achieve their goals, and unlock their full potential. By treating personal development like a game, LifeQuest motivates users to stay consistent through points, levels, and badges.

---

## Key Features

###  For Users
- **Dynamic Dashboard:** Track your current level, total points, earned badges, and active challenges all in one place.
- **Challenges System:** Explore diverse challenges across various categories (e.g., Health & Fitness, Productivity, Learning).
- **Log Daily Progress:** Join challenges and record your daily achievements to earn points.
- **Gamification Engine:** Watch your progress soar as you earn points, level up, and unlock unique badges for consistency.
- **Notifications:** Receive instant updates when you earn points, level up, or unlock a new badge.
- **Analytics & Decisions:** Visualize your progress and make data-driven decisions to improve your lifestyle.

###  For Administrators (RBAC)
- **Admin Dashboard:** A dedicated space for platform management.
- **User Management:** Oversee all registered users, view their statistics, and manage their status.
- **Content Moderation:** Add, edit, or remove challenges, categories, and badges to keep the platform fresh and engaging.
- **Role-Based Access Control:** Secure isolation between standard user data and administrative controls.

---

##  Technology Stack

LifeQuest is built using modern technologies with a clean **N-Tier Architecture** (Presentation, BLL, and DAL):

- **Framework:** ASP.NET Core MVC (C#)
- **ORM:** Entity Framework Core
- **Database:** Microsoft SQL Server
- **Authentication & Security:** ASP.NET Core Identity
- **Frontend:** HTML5, CSS3, Bootstrap, jQuery, JavaScript
- **Architecture Pattern:** Repository & Unit of Work, Dependency Injection

---

##  Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing.

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or another compatible IDE.
- Microsoft SQL Server

### Installation Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/mohamedtalaat2003/LifeQuest.git
   cd LifeQuest
   ```

2. **Configure the Database:**
   - Open `appsettings.json` in the `LIfeQuest.PresentationLayer` and update the `DefaultConnection` string to point to your local SQL Server instance.

3. **Apply Migrations and Seed Data:**
   - Open the **Package Manager Console (PMC)** in Visual Studio.
   - Set the Default Project to `LifeQuest.DAL`.
   - Run the following command to create the database:
     ```powershell
     Update-Database
     ```
   *(Note: The application includes a `DataSeeder` that will automatically populate the database with default Categories, Challenges, Badges, and an Admin account upon matching predefined criteria).*

4. **Run the Application:**
   - Set `LIfeQuest.PresentationLayer` as the Startup Project.
   - Press `F5` or click **Run** in Visual Studio.

---

##  How to Use (Demo Flow)

1. **Register/Login:** Create a new user account or log in with the default admin account.
2. **Dashboard:** Start at the User Dashboard to see your base stats (Level 1, 0 Points).
3. **Explore Challenges:** Navigate to the "Challenges" page and join one that interests you (e.g., "10,000 Steps Daily").
4. **Log Progress:** Submit your daily entry for the challenge and watch your points increase.
5. **Level Up:** Keep logging progress to level up and earn badges!

---

##  Contributing
Contributions, issues, and feature requests are welcome!

##  License
This project is open-source and available under the MIT License.
