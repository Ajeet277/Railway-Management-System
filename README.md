Railway Management System
A comprehensive full-stack web application for online train booking and railway management operations.

🚀 Tech Stack
Backend
ASP.NET Core Web API - RESTful API development

Entity Framework Core - Code First approach with SQL Server

JWT Authentication - Secure user authentication and authorization

BCrypt - Password hashing for enhanced security

SMTP Integration - Email notifications for bookings

Frontend
Angular 18+ - Modern frontend with standalone components

TypeScript - Type-safe development

RxJS - Reactive programming for HTTP operations

Modern CSS - Glassmorphism UI design with responsive layout

Database
SQL Server - Primary database with Entity Framework migrations

Code First Approach - Database schema management

Testing
xUnit & NUnit - Comprehensive unit testing frameworks

Postman - API endpoint testing and documentation

✨ Features
User Features
User Registration & Login - Secure authentication with JWT tokens

Train Search - Advanced search by route and train number

Booking Management - Complete booking workflow with seat selection

Payment Integration - Mock payment gateway with confirmation

Email Notifications - Booking confirmations and updates

Booking History - View and manage past reservations

Cancellation System - Cancel bookings with proper status management

Admin Features
Admin Dashboard - Comprehensive system overview with statistics

Train Management - Add, edit, and remove train schedules

Booking Oversight - Monitor all passenger reservations

User Management - Administrative controls for user accounts

Technical Features
Role-Based Authorization - Separate User and Admin access levels

Global Exception Handling - Centralized error management

Input Validation - DTO-level validation with custom attributes

Responsive Design - Mobile-friendly interface

Modern UI/UX - Glassmorphism effects with smooth animations

🛠️ Setup Instructions
Prerequisites
.NET 8.0 SDK

Node.js (v18+)

SQL Server

Angular CLI

Backend Setup
Clone the repository

git clone https://github.com/Ajeet277/Railway-Management-System.git
cd Railway-Management-System

Copy
Configure Database

cd RailwayManagement
# Copy appsettings.Example.json to appsettings.json
# Update connection string with your SQL Server details

Copy
bash
Run Database Migrations

dotnet ef database update

Copy
bash
Start Backend API

dotnet run

Copy
bash
API will be available at: https://localhost:5193

Frontend Setup
Navigate to Frontend Directory

cd RailwayManagement-Frontend

Copy
bash
Install Dependencies

npm install

Copy
bash
Start Angular Application

ng serve

Copy
bash
Frontend will be available at: http://localhost:4200

📁 Project Structure
Railway-Management-System/
├── RailwayManagement/              # ASP.NET Core Web API
│   ├── Controllers/                # API Controllers
│   ├── Models/                     # Entity Models
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Services/                   # Business Logic Services
│   ├── Data/                       # Database Context & Migrations
│   └── Middleware/                 # Custom Middleware
├── RailwayManagement-Frontend/     # Angular 18+ Frontend
│   ├── src/app/components/         # Angular Components
│   ├── src/app/services/           # HTTP Services
│   ├── src/app/models/             # TypeScript Models
│   └── src/app/guards/             # Route Guards
└── Tests/                          # Unit Test Projects

Copy
🔧 Configuration
Database Configuration
Update appsettings.json with your SQL Server connection string:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RailwayManagementDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}

Copy
json
JWT Configuration
Configure JWT settings for authentication:

{
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "RailwayManagementAPI",
    "Audience": "RailwayManagementUsers"
  }
}

Copy
json
🧪 Testing
Backend Testing
cd RailwayManagement.Tests
dotnet test

Copy
bash
API Testing
Import Postman collection for endpoint testing

Swagger UI available at: https://localhost:5193/swagger

🚀 Deployment
Backend Deployment
Configure production connection strings

Set up HTTPS certificates

Deploy to IIS or Azure App Service

Frontend Deployment
ng build --prod
# Deploy dist/ folder to web server

Copy
bash
🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit your changes (git commit -m 'Add some AmazingFeature')

Push to the branch (git push origin feature/AmazingFeature)

Open a Pull Request

📝 License
This project is licensed under the MIT License - see the LICENSE file for details.

👨‍💻 Author
Ajeet Kumar

GitHub: @Ajeet277

Email: mailto:ajeetshyanavad08@gmail.com

🙏 Acknowledgments
ASP.NET Core documentation and community

Angular team for the excellent framework

Entity Framework Core for seamless database operations

All contributors and testers

⭐ Star this repository if you found it helpful!
