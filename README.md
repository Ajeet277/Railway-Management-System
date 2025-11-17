# 🚂 Railway Management System

<div align="center">

![Railway](https://img.shields.io/badge/Railway-Management-blue?style=for-the-badge&logo=train&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-18+-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

*A comprehensive full-stack web application for online train booking and railway management operations*

</div>

---

## 🌟 **Features**

### 👤 **User Experience**
- 🔐 **Secure Authentication** - JWT-based login system with BCrypt password hashing
- 🔍 **Smart Train Search** - Advanced filtering by route, date, and train number
- 🎫 **Seamless Booking** - Complete reservation workflow with real-time availability
- 💳 **Payment Integration** - Secure payment processing with confirmation emails
- 📧 **Email Notifications** - Instant booking confirmations and updates
- 📱 **Responsive Design** - Modern glassmorphism UI across all devices

### 👨‍💼 **Admin Dashboard**
- 📊 **Analytics Overview** - Real-time statistics and system insights
- 🚆 **Train Management** - Complete CRUD operations for train schedules
- 📋 **Booking Oversight** - Monitor and manage all passenger reservations
- 👥 **User Administration** - Comprehensive user account management

---

## 🛠️ **Tech Stack**

<table>
<tr>
<td align="center" width="50%">

### **Backend**
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)

</td>
<td align="center" width="50%">

### **Frontend**
![Angular](https://img.shields.io/badge/Angular%2018+-DD0031?style=flat-square&logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=flat-square&logo=typescript&logoColor=white)
![RxJS](https://img.shields.io/badge/RxJS-B7178C?style=flat-square&logo=reactivex&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat-square&logo=css3&logoColor=white)

</td>
</tr>
</table>

---

## 🚀 **Quick Start**

### **Prerequisites**
```bash
✅ .NET 8.0 SDK
✅ Node.js (v18+)
✅ SQL Server
✅ Angular CLI

##🔧 **Backend Setup**
# Clone repository
git clone https://github.com/Ajeet277/Railway-Management-System.git
cd Railway-Management-System/RailwayManagement

# Configure database connection
update appsettings.json
# Update connection string in appsettings.json

# Run migrations
dotnet ef database update

# Start API server
dotnet run

##🎨 **Frontend Setup**
# Navigate to frontend
cd ../RailwayManagement-Frontend

# Install dependencies
npm install

# Start development server
ng serve

🏗️ Railway-Management-System/
├── 🔧 RailwayManagement/              # ASP.NET Core Web API
│   ├── 🎮 Controllers/                # API Controllers
│   ├── 📊 Models/                     # Entity Models
│   ├── 📦 DTOs/                       # Data Transfer Objects
│   ├── ⚙️ Services/                   # Business Logic
│   ├── 🗄️ Data/                       # Database Context
│   └── 🛡️ Middleware/                 # Custom Middleware
├── 🎨 RailwayManagement-Frontend/     # Angular 18+ Frontend
│   ├── 🧩 src/app/components/         # UI Components
│   ├── 🔗 src/app/services/           # HTTP Services
│   ├── 📋 src/app/models/             # TypeScript Models
│   └── 🛡️ src/app/guards/             # Route Guards
└── 🧪 Tests/                          # Unit Test Projects

##🧪 **Testing**
# Backend Tests
cd RailwayManagement.Tests
dotnet test

## 🍴 Fork the repository

## 🌿 Create your feature branch (git checkout -b feature/AmazingFeature)

## 💾 Commit your changes (git commit -m 'Add some AmazingFeature')

## 📤 Push to the branch (git push origin feature/AmazingFeature)

## 🔄 Open a Pull Request

## 👨‍💻 Author
# Ajeet Shyanavad
https://github.com/Ajeet277
mailto:ajeetshyanavad08@gmail.com

# Full Stack Developer | .NET & Angular Enthusiast

🌟 Show your support
Give a ⭐️ if this project helped you!

Made with ❤️ by Ajeet Shyanavad




## ⚙️ **Configuration Setup**

### **📝 appsettings.json Configuration**

Create `appsettings.json` file in the `RailwayManagement` folder with the following structure:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RailwayManagementDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "your-jwt-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "RailwayManagementAPI",
    "Audience": "RailwayManagementUsers"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "EnableSsl": true
  }
}




🔧 Configuration Details
🗄️ Database Connection
Replace YOUR_SERVER_NAME with your SQL Server instance:

Local SQL Server: Server=localhost;Database=RailwayManagementDB;Trusted_Connection=true;TrustServerCertificate=true;

SQL Server Express: Server=.\\SQLEXPRESS;Database=RailwayManagementDB;Trusted_Connection=true;TrustServerCertificate=true;

Named Instance: Server=YOUR_PC_NAME\\SQLEXPRESS;Database=RailwayManagementDB;Trusted_Connection=true;TrustServerCertificate=true;

🔐 JWT Authentication
Key: Must be at least 32 characters long for security

Issuer: API identifier (keep as provided)

Audience: Client identifier (keep as provided)

📧 Email Configuration (Optional)
For email notifications, configure SMTP settings:

Gmail Users:

Use your Gmail address

Generate App Password from Google Account settings

Use the App Password (not your regular password)

Other Providers: Update SMTP server and port accordingly

🚨 Important Notes
⚠️ Security Warning: Never commit appsettings.json to version control as it contains sensitive information.

📋 Database Setup: Run dotnet ef database update after configuring the connection string to create the database.

🔑 JWT Key: Generate a secure random key for production use.
