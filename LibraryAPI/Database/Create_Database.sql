-- sqlcmd -S 127.0.0.1 -U SA 
--select name from sys.databases;go
--seems like microsoft is unsafe by design (would select name but that would take 2 hours to write): 
--CREATE DATABASE testDB IF NOT EXISTS;
--I will allow the database take care of 200 char long unicode strings that are allowed (although 400 bytes in length)
--Could have let it be NVARCHAR(4k) but had no time to check if it was static length or dynamic.
--Still have not understood why nvarchar with no arguments equals to nvarchar(1) if size is dynamic..
CREATE DATABASE library;
--go
use library;
CREATE TABLE Category(
    id int IDENTITY(1,1) PRIMARY KEY,
    category nvarchar(200) NOT NULL UNIQUE--100 charachter category is unheard of
    );
--go
CREATE TABLE LibraryItem(
    id int IDENTITY(1,1) PRIMARY KEY,
    category_id int,
    title nvarchar(200),
    author nvarchar(200),
    pages int,--DEFAULT NULL
    run_time_minutes int,--DEFAULT NULL
    is_borrowable bit,--1 ==true? 0 == False?
    borrower nvarchar(200),
    borrow_date date ,--DEFAULT NULL
    type nvarchar(200),
    
    CONSTRAINT categoryFK FOREIGN KEY (category_id)
    REFERENCES Category (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

);
CREATE TABLE Employee(
    id int IDENTITY(1,1) PRIMARY KEY,
    first_name nvarchar(200),--abit long, no names are 200 bytes long, but as I understand nvarchar inserting 200 bytes in NVARCHAR(4000) will take 200bytes.
    last_name nvarchar(200),
    salary decimal,
    is_ceo bit,
    is_manager bit,
    manager_id int --DEFAULT NULL
);--Singular to stick with ONE naming convention instead of several.
--go
