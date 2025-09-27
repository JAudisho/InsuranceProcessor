create index IX_Claims_StatusCreated
  on dbo.Claims(Status, CreatedUtc)
  include (PolicyNumber, Amount);

create index IX_Claims_PolicyCreated
  on dbo.Claims(PolicyNumber, CreatedUtc)
  include (Amount, Status);

create index IX_ClaimEvents_ClaimCreated
  on dbo.ClaimEvents(ClaimId, CreatedUtc);
