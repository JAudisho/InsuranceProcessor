create or alter proc dbo.usp_Claims_Search
  @policyNumber varchar(32) = null,
  @skip int = 0,
  @take int = 20
as
begin
  set nocount on;
  select Id, PolicyNumber, Amount, Status, CreatedUtc
  from dbo.Claims
  where (@policyNumber is null or PolicyNumber = @policyNumber)
  order by CreatedUtc desc
  offset @skip rows fetch next @take rows only;
end
