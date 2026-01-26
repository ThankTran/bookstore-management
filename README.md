# 📚 Bookstore Management System

> A modern desktop application for managing bookstore operations, built with **C# (.NET Framework)** and **Microsoft SQL Server**.

---

## 📖 Overview

**Bookstore Management System** is a comprehensive Windows desktop application designed to support bookstore staff in managing daily business operations including books, customers, invoices, inventory, and revenue statistics.

The project is developed following **clean architecture principles** and applies the **MVVM (Model – View – ViewModel)** pattern to ensure scalability, maintainability, and separation of concerns.

- 🎯 Target users: Administrator, Sales Manager, Sales Staff, Inventory Manager, Customer Manager 
- 🖥 Platform: Windows Desktop (WPF)
- 🧠 Purpose: Educational project & practical software engineering exercise

---

## ✨ Features

### 🔐 Authentication & Authorization
- Secure login system with password hashing (SHA-256)
- Role-based access control (5 distinct roles)
- Permission-based feature navigation
- Session management

### 📚 Book Management
- Add, update, delete books with soft delete support
- Search books by name
- Manage publishers and pricing
- Track stock levels with low-stock alerts
- Export book lists to CSV

### 👥 Customer Management
- Comprehensive customer profile management
- View detailed purchase history
- Loyalty points tracking and management

### 🧾 Invoice & Sales Management
- Create and manage sales orders
- Track order details with line items
- Support multiple payment methods (Cash, Card, Bank Transfer, Debit Card)
- Apply discounts and calculate totals
- Invoice printing and export functionality
- Order history and filtering by date range

### 📦 Inventory Management
- Import bill creation and tracking
- Low stock and out-of-stock alerts
- Publisher-based inventory tracking
- Import history and analytics

### 📊 Statistics & Reports
- Revenue statistics with date range filtering
- Daily, monthly, and yearly revenue tracking
- Best-selling and lowest-selling books analysis
- Inventory summary and valuation
- Publisher import analytics
- Customer purchase ratio (walk-in vs. members)
- Export reports to CSV

### 🧩 Additional Features
- Responsive dialog-based CRUD operations
- Data validation and error handling
- Print support for invoices and reports

---

## 🛠 Technology Stack

### Runtime Environment
- Windows 10 / 11  
- .NET Framework 4.7.2+  
- Database: Microsoft SQL Server 2019+

### Development Tools
- IDE: Visual Studio 2022
- Database Management: SQL Server Management Studio (SSMS)
- Version Control: Git, GitHub

### Core Technologies
- UI Framework: WPF (Windows Presentation Foundation)
- Database ORM: Entity Framework 6.4.4
- Object Mapping: AutoMapper 10.1.1
- MVVM Toolkit: CommunityToolkit.Mvvm 8.2.1
- Database Testing: NUnit 3.13.3, NUnit3TestAdapter 4.5.0

---

## 📦 Key NuGet Packages

| Package                                   | Version | Purpose                                      |
|-------------------------------------------|---------|----------------------------------------------|
| AutoMapper                                | 10.1.1  | Object-to-object mapping for DTOs            |
| CommunityToolkit.Mvvm                    | 8.2.1   | MVVM pattern implementation helpers          |
| EntityFramework                           | 6.4.4   | Database ORM and migrations                  |
| Microsoft.EntityFrameworkCore.Tools      | 10.0.1  | EF Core CLI tools                            |
| NUnit                                     | 3.13.3  | Unit testing framework                      |
| NUnit3TestAdapter                        | 4.5.0   | NUnit test adapter for Visual Studio         |
| ClosedXML                                 | 0.105.0 | Excel file generation                       |
| DocumentFormat.OpenXml                   | 3.4.1   | Office document manipulation                |
| LiveCharts.Wpf                            | 0.9.7   | Interactive charts and graphs               |
| Microsoft.Extensions.DependencyInjection | 10.0.2  | Dependency injection container              |
| Moq                                       | 4.20.72 | Mocking framework for unit tests            |


---

## 📂 Project Structure

