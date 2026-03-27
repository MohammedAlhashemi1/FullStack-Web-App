if exists (select [name] from sysobjects 
            where [name] = 'DeleteStudent')
    drop procedure DeleteStudent
go

create procedure DeleteStudent
    @id      int = null,
    @status  nvarchar(100) output
as

if @id is null
begin
    set @status = 'Student ID missing!'
    return -1
end

-- FIRST delete from Results table
delete from Results where student_id = @id

-- THEN delete from class_to_student table
delete from class_to_student where student_id = @id

-- FINALLY delete from Students
delete from Students where student_id = @id

if @@error <> 0
begin
    set @status = 'Delete failed!'
    return -2
end

set @status = 'Delete successful!'
return 0
go