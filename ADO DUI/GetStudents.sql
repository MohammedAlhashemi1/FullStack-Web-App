if exists (select [name] from sysobjects 
            where [name] = 'GetStudents')
    drop procedure GetStudents
go

create procedure GetStudents
as

select  student_id,
        first_name,
        last_name,
        school_id
from Students
where first_name like 'E%' or first_name like 'F%'
order by first_name, last_name

return 0
go