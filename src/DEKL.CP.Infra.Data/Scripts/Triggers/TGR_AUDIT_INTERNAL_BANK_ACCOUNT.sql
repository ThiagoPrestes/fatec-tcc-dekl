USE DEKLCP
GO 

ALTER TRIGGER TGR_AUDIT_INTERNAL_BANK_ACCOUNT
ON InternalBankAccount
FOR UPDATE
AS
BEGIN
	DECLARE @ApplicationUserId INT;
	DECLARE @ModuleId INT;
    DECLARE @Event VARCHAR(MAX);
	
	SET @Event = '';

	IF UPDATE(Number)
	BEGIN
		SELECT @Event = @Event + '<b>N�mero da Conta de:</b> ' + D.Number + 
		                                      ' <b>Para:</b> ' + I.Number + '\n' 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.Number != I.Number
	END

	IF UPDATE(Name)
	BEGIN
		SELECT @Event = @Event + '<b>Nome de:</b> ' + ISNULL(D.Name, '') + 
		                           ' <b>Para:</b> ' + ISNULL(I.Name, '') + '\n' 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			ISNULL(D.Name, '') != ISNULL(I.Name, '')
	END

	IF UPDATE(Balance)
	BEGIN
		SELECT @Event = @Event + '<b>Saldo de:</b> ' + FORMAT(D.Balance, 'C2', 'pt-br') + 
		                            ' <b>Para:</b> ' + FORMAT(D.Balance, 'C2', 'pt-br') + '\n' 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.Balance != I.Balance
	END

	IF UPDATE(BankAgencyId)
	BEGIN
		SELECT @Event = @Event + '<b>Ag�ncia Banc�ria de:</b> ' + (
																SELECT FORMAT(BA.Number, 'D') + ' - ' + B.Name 
																FROM BankAgency BA
																JOIN Bank B ON BA.BankId = B.Id
																WHERE BA.Id = D.BankAgencyId
														    ) + 
		                                        ' <b>Para:</b> ' + (
																SELECT FORMAT(BA.Number, 'D') + ' - ' + B.Name 
																FROM BankAgency BA
																JOIN Bank B ON BA.BankId = B.Id
																WHERE BA.Id = I.BankAgencyId
														    ) 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.BankAgencyId != I.BankAgencyId
	END

	IF(@Event IS NOT NULL AND @Event != '')
	BEGIN
		SELECT @ApplicationUserId = ApplicationUserId, @ModuleId = ModuleId FROM INSERTED

		INSERT INTO Audit (ApplicationUserId, ModuleId, Event, DateTime, AddedDate, ModifiedDate, Active)
		VALUES (@ApplicationUserId, @ModuleId, @Event, GETDATE(), GETDATE(), NULL, 1)
	END

END