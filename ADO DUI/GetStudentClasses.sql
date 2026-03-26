if exists (select [name] from sysobjects 
            where [name] = 'GetStudentClasses')
    drop procedure GetStudentClasses
go

create procedure GetStudentClasses
    @studentId int = null
as

if @studentId is null
    return -1

select  c.class_id,
        c.class_desc,
        c.days,
        c.start_date,
        i.instructor_id,
        i.first_name,
        i.last_name
from Classes c
join class_to_student cs on c.class_id = cs.class_id
join Instructors i on c.instructor_id = i.instructor_id
where cs.student_id = @studentId

return 0
go