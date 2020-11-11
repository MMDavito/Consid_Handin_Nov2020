Update is complex.
Since if no change should not affect anything but the manager (since already exists)
I need to figure out 3.5 cases (4th being UPDATE isManager)
I have no action on UPDATE constraint for manager_id.
SO cant change someone manageing others, without deleting or changeing them first.


1. IS CEO
--------------------------------
  if (employee.isCEO)//This if-else could been avoided with better database constraint design (would been safer but taken longer)
                    {
                        sc.CommandText = @"
                    UPDATE Employee
                    SET
                        first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo='true',
                        is_manager='false',
                        manager_id=NULL
                    WHERE -- Following is to allow for updating manager to ceo OR employee to ceo, ALTERNITEVLY simply updating ceo (if is_ceo='true')
                    id=@ID AND(
                        is_ceo='true'
                        OR NOT 
                        (SELECT COUNT(id)
                            FROM Employee as e WHERE e.is_ceo= 'true'
                        )> 0);
                    ";
                    }
--------------------------------
2. is manager
                    else if (employee.isManager)
                    {//Cannot allow manager to be managed by employee, but it can however be without manager.
                        sc.CommandText = @"
                     UPDATE Employee
                    SET
                                            first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo=false,
                        is_manager=true,
                        manager_id=@manager_id)
                    WHERE
                    id=@ID AND NOT
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=0 AND e.is_ceo=0)
                        )> 0;
";//Where not regular employee (employee.isCeo ==false && employee.isManager == false), it should however allow manager_ID=null
                    }
-------------------------------
@"
                    UPDATE LibraryItem SET 
                    borrower = @borrower,
                    borrow_date = @borrow_date,
                    is_borrowable = 'false'

                    WHERE 
                    id = @ID
                    AND is_borrowable = 'true'; 
                    ";

------------------------------
3. is regular employee 
KLAR
---------------------------


                    else//IF NOT manager_id == NULL
                    {//Employee cant be managed by ceo
                        sc.CommandText = @"
                    UPDATE Employee
                    SET
                        first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo=false,
                        is_manager=false,
                        manager_id=@manager_id)
                    WHERE
                        id=@ID
                        AND
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=1)
                        )> 0;
                    ";
                    }