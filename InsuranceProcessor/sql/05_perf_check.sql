set statistics io on;
set statistics time on;

exec dbo.usp_Claims_Search @policyNumber = 'PC-10001', @skip = 0, @take = 20;
select top 10 * from dbo.v_ClaimDailyStats order by Day desc;
