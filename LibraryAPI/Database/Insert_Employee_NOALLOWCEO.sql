--COULD PROBABLY USE AN BETTER DATABASE CONSTRAINT, but no time!
use library;
INSERT INTO Employee
    ("first_name","last_name","salary","is_ceo","is_manager","manager_id")
select 'Fiiik', 'RenSkit', -1.2, 'true', 'false', null
WHERE NOT (SELECT COUNT(id)
FROM Employee
WHERE is_ceo='true')>0;

