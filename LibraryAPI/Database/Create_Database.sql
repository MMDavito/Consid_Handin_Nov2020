--select name from sys.databases;go
--seems like microsoft is unsafe by design (would select name but that would take 2 hours to write): 
--CREATE DATABASE testDB IF NOT EXISTS;

CREATE DATABASE library;
go
use library;
CREATE TABLE Category(
    id int PRIMARY KEY,
    );
go
CREATE TABLE LibraryItem();
CREATE TABLE Employee();--Singular to stick with ONE naming convention instead of several.
go