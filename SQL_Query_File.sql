CREATE TABLE CompanyWise_HR_Info (
    SNo INT PRIMARY KEY IDENTITY(1,1),  -- Auto-incrementing primary key
    Name NVARCHAR(100) NOT NULL,       -- HR's full name
    Email NVARCHAR(255) NOT NULL UNIQUE, -- Unique email address
    Title NVARCHAR(100) NOT NULL,      -- Job title (e.g., HR Manager)
    Company NVARCHAR(100) NOT NULL     -- Company name
);
ALTER TABLE dbo.CompanyWise_HR_Info
ALTER COLUMN Name NVARCHAR(255) NULL;

SELECT * FROM  CompanyWise_HR_Info;

ALTER TABLE dbo.CompanyWise_HR_Info
ALTER COLUMN Email NVARCHAR(255) NULL;

ALTER TABLE dbo.CompanyWise_HR_Info
ALTER COLUMN  Title NVARCHAR(255) NULL;

ALTER TABLE dbo.CompanyWise_HR_Info
ALTER COLUMN  Company NVARCHAR(255) NULL;

-- Step 1: Drop existing unique constraint/index
ALTER TABLE dbo.CompanyWise_HR_Info
DROP CONSTRAINT UQ__CompanyW__A9D1053418337E2C;

-- Step 2: Recreate as filtered index (ignores NULLs)
CREATE UNIQUE NONCLUSTERED INDEX UQ__CompanyWise_HR_Info_Email
ON dbo.CompanyWise_HR_Info(Email)
WHERE Email IS NOT NULL;