USE [master]
GO
/****** Object:  Database [dbcafeteria]    Script Date: 01/05/2026 4:26:42 am ******/
CREATE DATABASE [dbcafeteria]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'dbcafeteria', FILENAME = N'C:\Users\usama.mukhyer\dbcafeteria.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'dbcafeteria_log', FILENAME = N'C:\Users\usama.mukhyer\dbcafeteria_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [dbcafeteria] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dbcafeteria].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [dbcafeteria] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [dbcafeteria] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [dbcafeteria] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [dbcafeteria] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [dbcafeteria] SET ARITHABORT OFF 
GO
ALTER DATABASE [dbcafeteria] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [dbcafeteria] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [dbcafeteria] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [dbcafeteria] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [dbcafeteria] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [dbcafeteria] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [dbcafeteria] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [dbcafeteria] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [dbcafeteria] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [dbcafeteria] SET  DISABLE_BROKER 
GO
ALTER DATABASE [dbcafeteria] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [dbcafeteria] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [dbcafeteria] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [dbcafeteria] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [dbcafeteria] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [dbcafeteria] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [dbcafeteria] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [dbcafeteria] SET RECOVERY FULL 
GO
ALTER DATABASE [dbcafeteria] SET  MULTI_USER 
GO
ALTER DATABASE [dbcafeteria] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [dbcafeteria] SET DB_CHAINING OFF 
GO
ALTER DATABASE [dbcafeteria] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [dbcafeteria] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [dbcafeteria] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [dbcafeteria] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [dbcafeteria] SET QUERY_STORE = OFF
GO
USE [dbcafeteria]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clasificaciones]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clasificaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](255) NOT NULL,
	[Imagen] [varbinary](max) NULL,
	[Clave] [varchar](255) NOT NULL,
	[ClaveNumerica] [int] NULL,
	[Precio] [float] NULL,
	[TasaIVA] [float] NULL,
	[PrecioConIVA] [float] NULL,
 CONSTRAINT [PK_dbo.Clasificaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Telefono] [bigint] NULL,
	[Nombre] [varchar](255) NULL,
	[EsInvitado] [bit] NOT NULL,
	[Email] [varchar](255) NULL,
	[PasswordHash] [varbinary](max) NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[Activo] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Clientes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientesRefreshTokens]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientesRefreshTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NOT NULL,
	[TokenHash] [varbinary](32) NOT NULL,
	[FechaExpiracion] [datetime] NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
	[FechaRevocacion] [datetime] NULL,
 CONSTRAINT [PK_dbo.ClientesRefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Entradas]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Entradas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Tipo] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Entradas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntradasLeches]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntradasLeches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdEntrada] [int] NOT NULL,
	[IdLeche] [int] NOT NULL,
	[CantidadLitros] [float] NOT NULL,
 CONSTRAINT [PK_dbo.EntradasLeches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntradasTiposGranos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntradasTiposGranos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdEntrada] [int] NOT NULL,
	[IdTipoGrano] [int] NOT NULL,
	[CantidadKilos] [float] NOT NULL,
 CONSTRAINT [PK_dbo.EntradasTiposGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntradasToppings]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntradasToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdEntrada] [int] NOT NULL,
	[IdTopping] [int] NOT NULL,
	[Cantidad] [float] NOT NULL,
 CONSTRAINT [PK_dbo.EntradasToppings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntradasVasos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntradasVasos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdEntrada] [int] NOT NULL,
	[IdVaso] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
 CONSTRAINT [PK_dbo.EntradasVasos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Leches]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Leches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [varchar](20) NULL,
	[Descripcion] [varchar](250) NULL,
	[ClaveNumerica] [int] NULL,
	[Imagen] [varbinary](max) NULL,
	[Precio] [float] NOT NULL,
	[TasaIVA] [float] NOT NULL,
	[PrecioConIVA] [float] NOT NULL,
	[ExistenciaLitros] [float] NOT NULL,
 CONSTRAINT [PK_dbo.Leches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pedidos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pedidos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Subtotal] [float] NOT NULL,
	[IVA] [float] NOT NULL,
	[Total] [float] NOT NULL,
	[IdCliente] [int] NULL,
	[Estado] [int] NOT NULL,
	[NoPedido] [int] NOT NULL,
	[Cambio] [float] NOT NULL,
	[IdUsuarioPreparo] [int] NULL,
	[IdUsuarioEntrego] [int] NULL,
	[TipoPedido] [varchar](50) NOT NULL,
	[TipoPickup] [varchar](50) NOT NULL,
	[FechaPickup] [datetime] NULL,
	[Ubicacion] [varchar](255) NULL,
	[IdSucursal] [int] NULL,
 CONSTRAINT [PK_dbo.Pedidos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PedidosPagos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidosPagos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPedido] [int] NOT NULL,
	[FormaPago] [varchar](255) NOT NULL,
	[Moneda] [varchar](255) NOT NULL,
	[TipoCambio] [float] NOT NULL,
	[Pago] [float] NOT NULL,
	[PagoMXN] [float] NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Estado] [varchar](50) NOT NULL,
 CONSTRAINT [PK_dbo.PedidosPagos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PedidosProductos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidosProductos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPedido] [int] NOT NULL,
	[IdClasificacion] [int] NOT NULL,
	[TipoBebida] [int] NULL,
	[IdVaso] [int] NULL,
	[IdLeche] [int] NULL,
	[Subtotal] [float] NOT NULL,
	[TasaIVA] [float] NOT NULL,
	[IVA] [float] NOT NULL,
	[Total] [float] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[IdSubClasificacion] [int] NOT NULL,
	[Precio] [float] NOT NULL,
	[PrecioLeche] [float] NOT NULL,
	[TasaIVALeche] [float] NOT NULL,
	[PrecioConIVALeche] [float] NOT NULL,
	[IdTipoGrano] [int] NULL,
	[PrecioTipoGrano] [float] NOT NULL,
	[PrecioConIVATipoGrano] [float] NOT NULL,
	[TasaIVATipoGrano] [float] NOT NULL,
	[CantidadLeche] [float] NOT NULL,
	[CantidadCafe] [float] NOT NULL,
 CONSTRAINT [PK_dbo.PedidosProductos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PedidosProductosToppings]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidosProductosToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPedidoProducto] [int] NOT NULL,
	[IdTopping] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Subtotal] [float] NOT NULL,
	[TasaIVA] [float] NOT NULL,
	[IVA] [float] NOT NULL,
	[Total] [float] NOT NULL,
	[Precio] [float] NOT NULL,
 CONSTRAINT [PK_dbo.PedidosProductosToppings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Regalos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regalos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPedido] [int] NOT NULL,
	[IdClienteRemitente] [int] NULL,
	[IdClienteReceptor] [int] NULL,
	[NombreReceptor] [varchar](255) NOT NULL,
	[TelefonoReceptor] [varchar](50) NOT NULL,
	[Mensaje] [varchar](500) NULL,
	[CodigoRegalo] [varchar](100) NULL,
	[Estado] [varchar](50) NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
	[FechaEnvio] [datetime] NULL,
	[FechaRedimido] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Salidas]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salidas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Tipo] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Salidas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalidasLeches]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalidasLeches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSalida] [int] NOT NULL,
	[IdLeche] [int] NOT NULL,
	[CantidadLitros] [float] NOT NULL,
 CONSTRAINT [PK_dbo.SalidasLeches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalidasTiposGranos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalidasTiposGranos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSalida] [int] NOT NULL,
	[IdTipoGrano] [int] NOT NULL,
	[CantidadKilos] [float] NOT NULL,
 CONSTRAINT [PK_dbo.SalidasTiposGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalidasToppings]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalidasToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSalida] [int] NOT NULL,
	[IdTopping] [int] NOT NULL,
	[Cantidad] [float] NOT NULL,
 CONSTRAINT [PK_dbo.SalidasToppings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalidasVasos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalidasVasos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSalida] [int] NOT NULL,
	[IdVaso] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SalidasVasos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubClasificaciones]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubClasificaciones](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [varchar](255) NULL,
	[ClaveNumerica] [int] NULL,
	[Descripcion] [varchar](255) NOT NULL,
	[Imagen] [varbinary](max) NULL,
	[BebidasFrias] [bit] NOT NULL,
	[BebidasCalientes] [bit] NOT NULL,
	[Precio] [float] NULL,
	[TasaIVA] [float] NOT NULL,
	[PrecioConIVA] [float] NULL,
	[IdClasificacion] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SubClasificaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubClasificacionesLeches]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubClasificacionesLeches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSubClasificacion] [int] NOT NULL,
	[IdLeche] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SubClasificacionesLeches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubClasificacionesTiposGranos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubClasificacionesTiposGranos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSubClasificacion] [int] NOT NULL,
	[IdTipoGrano] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SubClasificacionesTiposGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubClasificacionesToppings]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubClasificacionesToppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSubClasificacion] [int] NOT NULL,
	[IdTopping] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SubClasificacionesToppings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubClasificacionesVasos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubClasificacionesVasos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdSubClasificacion] [int] NOT NULL,
	[IdVaso] [int] NOT NULL,
	[Precio] [float] NULL,
	[PrecioConIVA] [float] NULL,
	[CantidadLeche] [float] NOT NULL,
	[GramosCafe] [float] NOT NULL,
 CONSTRAINT [PK_dbo.SubClasificacionesVasos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sucursales]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sucursales](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](255) NOT NULL,
	[Direccion] [varchar](500) NULL,
	[Telefono] [varchar](50) NULL,
	[Activo] [bit] NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
 CONSTRAINT [PK_Sucursales] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TiposCambio]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposCambio](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [date] NOT NULL,
	[TipoCambio] [float] NOT NULL,
 CONSTRAINT [PK_dbo.TiposCambio] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TiposGranos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TiposGranos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [varchar](20) NULL,
	[Descripcion] [varchar](150) NULL,
	[Imagen] [varbinary](max) NULL,
	[Precio] [float] NOT NULL,
	[TasaIva] [float] NOT NULL,
	[PrecioConIva] [float] NOT NULL,
	[ClaveNumerica] [int] NULL,
	[Existenciakilos] [float] NOT NULL,
 CONSTRAINT [PK_dbo.TiposGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Toppings]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Toppings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [varchar](255) NULL,
	[ClaveNumerica] [int] NULL,
	[Descripcion] [varchar](255) NOT NULL,
	[Imagen] [varbinary](max) NULL,
	[CantidadCortesia] [int] NOT NULL,
	[Precio] [float] NOT NULL,
	[TasaIVA] [float] NOT NULL,
	[PrecioConIVA] [float] NOT NULL,
	[Existencia] [float] NOT NULL,
	[ShotExpresso] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Toppings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Usuario] [varchar](255) NOT NULL,
	[Password] [varbinary](max) NOT NULL,
	[Nombre] [varchar](255) NOT NULL,
	[Inactivo] [bit] NOT NULL,
	[FechaAlta] [datetime] NULL,
	[Tipo] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Usuarios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vasos]    Script Date: 01/05/2026 4:26:43 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vasos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clave] [varchar](20) NULL,
	[Descripcion] [varchar](250) NULL,
	[ClaveNumerica] [int] NULL,
	[Imagen] [varbinary](max) NULL,
	[BebidasFrias] [bit] NOT NULL,
	[BebidasCalientes] [bit] NOT NULL,
	[Existencia] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Vasos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_IdUsuario]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdUsuario] ON [dbo].[Entradas]
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdEntrada]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdEntrada] ON [dbo].[EntradasLeches]
(
	[IdEntrada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdLeche]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdLeche] ON [dbo].[EntradasLeches]
