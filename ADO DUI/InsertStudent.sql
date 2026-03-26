if exists (select [name] from sysobjects 
            where [name] = 'InsertStudent')
    drop procedure InsertStudent
go

create procedure InsertStudent
    @first      nvarchar(24) = null,
    @last       nvarchar(24) = null,
    @school     int          = null,
    @newId      int          output,
    @status     nvarchar(100) output
as

if @first is null
begin
    set @status = 'First name was null!'
    return -1
end

if @last is null
begin
    set @status = 'Last name was null!'
    return -2
end

if @school is null
begin
    set @status = 'School id was null!'
    return -3
end

insert into Students (first_name, last_name, school_id)
values (@first, @last, @school)

set @newId = SCOPE_IDENTITY()

if @@error <> 0
begin
    set @status = 'Insert failed!'
    return -4
end

set @status = 'Insert successful!'
return 0
go