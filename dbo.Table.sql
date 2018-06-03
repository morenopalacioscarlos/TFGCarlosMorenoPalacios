CREATE TABLE [dbo].[Sold]
(
	[IdSold] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [Date] DATE NOT NULL, 
    [Time] TIME NOT NULL, 
    [IdMachine] INT NOT NULL, 
    [IdProduct] INT NOT NULL
	CONSTRAINT [Vending_fk0] FOREIGN KEY ([IdMachine]) REFERENCES [dbo].[Vending_Machine] ([Id_Machine])
)