(
	[IdLeche] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdEntrada]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdEntrada] ON [dbo].[EntradasTiposGranos]
(
	[IdEntrada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTipoGrano]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTipoGrano] ON [dbo].[EntradasTiposGranos]
(
	[IdTipoGrano] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdEntrada]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdEntrada] ON [dbo].[EntradasToppings]
(
	[IdEntrada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTopping]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTopping] ON [dbo].[EntradasToppings]
(
	[IdTopping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdEntrada]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdEntrada] ON [dbo].[EntradasVasos]
(
	[IdEntrada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdVaso]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdVaso] ON [dbo].[EntradasVasos]
(
	[IdVaso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdCliente]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdCliente] ON [dbo].[Pedidos]
(
	[IdCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdUsuarioEntrego]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdUsuarioEntrego] ON [dbo].[Pedidos]
(
	[IdUsuarioEntrego] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdUsuarioPreparo]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdUsuarioPreparo] ON [dbo].[Pedidos]
(
	[IdUsuarioPreparo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdPedido]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdPedido] ON [dbo].[PedidosPagos]
(
	[IdPedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdUsuario]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdUsuario] ON [dbo].[PedidosPagos]
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdClasificacion]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdClasificacion] ON [dbo].[PedidosProductos]
(
	[IdClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdLeche]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdLeche] ON [dbo].[PedidosProductos]
