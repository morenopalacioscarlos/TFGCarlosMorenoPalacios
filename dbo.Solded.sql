CREATE TABLE [dbo].[Solded] (
    [IdSold]  INT      IDENTITY (1, 1) NOT NULL,
    [Date]      DATE     NOT NULL,
    [Time]      TIME (7) NOT NULL,
    [IdMachine] INT      NOT NULL,
    [IdProduct] INT      NOT NULL,
    PRIMARY KEY CLUSTERED ([IdSold] ASC),
    CONSTRAINT [Vending_fk0] FOREIGN KEY ([IdMachine]) REFERENCES [dbo].[Vending_Machine] ([Id_Machine])
);

