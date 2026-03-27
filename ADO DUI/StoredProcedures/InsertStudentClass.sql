if exists (select [name] from sysobjects 
            where [name] = 'InsertStudentClass')
    drop procedure InsertStudentClass
go

create procedure InsertStudentClass
    @studentId int = null,
    @classId   int = null,
    @status    nvarchar(100) output
as

if @studentId is null
begin
    set @status = 'Student ID missing!'
    return -1
end

if @classId is null
begin
    set @status = 'Class ID missing!'
    return -2
end

insert into class_to_student (student_id, class_id)
values (@studentId, @classId)

if @@error <> 0
begin
    set @status = 'Insert failed!'
    return -3
end

set @status = 'Insert successful!'
return 0
go