(
	[IdLeche] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdPedido]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdPedido] ON [dbo].[PedidosProductos]
(
	[IdPedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSubClasificacion]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSubClasificacion] ON [dbo].[PedidosProductos]
(
	[IdSubClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTipoGrano]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTipoGrano] ON [dbo].[PedidosProductos]
(
	[IdTipoGrano] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdVaso]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdVaso] ON [dbo].[PedidosProductos]
(
	[IdVaso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdPedidoProducto]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdPedidoProducto] ON [dbo].[PedidosProductosToppings]
(
	[IdPedidoProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTopping]    Script Date: 01/05/2026 4:26:43 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTopping] ON [dbo].[PedidosProductosToppings]
(
	[IdTopping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdUsuario]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdUsuario] ON [dbo].[Salidas]
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdLeche]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdLeche] ON [dbo].[SalidasLeches]
(
	[IdLeche] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSalida]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSalida] ON [dbo].[SalidasLeches]
(
	[IdSalida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSalida]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSalida] ON [dbo].[SalidasTiposGranos]
(
	[IdSalida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTipoGrano]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTipoGrano] ON [dbo].[SalidasTiposGranos]
(
	[IdTipoGrano] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSalida]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSalida] ON [dbo].[SalidasToppings]
(
	[IdSalida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTopping]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTopping] ON [dbo].[SalidasToppings]
