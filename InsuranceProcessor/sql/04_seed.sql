if not exists (select 1 from dbo.Policies where PolicyNumber = 'PC-10001')
insert dbo.Policies(Id, PolicyNumber, HolderName, EffectiveDate, ExpirationDate, Deductible, CoverageLimit)
values (newid(),'PC-10001','Jane Doe','2025-01-01','2025-12-31',500,25000);

insert dbo.Claims(Id, PolicyNumber, LossDate, Amount, Status, CreatedUtc)
select top (200)
  newid(), 'PC-10001',
  datefromparts(2025, 1 + abs(checksum(newid())) % 12, 1 + abs(checksum(newid())) % 28),
  cast(100 + abs(checksum(newid())) % 5000 as money),
  abs(checksum(newid())) % 3,      -- 0 submitted, 1 validated, 2 rejected
  dateadd(hour, -abs(checksum(newid())) % 720, sysutcdatetime())
from sys.objects s1, sys.objects s2;
