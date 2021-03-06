USE [DEKLCP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[FN_PRIORITY_ACCOUNT_TO_PAY](@Priority INT)
RETURNS VARCHAR(20)
AS
BEGIN
	RETURN CASE @Priority 
		   WHEN 0 THEN 'Alta'
	       WHEN 1 THEN 'Média'
		   WHEN 2 THEN 'Baixa'
	END
END