(
	[IdTopping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSalida]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSalida] ON [dbo].[SalidasVasos]
(
	[IdSalida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdVaso]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdVaso] ON [dbo].[SalidasVasos]
(
	[IdVaso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdClasificacion]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdClasificacion] ON [dbo].[SubClasificaciones]
(
	[IdClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdLeche]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdLeche] ON [dbo].[SubClasificacionesLeches]
(
	[IdLeche] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSubClasificacion]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSubClasificacion] ON [dbo].[SubClasificacionesLeches]
(
	[IdSubClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSubClasificacion]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSubClasificacion] ON [dbo].[SubClasificacionesTiposGranos]
(
	[IdSubClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTipoGrano]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTipoGrano] ON [dbo].[SubClasificacionesTiposGranos]
(
	[IdTipoGrano] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSubClasificacion]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSubClasificacion] ON [dbo].[SubClasificacionesToppings]
(
	[IdSubClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdTopping]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdTopping] ON [dbo].[SubClasificacionesToppings]
(
	[IdTopping] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdSubClasificacion]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdSubClasificacion] ON [dbo].[SubClasificacionesVasos]
(
	[IdSubClasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdVaso]    Script Date: 01/05/2026 4:26:44 am ******/
CREATE NONCLUSTERED INDEX [IX_IdVaso] ON [dbo].[SubClasificacionesVasos]
(
	[IdVaso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Clientes] ADD  DEFAULT ((0)) FOR [EsInvitado]
GO
ALTER TABLE [dbo].[Clientes] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO
ALTER TABLE [dbo].[Clientes] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[ClientesRefreshTokens] ADD  CONSTRAINT [DF_ClientesRefreshTokens_FechaCreacion]  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[Entradas] ADD  DEFAULT ((0)) FOR [Tipo]
GO
ALTER TABLE [dbo].[Leches] ADD  DEFAULT ((0)) FOR [Precio]
GO
ALTER TABLE [dbo].[Leches] ADD  DEFAULT ((0)) FOR [TasaIVA]
GO
ALTER TABLE [dbo].[Leches] ADD  DEFAULT ((0)) FOR [PrecioConIVA]
GO
ALTER TABLE [dbo].[Leches] ADD  DEFAULT ((0)) FOR [ExistenciaLitros]
GO
ALTER TABLE [dbo].[Pedidos] ADD  DEFAULT ('ForMe') FOR [TipoPedido]
GO
ALTER TABLE [dbo].[Pedidos] ADD  DEFAULT ('Now') FOR [TipoPickup]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [PrecioLeche]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [TasaIVALeche]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [PrecioConIVALeche]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [PrecioTipoGrano]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [PrecioConIVATipoGrano]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [TasaIVATipoGrano]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [CantidadLeche]
GO
ALTER TABLE [dbo].[PedidosProductos] ADD  DEFAULT ((0)) FOR [CantidadCafe]
GO
ALTER TABLE [dbo].[PedidosPagos] ADD  DEFAULT ('Pending') FOR [Estado]
GO
ALTER TABLE [dbo].[Regalos] ADD  DEFAULT ('Pending') FOR [Estado]
GO
ALTER TABLE [dbo].[Regalos] ADD  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[SubClasificacionesVasos] ADD  DEFAULT ((0)) FOR [CantidadLeche]
GO
ALTER TABLE [dbo].[SubClasificacionesVasos] ADD  DEFAULT ((0)) FOR [GramosCafe]
GO
ALTER TABLE [dbo].[Sucursales] ADD  CONSTRAINT [DF_Sucursales_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Sucursales] ADD  CONSTRAINT [DF_Sucursales_FechaCreacion]  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[TiposGranos] ADD  DEFAULT ((0)) FOR [Existenciakilos]
GO
ALTER TABLE [dbo].[Toppings] ADD  DEFAULT ((0)) FOR [Existencia]
GO
ALTER TABLE [dbo].[Toppings] ADD  DEFAULT ((0)) FOR [ShotExpresso]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((0)) FOR [Tipo]
GO
ALTER TABLE [dbo].[Vasos] ADD  DEFAULT ((0)) FOR [Existencia]
GO
ALTER TABLE [dbo].[ClientesRefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ClientesRefreshTokens_dbo.Clientes_IdCliente] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[ClientesRefreshTokens] CHECK CONSTRAINT [FK_dbo.ClientesRefreshTokens_dbo.Clientes_IdCliente]
GO
ALTER TABLE [dbo].[Entradas]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Entradas_dbo.Usuarios_IdUsuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Entradas] CHECK CONSTRAINT [FK_dbo.Entradas_dbo.Usuarios_IdUsuario]
GO
ALTER TABLE [dbo].[EntradasLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasLeches_dbo.Entradas_IdEntrada] FOREIGN KEY([IdEntrada])
REFERENCES [dbo].[Entradas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntradasLeches] CHECK CONSTRAINT [FK_dbo.EntradasLeches_dbo.Entradas_IdEntrada]
GO
ALTER TABLE [dbo].[EntradasLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasLeches_dbo.Leches_IdLeche] FOREIGN KEY([IdLeche])
REFERENCES [dbo].[Leches] ([Id])
GO
ALTER TABLE [dbo].[EntradasLeches] CHECK CONSTRAINT [FK_dbo.EntradasLeches_dbo.Leches_IdLeche]
GO
ALTER TABLE [dbo].[EntradasTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasTiposGranos_dbo.Entradas_IdEntrada] FOREIGN KEY([IdEntrada])
REFERENCES [dbo].[Entradas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntradasTiposGranos] CHECK CONSTRAINT [FK_dbo.EntradasTiposGranos_dbo.Entradas_IdEntrada]
GO
ALTER TABLE [dbo].[EntradasTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasTiposGranos_dbo.TiposGranos_IdTipoGrano] FOREIGN KEY([IdTipoGrano])
REFERENCES [dbo].[TiposGranos] ([Id])
GO
ALTER TABLE [dbo].[EntradasTiposGranos] CHECK CONSTRAINT [FK_dbo.EntradasTiposGranos_dbo.TiposGranos_IdTipoGrano]
GO
ALTER TABLE [dbo].[EntradasToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasToppings_dbo.Entradas_IdEntrada] FOREIGN KEY([IdEntrada])
REFERENCES [dbo].[Entradas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntradasToppings] CHECK CONSTRAINT [FK_dbo.EntradasToppings_dbo.Entradas_IdEntrada]
GO
ALTER TABLE [dbo].[EntradasToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasToppings_dbo.Toppings_IdTopping] FOREIGN KEY([IdTopping])
REFERENCES [dbo].[Toppings] ([Id])
GO
ALTER TABLE [dbo].[EntradasToppings] CHECK CONSTRAINT [FK_dbo.EntradasToppings_dbo.Toppings_IdTopping]
GO
ALTER TABLE [dbo].[EntradasVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasVasos_dbo.Entradas_IdEntrada] FOREIGN KEY([IdEntrada])
REFERENCES [dbo].[Entradas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntradasVasos] CHECK CONSTRAINT [FK_dbo.EntradasVasos_dbo.Entradas_IdEntrada]
GO
ALTER TABLE [dbo].[EntradasVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EntradasVasos_dbo.Vasos_IdVaso] FOREIGN KEY([IdVaso])
REFERENCES [dbo].[Vasos] ([Id])
GO
ALTER TABLE [dbo].[EntradasVasos] CHECK CONSTRAINT [FK_dbo.EntradasVasos_dbo.Vasos_IdVaso]
GO
ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pedidos_dbo.Clientes_IdCliente] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Pedidos] CHECK CONSTRAINT [FK_dbo.Pedidos_dbo.Clientes_IdCliente]
GO
ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pedidos_dbo.Usuarios_IdUsuarioEntrego] FOREIGN KEY([IdUsuarioEntrego])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Pedidos] CHECK CONSTRAINT [FK_dbo.Pedidos_dbo.Usuarios_IdUsuarioEntrego]
GO
ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pedidos_dbo.Usuarios_IdUsuarioPreparo] FOREIGN KEY([IdUsuarioPreparo])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Pedidos] CHECK CONSTRAINT [FK_dbo.Pedidos_dbo.Usuarios_IdUsuarioPreparo]
GO
ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_Sucursales] FOREIGN KEY([IdSucursal])
REFERENCES [dbo].[Sucursales] ([Id])
GO
ALTER TABLE [dbo].[Pedidos] CHECK CONSTRAINT [FK_Pedidos_Sucursales]
GO
ALTER TABLE [dbo].[PedidosPagos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosPagos_dbo.Pedidos_IdPedido] FOREIGN KEY([IdPedido])
REFERENCES [dbo].[Pedidos] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PedidosPagos] CHECK CONSTRAINT [FK_dbo.PedidosPagos_dbo.Pedidos_IdPedido]
GO
ALTER TABLE [dbo].[PedidosPagos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosPagos_dbo.Usuarios_IdUsuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[PedidosPagos] CHECK CONSTRAINT [FK_dbo.PedidosPagos_dbo.Usuarios_IdUsuario]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.Clasificaciones_IdClasificacion] FOREIGN KEY([IdClasificacion])
REFERENCES [dbo].[Clasificaciones] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.Clasificaciones_IdClasificacion]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.Leches_IdLeche] FOREIGN KEY([IdLeche])
REFERENCES [dbo].[Leches] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.Leches_IdLeche]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.Pedidos_IdPedido] FOREIGN KEY([IdPedido])
REFERENCES [dbo].[Pedidos] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.Pedidos_IdPedido]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.SubClasificaciones_IdSubClasificacion] FOREIGN KEY([IdSubClasificacion])
REFERENCES [dbo].[SubClasificaciones] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.SubClasificaciones_IdSubClasificacion]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.TiposGranos_IdTipoGrano] FOREIGN KEY([IdTipoGrano])
REFERENCES [dbo].[TiposGranos] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.TiposGranos_IdTipoGrano]
GO
ALTER TABLE [dbo].[PedidosProductos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductos_dbo.Vasos_IdVaso] FOREIGN KEY([IdVaso])
REFERENCES [dbo].[Vasos] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductos] CHECK CONSTRAINT [FK_dbo.PedidosProductos_dbo.Vasos_IdVaso]
GO
ALTER TABLE [dbo].[PedidosProductosToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductosToppings_dbo.PedidosProductos_IdPedidoProducto] FOREIGN KEY([IdPedidoProducto])
REFERENCES [dbo].[PedidosProductos] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PedidosProductosToppings] CHECK CONSTRAINT [FK_dbo.PedidosProductosToppings_dbo.PedidosProductos_IdPedidoProducto]
GO
ALTER TABLE [dbo].[PedidosProductosToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PedidosProductosToppings_dbo.Toppings_IdTopping] FOREIGN KEY([IdTopping])
REFERENCES [dbo].[Toppings] ([Id])
GO
ALTER TABLE [dbo].[PedidosProductosToppings] CHECK CONSTRAINT [FK_dbo.PedidosProductosToppings_dbo.Toppings_IdTopping]
GO
ALTER TABLE [dbo].[Regalos]  WITH CHECK ADD  CONSTRAINT [FK_Regalos_ClienteReceptor] FOREIGN KEY([IdClienteReceptor])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Regalos] CHECK CONSTRAINT [FK_Regalos_ClienteReceptor]
GO
ALTER TABLE [dbo].[Regalos]  WITH CHECK ADD  CONSTRAINT [FK_Regalos_ClienteRemitente] FOREIGN KEY([IdClienteRemitente])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Regalos] CHECK CONSTRAINT [FK_Regalos_ClienteRemitente]
GO
ALTER TABLE [dbo].[Regalos]  WITH CHECK ADD  CONSTRAINT [FK_Regalos_Pedidos] FOREIGN KEY([IdPedido])
REFERENCES [dbo].[Pedidos] ([Id])
GO
ALTER TABLE [dbo].[Regalos] CHECK CONSTRAINT [FK_Regalos_Pedidos]
GO
ALTER TABLE [dbo].[Salidas]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Salidas_dbo.Usuarios_IdUsuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Salidas] CHECK CONSTRAINT [FK_dbo.Salidas_dbo.Usuarios_IdUsuario]
GO
ALTER TABLE [dbo].[SalidasLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasLeches_dbo.Leches_IdLeche] FOREIGN KEY([IdLeche])
REFERENCES [dbo].[Leches] ([Id])
GO
ALTER TABLE [dbo].[SalidasLeches] CHECK CONSTRAINT [FK_dbo.SalidasLeches_dbo.Leches_IdLeche]
GO
ALTER TABLE [dbo].[SalidasLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasLeches_dbo.Salidas_IdSalida] FOREIGN KEY([IdSalida])
REFERENCES [dbo].[Salidas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SalidasLeches] CHECK CONSTRAINT [FK_dbo.SalidasLeches_dbo.Salidas_IdSalida]
GO
ALTER TABLE [dbo].[SalidasTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasTiposGranos_dbo.Salidas_IdSalida] FOREIGN KEY([IdSalida])
REFERENCES [dbo].[Salidas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SalidasTiposGranos] CHECK CONSTRAINT [FK_dbo.SalidasTiposGranos_dbo.Salidas_IdSalida]
GO
ALTER TABLE [dbo].[SalidasTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasTiposGranos_dbo.TiposGranos_IdTipoGrano] FOREIGN KEY([IdTipoGrano])
REFERENCES [dbo].[TiposGranos] ([Id])
GO
ALTER TABLE [dbo].[SalidasTiposGranos] CHECK CONSTRAINT [FK_dbo.SalidasTiposGranos_dbo.TiposGranos_IdTipoGrano]
GO
ALTER TABLE [dbo].[SalidasToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasToppings_dbo.Salidas_IdSalida] FOREIGN KEY([IdSalida])
REFERENCES [dbo].[Salidas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SalidasToppings] CHECK CONSTRAINT [FK_dbo.SalidasToppings_dbo.Salidas_IdSalida]
GO
ALTER TABLE [dbo].[SalidasToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasToppings_dbo.Toppings_IdTopping] FOREIGN KEY([IdTopping])
REFERENCES [dbo].[Toppings] ([Id])
GO
ALTER TABLE [dbo].[SalidasToppings] CHECK CONSTRAINT [FK_dbo.SalidasToppings_dbo.Toppings_IdTopping]
GO
ALTER TABLE [dbo].[SalidasVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasVasos_dbo.Salidas_IdSalida] FOREIGN KEY([IdSalida])
REFERENCES [dbo].[Salidas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SalidasVasos] CHECK CONSTRAINT [FK_dbo.SalidasVasos_dbo.Salidas_IdSalida]
GO
ALTER TABLE [dbo].[SalidasVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SalidasVasos_dbo.Vasos_IdVaso] FOREIGN KEY([IdVaso])
REFERENCES [dbo].[Vasos] ([Id])
GO
ALTER TABLE [dbo].[SalidasVasos] CHECK CONSTRAINT [FK_dbo.SalidasVasos_dbo.Vasos_IdVaso]
GO
ALTER TABLE [dbo].[SubClasificaciones]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificaciones_dbo.Clasificaciones_IdClasificacion] FOREIGN KEY([IdClasificacion])
REFERENCES [dbo].[Clasificaciones] ([Id])
GO
ALTER TABLE [dbo].[SubClasificaciones] CHECK CONSTRAINT [FK_dbo.SubClasificaciones_dbo.Clasificaciones_IdClasificacion]
GO
ALTER TABLE [dbo].[SubClasificacionesLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesLeches_dbo.Leches_IdLeche] FOREIGN KEY([IdLeche])
REFERENCES [dbo].[Leches] ([Id])
GO
ALTER TABLE [dbo].[SubClasificacionesLeches] CHECK CONSTRAINT [FK_dbo.SubClasificacionesLeches_dbo.Leches_IdLeche]
GO
ALTER TABLE [dbo].[SubClasificacionesLeches]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesLeches_dbo.SubClasificaciones_IdSubClasificacion] FOREIGN KEY([IdSubClasificacion])
REFERENCES [dbo].[SubClasificaciones] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubClasificacionesLeches] CHECK CONSTRAINT [FK_dbo.SubClasificacionesLeches_dbo.SubClasificaciones_IdSubClasificacion]
GO
ALTER TABLE [dbo].[SubClasificacionesTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesTiposGranos_dbo.SubClasificaciones_IdSubClasificacion] FOREIGN KEY([IdSubClasificacion])
REFERENCES [dbo].[SubClasificaciones] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubClasificacionesTiposGranos] CHECK CONSTRAINT [FK_dbo.SubClasificacionesTiposGranos_dbo.SubClasificaciones_IdSubClasificacion]
GO
ALTER TABLE [dbo].[SubClasificacionesTiposGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesTiposGranos_dbo.TiposGranos_IdTipoGrano] FOREIGN KEY([IdTipoGrano])
REFERENCES [dbo].[TiposGranos] ([Id])
GO
ALTER TABLE [dbo].[SubClasificacionesTiposGranos] CHECK CONSTRAINT [FK_dbo.SubClasificacionesTiposGranos_dbo.TiposGranos_IdTipoGrano]
GO
ALTER TABLE [dbo].[SubClasificacionesToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesToppings_dbo.SubClasificaciones_IdSubClasificacion] FOREIGN KEY([IdSubClasificacion])
REFERENCES [dbo].[SubClasificaciones] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubClasificacionesToppings] CHECK CONSTRAINT [FK_dbo.SubClasificacionesToppings_dbo.SubClasificaciones_IdSubClasificacion]
GO
ALTER TABLE [dbo].[SubClasificacionesToppings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesToppings_dbo.Toppings_IdTopping] FOREIGN KEY([IdTopping])
REFERENCES [dbo].[Toppings] ([Id])
GO
ALTER TABLE [dbo].[SubClasificacionesToppings] CHECK CONSTRAINT [FK_dbo.SubClasificacionesToppings_dbo.Toppings_IdTopping]
GO
ALTER TABLE [dbo].[SubClasificacionesVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesVasos_dbo.SubClasificaciones_IdSubClasificacion] FOREIGN KEY([IdSubClasificacion])
REFERENCES [dbo].[SubClasificaciones] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubClasificacionesVasos] CHECK CONSTRAINT [FK_dbo.SubClasificacionesVasos_dbo.SubClasificaciones_IdSubClasificacion]
GO
ALTER TABLE [dbo].[SubClasificacionesVasos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SubClasificacionesVasos_dbo.Vasos_IdVaso] FOREIGN KEY([IdVaso])
REFERENCES [dbo].[Vasos] ([Id])
GO
ALTER TABLE [dbo].[SubClasificacionesVasos] CHECK CONSTRAINT [FK_dbo.SubClasificacionesVasos_dbo.Vasos_IdVaso]
GO
USE [master]
GO
ALTER DATABASE [dbcafeteria] SET  READ_WRITE 
GO
