if exists (select [name] from sysobjects 
            where [name] = 'GetAllClasses')
    drop procedure GetAllClasses
go

create procedure GetAllClasses
as

select  class_id,
        class_desc
from Classes
order by class_id

return 0
go