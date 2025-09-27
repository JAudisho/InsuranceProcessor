create or alter view dbo.v_ClaimDailyStats as
select
  convert(date, CreatedUtc) as Day,
  sum(case when Status = 1 then 1 else 0 end) as Validated,
  sum(case when Status = 2 then 1 else 0 end) as Rejected,
  count(*) as Total
from dbo.Claims
group by convert(date, CreatedUtc);

create or alter view dbo.v_ClaimsByPolicy as
select PolicyNumber, count(*) as ClaimCount,
       sum(cast(Amount as decimal(18,2))) as TotalAmount
from dbo.Claims
group by PolicyNumber;
