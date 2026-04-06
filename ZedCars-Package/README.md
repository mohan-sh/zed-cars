# DISCLAIMER

This application is for demonstration and learning purposes only. It is not intended for production use. The ZedCars application was created to showcase ASP.NET MVC 3 development techniques and should be used solely for educational purposes.

# ZedCars - Vehicle Inventory Management System

ZedCars is a web-based vehicle inventory management system built with ASP.NET MVC 3. It allows users to browse vehicle listings and administrators to manage inventory, users, and generate reports.

## Features

- Vehicle inventory browsing and filtering
- User authentication and role-based access control
- Admin dashboard with analytics
- Vehicle management (add, edit, delete)
- User management
- Reporting functionality

## Setup Instructions

### Prerequisites

- **Windows OS**: Windows 7 or later
- **Visual Studio**: Visual Studio 2010 or Visual Studio 2019 with .NET Framework 4.0 targeting pack
- **ASP.NET MVC**: Version 3.0.0.0 (specific version required)
- **MySQL**: Version 5.7 or 8.0
- **.NET Framework**: 4.0 or 4.8

### Step 1: Install Required Software

1. **Install Visual Studio**
   - Download and install Visual Studio 2019 Community Edition
   - During installation, select the ".NET desktop development" workload
   - In Individual Components, select ".NET Framework 4.0 targeting pack"

2. **Install ASP.NET MVC 3**
   - Download ASP.NET MVC 3 from: https://www.microsoft.com/en-us/download/details.aspx?id=1491
   - Run the installer and follow the prompts

3. **Install MySQL**
   - Download MySQL 8.0 from: https://dev.mysql.com/downloads/mysql/
   - Install with default settings
   - Set root password during installation
   - Add MySQL to system PATH

### Step 2: Database Setup

1. **Create Database**
   ```sql
   CREATE DATABASE zoomcars_inventory;
   ```

2. **Create User**
   ```sql
   CREATE USER 'zoomcars_user'@'localhost' IDENTIFIED BY 'admin123';
   GRANT ALL PRIVILEGES ON zoomcars_inventory.* TO 'zoomcars_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

3. **Run Database Scripts**
   - Navigate to the `database/init` folder in the project
   - Run the following scripts in order:
     ```
     mysql -u zoomcars_user -p zoomcars_inventory < 01-create-tables.sql
     mysql -u zoomcars_user -p zoomcars_inventory < 02-seed-data.sql
     ```

### Step 3: Application Setup

1. **Clone or Download the Repository**
   ```
   git clone https://github.com/yourusername/zedcars.git
   ```

2. **Open the Solution**
   - Open Visual Studio
   - Navigate to File > Open > Project/Solution
   - Select the ZedCars.sln file

3. **Restore NuGet Packages**
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"

4. **Update Connection String**
   - Open Web.config
   - Update the connection string if needed:
     ```xml
     <connectionStrings>
       <add name="ZedCarsDB" connectionString="Server=localhost;Database=zoomcars_inventory;Uid=zoomcars_user;Pwd=admin123;Port=3306;SslMode=None;AllowUserVariables=True;" providerName="MySql.Data.MySqlClient" />
     </connectionStrings>
     ```

5. **Build the Solution**
   - Select Build > Clean Solution
   - Select Build > Rebuild Solution

### Step 4: Run the Application

1. **Start the Application**
   - Press F5 or click the "Start" button
   - The application should launch in your default browser

2. **Login Credentials**
   - Admin User:
     - Username: admin
     - Password: admin123
   - Regular User:
     - Username: user1
     - Password: password1

## Troubleshooting

### Common Issues

1. **MVC Version Mismatch**
   - Ensure you have ASP.NET MVC 3.0.0.0 installed
   - Check that the System.Web.Mvc.dll reference points to version 3.0.0.0

2. **Database Connection Issues**
   - Verify MySQL is running
   - Check connection string in Web.config
   - Ensure the user has proper permissions

3. **Build Errors**
   - Make sure all required NuGet packages are restored
   - Verify .NET Framework 4.0/4.8 is installed

### Additional Resources

- ASP.NET MVC 3 Documentation: https://docs.microsoft.com/en-us/aspnet/mvc/mvc3
- MySQL Documentation: https://dev.mysql.com/doc/

## License

This project is licensed for educational purposes only. Not for commercial use.
