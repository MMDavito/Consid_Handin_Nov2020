-- sqlcmd -S 127.0.0.1 -U SA 
--select name from sys.databases;go
--seems like microsoft is unsafe by design (would select name but that would take 2 hours to write): 
--CREATE DATABASE testDB IF NOT EXISTS;

CREATE DATABASE library;
go
use library;
CREATE TABLE Category(
    id int PRIMARY KEY,
    category nvarchar
    );
go
CREATE TABLE LibraryItem(
    id int PRIMARY KEY,
    category_id int,
    title nvarchar,
    author nvarchar,
    pages int,--DEFAULT NULL
    run_time_minutes int,--DEFAULT NULL
    is_borrowed bit,--1 ==true? 0 == False?
    borrower nvarchar,
    borrow_date date ,--DEFAULT NULL
    type nvarchar,
    
    CONSTRAINT categoryFK FOREIGN KEY (category_id)
    REFERENCES Category (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

);
CREATE TABLE Employee(
    id int PRIMARY KEY,
    first_name nvarchar,
    last_name nvarchar,
    salary decimal,
    is_ceo bit,
    is_manager bit,
    manager_id int --DEFAULT NULL
);--Singular to stick with ONE naming convention instead of several.
go