```text
BookstoreManagement/
│
├── Core/                    # Core business logic
│   ├── Constants/           # Application constants
│   ├── Enums/               # Enumeration types
│   ├── Exceptions/          # Custom exceptions
│   ├── Interfaces/          # Core interfaces
│   ├── Results/             # Result pattern implementation
│   ├── Utils/               # Utility classes
│   
├── Data/                    # Data access layer
│   ├── Context/             # EF DbContext configuration
│   └── Repositories/        # Repository interfaces & implementations
│
├── Models/                  # Domain models
│
├── DTOs/                    # Data Transfer Objects
│
├── Services/                # Business logic layer
│   ├── Interfaces/          # Service interfaces
│   └── Implementations/     # Service implementations
│
├── Migrations/              # EF Database migrations
│
├── Helpers/                 # Helper utilities
│
├── Presentation/            # UI Layer
│   ├── AppResources/        # UI Resources
│   ├── Converters/          # Value converters
│   ├── Views/               # XAML Views
│   └── ViewModels/          # MVVM ViewModels
│
├── Tests/                   # Unit tests
│
├── App.config               # Application configuration
├── App.xaml                 # Application entry point
└── BookstoreManagement.sln

```

---

## ⚙️ Installation Guide

## Prerequisites
- Visual Studio 2022 or later
- .NET Framework 4.7.2 or later
- SQL Server 2019 or later
- SQL Server Management Studio (SSMS) - recommended
  
### 1️⃣ Clone the Repository

```bash
git clone https://github.com/yourusername/bookstore-management.git
cd bookstore-management
```

### 2️⃣ Database Setup
1. Open SQL Server Management Studio (SSMS)

2. Connect to your SQL Server instance

3. Open App.config and update the connection string:
```xml
<connectionStrings>
  <add name="BookstoreConnection"
       connectionString="Data Source=YOUR_SERVER_NAME;
                         Initial Catalog=BookstoreDB;
                         Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
4. Open **Package Manager Console** in Visual Studio:  
   - Tools → NuGet Package Manager → Package Manager Console  

5. Run the migration commands:
```powershell
# Create/update the database
Update-Database

# The seed data will be automatically populated
```

### 3️⃣ Build & Run

1. Open the solution file **`bookstore_Management.sln`** in Visual Studio 2022  

2. Restore NuGet packages:  
   - Right-click on **Solution** → **Restore NuGet Packages**  

3. Build the solution:  
   - **Build** → **Build Solution** (`Ctrl + Shift + B`)  

4. Run the application:  
   - **Debug** → **Start Debugging** (`F5`)

### 4️⃣ Default Login Credentials

After running migrations, use these credentials to login:

| Role              | Username             | Password          |
|-------------------|----------------------|-------------------|
| Administrator     | `admin`              | `Admin@123`       |
| Customer Manager  | `cust.manager`       | `CustManager@123` |
| Inventory Manager | `inventory.manager01`| `Inventory@123`   |
| Sales Staff       | `sales.staff01`      | `SalesStaff@123`  |

---

## 🚀 Usage Guide

### 🔑 Login
- Launch the application
- Enter username and password
- The system redirects based on user role

### 🧭 Navigation
- Use the sidebar on the left to navigate between features
- Feature availability depends on user permissions
- Current user info is displayed at the top of the sidebar

### 👨‍💼 Administrator

Full system access including:

- Manage books, customers, staff, and publishers  
- Create and manage invoices and orders  
- View comprehensive statistics and reports  
- Manage user accounts and permissions  

### 👩‍💼 Manager Roles

#### Sales Manager

- Create and manage sales orders  
- View customer information  
- Access sales reports and statistics  

#### Inventory Manager

- Manage book inventory and stock levels  
- Create import bills from publishers  
- Monitor stock alerts and reports  

#### Customer Manager

- Full customer profile management  
- Update membership tiers and loyalty points  
- View customer purchase history

### 👥 Sales Staff

- Process sales and create orders
- View customer information
- Check inventory levels

---

## 🤝 Contributors
- **[Trần Thị Hồng Thanh](https://github.com/ThankTran)**  
- **[Phạm Hoàng Gia Hiển](https://github.com/hienpham0344)**
- **[Nguyễn Ái My](https://github.com/aimynguyen)**

