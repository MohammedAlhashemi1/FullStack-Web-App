if exists (select [name] from sysobjects 
            where [name] = 'UpdateStudent')
    drop procedure UpdateStudent
go

create procedure UpdateStudent
    @id      int          = null,
    @first   nvarchar(24) = null,
    @last    nvarchar(24) = null,
    @school  int          = null,
    @status  nvarchar(100) output
as

if @id is null
begin
    set @status = 'Student ID missing!'
    return -1
end

if @first is null
begin
    set @status = 'First name missing!'
    return -2
end

if @last is null
begin
    set @status = 'Last name missing!'
    return -3
end

if @school is null
begin
    set @status = 'School ID missing!'
    return -4
end

update Students
set first_name = @first,
    last_name  = @last,
    school_id  = @school
where student_id = @id

if @@error <> 0
begin
    set @status = 'Update failed!'
    return -5
end

set @status = 'Update successful!'
return 0
go