# Tehnicharche
## About the Project
An open-source web platform for publishing and discovering listings for hands-on technical services. Users can create listings offering practical services such as soldering, appliance repair, electronics maintenance, electrical work, and other tasks that require technical skills.

---

## Features

- Create and publish technical service listings  
- User registration and authentication  
- Manage your own listings  
- Browse available services  
- Clean layered architecture (Data, Services, Web, ViewModels)  
- 🔜 Planned: Advanced filtering system for services (by category, location, etc.)

---

## Built With

- C#
- ASP.NET
- .NET SDK
- HTML / CSS
- Entity Framework

---

## Project Structure

```
Tehnicharche/
│
├── Tehnicharche.Data
├── Tehnicharche.Data.Models
├── Tehnicharche.GCommon
├── Tehnicharche.Services.Core
├── Tehnicharche.ViewModels
├── Tehnicharche.Web
└── Tehnicharche.sln
```

- **Data** – Database access and persistence logic  
- **Data.Models** – Domain models  
- **Services.Core** – Business logic  
- **ViewModels** – Models used for UI representation  
- **Web** – Web application (controllers, views, frontend)  

---

## Getting Started

### Prerequisites

Make sure you have installed:

- .NET 8.0
- Visual Studio / VS Code
- SQL Server (or configured database provider)

---

### Installation

Clone the repository:

```bash
git clone https://github.com/StunnyBG/Tehnicharche.git
cd Tehnicharche
```

Restore dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

---

### Run the Application

```bash
dotnet run --project Tehnicharche.Web
```

The application should now be running on:

```
https://localhost:5001
```

(or the port shown in your terminal)

---

## Future Improvements

- Advanced service filtering
- Search functionality
- Reviews and rating system
- Admin dashboard

---

## License

This project is licensed under the MIT License.

---

## Author

GitHub: https://github.com/StunnyBG
