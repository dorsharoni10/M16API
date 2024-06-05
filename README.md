# M16API

### 1. Clone the Repository

Clone the repository to your local machine using the following command:
```bash
git clone https://github.com/dorsharoni10/M16API.git
```

### 2. Restore Dependencies
Navigate to the root directory of the project and restore all dependencies:
```bash
dotnet restore
```
### 3. Create the Database Migration
To create the initial database migration using Entity Framework Core, run the following command:
```bash
dotnet ef migrations add InitialCreate
```

### 4. Update the Database Schema
```bash
dotnet ef database update
```
