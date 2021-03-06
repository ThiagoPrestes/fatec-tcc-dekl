﻿
CREATE TRIGGER [dbo].[TGR_AUDIT_ACCOUNT_TO_PAY]
ON [dbo].[AccountToPay]
FOR UPDATE
AS
BEGIN
    DECLARE @UpdatedRecordId INT;
	DECLARE @ApplicationUserId INT;
	DECLARE @ModuleId INT;
    DECLARE @Event VARCHAR(MAX);
	
	SET @Event = '';

	IF UPDATE(Value)
	BEGIN
		SELECT @Event = @Event + '<b>Valor de:</b> ' + FORMAT(D.Value, 'C2', 'pt-br') + 
		                            '  <b>Para:</b> ' + FORMAT(I.Value, 'C2', 'pt-br') + '|' 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.Value != I.Value
	END

	IF UPDATE(PaidValue)
	BEGIN
		SELECT @Event = @Event + '<b>Valor Pago de:</b> ' + FORMAT(ISNULL(D.PaidValue, 0), 'C2', 'pt-br') + 
		                                 '  <b>Para:</b> ' + FORMAT(ISNULL(I.PaidValue, 0), 'C2', 'pt-br') + '|' 
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			ISNULL(D.PaidValue, 0) != ISNULL(I.PaidValue, 0)
	END

	IF UPDATE(PaymentDate)
	BEGIN
		SELECT @Event = @Event + '<b>Data de pagamento de:</b> ' + IIF(D.PaymentDate IS NULL, '', FORMAT(D.PaymentDate, 'dd/MM/yyyy', 'pt-br')) + 
		                                        '  <b>Para:</b> ' + IIF(I.PaymentType IS NULL, '', FORMAT(I.PaymentDate, 'dd/MM/yyyy', 'pt-br')) + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			ISNULL(D.PaymentDate, GETDATE()) != ISNULL(I.PaymentDate, GETDATE())
	END

	IF UPDATE(Description)
	BEGIN
		SELECT @Event = @Event + '<b>Descrição de:</b> ' + ISNULL(D.Description, '') + 
		                                '  <b>Para:</b> ' + ISNULL(I.Description, '') + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			ISNULL(D.Description, '') != ISNULL(I.Description, '')
	END

	IF UPDATE(MaturityDate)
	BEGIN
		SELECT @Event = @Event + '<b>Data de vencimento de:</b> ' + FORMAT(D.MaturityDate, 'dd/MM/yyyy', 'pt-br') + 
		                                         '  <b>Para:</b> ' + FORMAT(I.MaturityDate, 'dd/MM/yyyy', 'pt-br') + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.MaturityDate != I.MaturityDate
	END

	IF UPDATE(DailyInterest)
	BEGIN
		SELECT @Event = @Event + '<b>Mora diária de</b>: ' + CAST(D.DailyInterest AS VARCHAR(10)) + '%' +  
		                                  '  <b>Para:</b> ' + CAST(I.DailyInterest AS VARCHAR(10)) + '%|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.DailyInterest != I.DailyInterest
	END

	IF UPDATE(Penalty)
	BEGIN
		SELECT @Event = @Event + '<b>Multa de:</b> ' + CAST(D.Penalty AS VARCHAR(10)) + '%' + 
		                            '  <b>Para:</b> ' + CAST(I.Penalty AS VARCHAR(10)) + '%|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE
			D.Penalty != I.Penalty
	END

	IF UPDATE(MonthlyAccount)
	BEGIN
		SELECT @Event = @Event + '<b>Conta mensal de:</b> ' + IIF(D.MonthlyAccount = 1, 'Sim', 'Não') +
		                                   '  <b>Para:</b> ' + IIF(I.MonthlyAccount = 1, 'Sim', 'Não') + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.MonthlyAccount != I.MonthlyAccount
	END

	IF UPDATE(Priority)
	BEGIN
		SELECT @Event = @Event + '<b>Prioridade de:</b> ' + dbo.FN_PRIORITY_ACCOUNT_TO_PAY(D.Priority) +
		                                 '  <b>Para:</b> ' + dbo.FN_PRIORITY_ACCOUNT_TO_PAY(I.Priority) + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.Priority != I.Priority
	END

	IF UPDATE(PaymentType)
	BEGIN
		SELECT @Event = @Event + '<b>Tipo de Pagamento:</b> ' + dbo.FN_PAYMENT_TYPE_ACCOUNT_TO_PAY(D.PaymentType) +
		                                 '  <b>Para:</b> ' + dbo.FN_PAYMENT_TYPE_ACCOUNT_TO_PAY(I.PaymentType) + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.PaymentType != I.PaymentType
	END

	IF UPDATE(DocumentNumber)
	BEGIN
		SELECT @Event = @Event + '<b>Número do Documento de:</b> ' + D.DocumentNumber + 
		                                          '  <b>Para:</b> ' + I.DocumentNumber + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.DocumentNumber != I.DocumentNumber
	END

	IF UPDATE(NumberOfInstallments)
	BEGIN
		SELECT @Event = @Event + '<b>Número de parcelas de:</b> ' + FORMAT(D.NumberOfInstallments, 'D') + 
		                                         '  <b>Para:</b> ' + FORMAT(I.NumberOfInstallments, 'D') + '|'
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.NumberOfInstallments != I.NumberOfInstallments
	END

	IF UPDATE(ProviderId)
	BEGIN
		SELECT @Event = @Event + '<b>Fornecedor de:</b> ' + (
														SELECT ISNULL(PPP.Name, PLP.CorporateName) FROM Provider P
														LEFT JOIN ProviderPhysicalPerson PPP ON P.Id = PPP.Id
														LEFT JOIN ProviderLegalPerson PLP ON P.Id = PLP.Id
														WHERE 
															P.Id = D.ProviderId
													  ) +
		                                  '  <b>Para:</b> ' + (
														SELECT ISNULL(PPP.Name, PLP.CorporateName) FROM Provider P
														LEFT JOIN ProviderPhysicalPerson PPP ON P.Id = PPP.Id
														LEFT JOIN ProviderLegalPerson PLP ON P.Id = PLP.Id
														WHERE 
															P.Id = I.ProviderId
													  )
		FROM DELETED D
		JOIN INSERTED I ON D.Id = I.Id
		WHERE 
			D.ProviderId != I.ProviderId
	END

	IF(@Event IS NOT NULL AND @Event != '')
	BEGIN
		SELECT @UpdatedRecordId =  Id, @ApplicationUserId = ApplicationUserId, @ModuleId = ModuleId FROM INSERTED

		INSERT INTO Audit (UpdatedRecordId, ApplicationUserId, ModuleId, Event, DateTime, AddedDate, ModifiedDate, Active)
		VALUES (@UpdatedRecordId, @ApplicationUserId, @ModuleId, @Event, GETDATE(), GETDATE(), NULL, 1)
	END
END