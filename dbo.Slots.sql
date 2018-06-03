CREATE TABLE [dbo].[Slots] (
    [Id_Slot]     INT   IDENTITY (1, 1) NOT NULL,
    [Id_Machine]  INT   NOT NULL,
    [Id_Product]  INT   NOT NULL,
    [Stock]       INT   NULL,
    [Price]       DECIMAL(2, 4) NULL,
    [Slot_Number] INT   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id_Slot] ASC, [Id_Machine] ASC, [Id_Product] ASC),
    CONSTRAINT [Slots_fk0] FOREIGN KEY ([Id_Machine]) REFERENCES [dbo].[Vending_Machine] ([Id_Machine]),
    CONSTRAINT [Slots_fk1] FOREIGN KEY ([Id_Product]) REFERENCES [dbo].[Items] ([Id])
);

