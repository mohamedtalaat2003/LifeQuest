# 🌟 LifeQuest 
**Gamified Habit & Challenge Tracker Web Application**

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core_MVC-Blue?style=for-the-badge&logo=asp.net)
![Architecture](https://img.shields.io/badge/Architecture-N--Tier_Architecture-brightgreen?style=for-the-badge)

## 📖 Overview
**LifeQuest** is a comprehensive gamified platform designed to help users track their daily habits and participate in challenges. By incorporating game mechanics like points, levels, and badges, LifeQuest transforms self-improvement into an engaging and rewarding experience. 

The application is built using a clean **N-Tier Architecture** separating the Data Access, Business Logic, and Presentation layers to maintain scalability and maintainability.

## 🏗️ Architecture
The solution is structured into three main layers:
* **LifeQuest.DAL (Data Access Layer):** Manages the database context, models, and implements the **Repository** and **Unit of Work** patterns using Entity Framework Core.
* **LifeQuest.BLL (Business Logic Layer):** Contains the core services and use-cases (e.g., ChallengeService, NotificationService).
* **LIfeQuest.PresentationLayer:** An ASP.NET Core MVC application serving as the UI, handling user interactions, views, and controllers.

## 🚀 Tech Stack
* **Framework:** ASP.NET Core 8.0 MVC
* **Language:** C#
* **ORM:** Entity Framework Core
* **Database:** SQL Server
* **Authentication/Authorization:** ASP.NET Core Identity (Role-Based Access Control)
* **Frontend:** Razor Pages, HTML/CSS, Bootstrap, and JavaScript.

## ✨ Key Features
### 👤 User Features:
* **User Accounts & Dashboard:** Secure registration/login with an interactive personal dashboard.
* **Challenges system:** Browse, join, and complete various challenges.
* **Gamification:** Earn points, unlock badges, and level up as you complete challenges.
* **Progress Tracking:** Log daily progress and view analytics.
* **Real-time Notifications:** Receive dynamic updates on achievements, leveling up, and points earned.

### 🛡️ Admin Features (RBAC):
* **Admin Dashboard:** Exclusive access for administrators.
* **Content Management:** Create, manage, and monitor challenges, categories, and dynamically seed initial levels and badges.
* **User Oversight:** Strict data isolation where users only see their progress, while admins have global system oversight.

## 🧩 Design Patterns 
* **N-Tier Architecture:** Clear separation of concerns.
* **Repository & Unit of Work:** Efficient and transactional database operations.
* **Dependency Injection (DI):** Configured across all external and internal services.

## 🛠️ Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* SQL Server
* Visual Studio 2022 / JetBrains Rider

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/mohamedtalaat2003/LifeQuest.git
   ```
2. Navigate to the `LIfeQuest.PresentationLayer`:
   ```bash
   cd LifeQuest/LIfeQuest.PresentationLayer
   ```
3. Update the `DefaultConnection` string in [appsettings.json](cci:7://file:///c:/Users/RoYaA/Pictures/Pictures/API/E-Commerce%20SaaS/ECommerce/ECommerce/Dapper---CRUD/DapperBiggnerCourse/appsettings.json:0:0-0:0) to your local SQL Server.
4. Update the Database (Using EF Core CLI):
   ```bash
   dotnet ef database update --project ../LifeQuest.DAL --startup-project .
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## 🗺️ Roadmap
- [x] Implement initial N-Tier Architecture setup.
- [x] Add Authentication & Role-Based Access Control (Admin/User).
- [x] Gamification Engine (Points, Levels, Badges).
- [x] Dynamic Notification System.
- [ ] Add Real-time Chat/Community features (e.g., SignalR).
- [ ] Implement robust Unit Testing.

## 🤝 Contributing
Contributions, issues, and feature requests are welcome! Check the [issues page](https://github.com/mohamedtalaat2003/LifeQuest/issues).

---
**Developed with ❤️ by [Mohamed Talaat]**
```
