USE [dbcafeteria]
GO

-- Optional production auth extension for refresh-token rotation.
-- Cafe.sql already contains the requested Clientes auth columns:
-- PasswordHash, FechaRegistro, Activo, EsInvitado.
IF OBJECT_ID('[dbo].[ClientesRefreshTokens]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ClientesRefreshTokens](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [IdCliente] [int] NOT NULL,
        [TokenHash] [varbinary](32) NOT NULL,
        [FechaExpiracion] [datetime] NOT NULL,
        [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_ClientesRefreshTokens_FechaCreacion] DEFAULT (getdate()),
        [FechaRevocacion] [datetime] NULL,
        CONSTRAINT [PK_dbo.ClientesRefreshTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_dbo.ClientesRefreshTokens_dbo.Clientes_IdCliente] FOREIGN KEY([IdCliente]) REFERENCES [dbo].[Clientes] ([Id])
    )
END
GO

-- Optional payment state used by Cash on Delivery orders until staff receives cash.
IF COL_LENGTH('dbo.PedidosPagos', 'Estado') IS NULL
BEGIN
    ALTER TABLE [dbo].[PedidosPagos]
    ADD [Estado] VARCHAR(50) NOT NULL
        CONSTRAINT [DF_PedidosPagos_Estado] DEFAULT ('Pending');
END
GO

-- Created Sucursales table for store branches used during order placement.
IF OBJECT_ID('[dbo].[Sucursales]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Sucursales]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] VARCHAR(255) NOT NULL,
        [Direccion] VARCHAR(500) NULL,
        [Telefono] VARCHAR(50) NULL,
        [Activo] BIT NOT NULL CONSTRAINT [DF_Sucursales_Activo] DEFAULT (1),
        [FechaCreacion] DATETIME NOT NULL CONSTRAINT [DF_Sucursales_FechaCreacion] DEFAULT (GETDATE()),
        CONSTRAINT [PK_Sucursales] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Added IdSucursal to Pedidos so only placed orders reference the selected branch.
IF COL_LENGTH('dbo.Pedidos', 'IdSucursal') IS NULL
BEGIN
    ALTER TABLE [dbo].[Pedidos]
    ADD [IdSucursal] INT NULL;
END
GO

-- Added FK from Pedidos to Sucursales.
IF OBJECT_ID('[dbo].[FK_Pedidos_Sucursales]', 'F') IS NULL
BEGIN
    ALTER TABLE [dbo].[Pedidos]
    ADD CONSTRAINT [FK_Pedidos_Sucursales]
    FOREIGN KEY ([IdSucursal]) REFERENCES [dbo].[Sucursales]([Id]);
END
GO

-- Seeded initial branch records.
IF OBJECT_ID('[dbo].[Sucursales]', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Sucursales] WHERE [Nombre] = 'Bangalore')
    BEGIN
        INSERT INTO [dbo].[Sucursales] ([Nombre], [Direccion], [Activo])
        VALUES ('Bangalore', NULL, 1);
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Sucursales] WHERE [Nombre] = 'Pune')
    BEGIN
        INSERT INTO [dbo].[Sucursales] ([Nombre], [Direccion], [Activo])
        VALUES ('Pune', NULL, 1);
    END
END
GO
