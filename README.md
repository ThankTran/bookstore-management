# 📚 Bookstore Management System

> A modern desktop application for managing bookstore operations, built with **C# (.NET Framework)** and **Microsoft SQL Server**.

---

## 📖 Overview

**Bookstore Management System** is a Windows desktop application designed to support bookstore staff in managing daily business operations such as books, customers, invoices, inventory, and revenue statistics.

The project is developed following **clean architecture principles** and applies the **MVVM (Model – View – ViewModel)** pattern to ensure scalability, maintainability, and separation of concerns.

- 🎯 Target users: Admin, Manager, Staff  
- 🖥 Platform: Windows Desktop  
- 🧠 Purpose: Educational project & practical software engineering exercise

---

## ✨ Features

### 🔐 Authentication & Authorization
- Secure login system
- Role-based access control (Admin / Staff)
- Permission-based feature navigation

### 📚 Book Management
- Add, update, delete books
- Search books by name
- Categorize books
- Manage publishers and pricing

### 👥 Customer Management
- Manage customer profiles
- View purchase history
- Membership tiers & loyalty points
- Real-time search

### 🧾 Invoice & Sales Management
- Create and manage invoices
- Track order details
- Calculate total revenue
- Invoice printing support

### 📊 Statistics & Reports
- Revenue statistics
- Best-selling books
- Customer spending analysis
- Export reports to Excel

### 🧩 Additional Features
- Export data to Excel
- Print support
- Modern, clean UI
- Dialog-based CRUD operations

---

## 🛠 Technology Stack

### Runtime Environment
- Windows 10 / 11  
- .NET Framework 4.7.2+  
- Microsoft SQL Server 2019+  

### Development Tools
- Visual Studio 2022  
- SQL Server Management Studio (SSMS)  

### Architecture & Patterns
- MVVM (Model – View – ViewModel)
- Repository Pattern
- Service Layer
- Manual Dependency Injection

---

## 📂 Project Structure

```text
BookstoreManagement/
│
├── Core/                    # Constants, Enums, Exceptions, Interfaces, Results, Utils
├── Data/
│   ├── Context/             # DbContext
│   └── Repositories/        # Data access layer
│
├── Models/                  # Domain models
│
├── DTOs/                    # Request / Response DTOs
│
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│
├── Migrations/              # Database migrations
│
├── Helpers/                 # Print / Export helpers
│
├── Presentation/
│   ├── AppResources/        # Styles, Colors, Fonts
│   ├── Converters/          # Value converters
│   ├── Views/               # XAML Views, Dialogs
│   └── ViewModels/          # MVVM ViewModels
│
├── Tests/                   # Unit tests
│
├── App.xaml
└── BookstoreManagement.sln

```

---

## ⚙️ Installation Guide

### 1️⃣ Database Setup

### 2️⃣ Build & Run

---

## 🚀 Usage

### 🔑 Login
- Launch the application
- Enter user credentials
- The system redirects based on the user role

### 🧭 Navigation
- Sidebar-based navigation
- Feature availability depends on user permissions

### 🛠 Admin
- Manage books, customers, and system users
- View statistics and reports

### 👩‍💼 Staff
- Manage invoices
- Handle sales operations
- View customer information
