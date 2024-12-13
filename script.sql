USE [master]
GO
/****** Object:  Database [MonkDB]    Script Date: 11/25/2024 03:19:22 ******/
CREATE DATABASE [MonkDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MonkDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\MonkDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MonkDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\MonkDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [MonkDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MonkDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MonkDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MonkDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MonkDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MonkDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MonkDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [MonkDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MonkDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MonkDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MonkDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MonkDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MonkDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MonkDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MonkDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MonkDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MonkDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MonkDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MonkDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MonkDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MonkDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MonkDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MonkDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MonkDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MonkDB] SET RECOVERY FULL 
GO
ALTER DATABASE [MonkDB] SET  MULTI_USER 
GO
ALTER DATABASE [MonkDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MonkDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MonkDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MonkDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MonkDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MonkDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'MonkDB', N'ON'
GO
ALTER DATABASE [MonkDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [MonkDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MonkDB]
GO
/****** Object:  UserDefinedTableType [dbo].[CartItemsTableType]    Script Date: 11/25/2024 03:19:22 ******/
CREATE TYPE [dbo].[CartItemsTableType] AS TABLE(
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](18, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[type_BuyProducts]    Script Date: 11/25/2024 03:19:22 ******/
CREATE TYPE [dbo].[type_BuyProducts] AS TABLE(
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](8, 2) NULL,
	[TotalDiscount] [decimal](8, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[type_GetProducts]    Script Date: 11/25/2024 03:19:22 ******/
CREATE TYPE [dbo].[type_GetProducts] AS TABLE(
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](8, 2) NULL,
	[TotalDiscount] [decimal](8, 2) NULL
)
GO
/****** Object:  Table [dbo].[tblBuyProducts]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBuyProducts](
	[buyId] [int] IDENTITY(1,1) NOT NULL,
	[cuponId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[quantity] [int] NULL,
 CONSTRAINT [PK_tblBuyProducts] PRIMARY KEY CLUSTERED 
(
	[buyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCoupons]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCoupons](
	[cuponId] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](15) NOT NULL,
	[threshold] [decimal](8, 2) NULL,
	[discount] [decimal](8, 2) NULL,
	[productId] [int] NULL,
	[repitionLimit] [int] NULL,
	[expiryDate] [datetime] NOT NULL,
	[status] [bit] NULL,
 CONSTRAINT [PK_tblCoupons] PRIMARY KEY CLUSTERED 
(
	[cuponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblGetProduct]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGetProduct](
	[getId] [int] IDENTITY(1,1) NOT NULL,
	[cuponId] [int] NOT NULL,
	[productId] [int] NOT NULL,
	[quantity] [int] NULL,
 CONSTRAINT [PK_tblGetProduct] PRIMARY KEY CLUSTERED 
(
	[getId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblProducts]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProducts](
	[productId] [int] NOT NULL,
	[price] [decimal](8, 2) NOT NULL,
 CONSTRAINT [PK_tblProducts] PRIMARY KEY CLUSTERED 
(
	[productId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblBuyProducts] ON 

INSERT [dbo].[tblBuyProducts] ([buyId], [cuponId], [productId], [quantity]) VALUES (1, 12, 1, 3)
INSERT [dbo].[tblBuyProducts] ([buyId], [cuponId], [productId], [quantity]) VALUES (2, 12, 2, 3)
SET IDENTITY_INSERT [dbo].[tblBuyProducts] OFF
GO
SET IDENTITY_INSERT [dbo].[tblCoupons] ON 

INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (12, N'bxgy', NULL, NULL, NULL, 2, CAST(N'2024-12-01T17:16:11.387' AS DateTime), 1)
INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (13, N'product-wise', NULL, CAST(20.00 AS Decimal(8, 2)), NULL, NULL, CAST(N'2024-12-01T17:16:53.933' AS DateTime), 0)
INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (14, N'product-wise', NULL, CAST(20.00 AS Decimal(8, 2)), 1, NULL, CAST(N'2024-12-01T17:17:30.877' AS DateTime), 1)
INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (15, N'cart-wise', CAST(1100.00 AS Decimal(8, 2)), CAST(10.00 AS Decimal(8, 2)), NULL, NULL, CAST(N'2024-12-01T17:17:54.923' AS DateTime), 1)
INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (16, N'product-wise', CAST(0.00 AS Decimal(8, 2)), CAST(60.00 AS Decimal(8, 2)), 8, 0, CAST(N'2024-12-03T03:10:58.380' AS DateTime), 1)
INSERT [dbo].[tblCoupons] ([cuponId], [type], [threshold], [discount], [productId], [repitionLimit], [expiryDate], [status]) VALUES (17, N'product-wise', CAST(0.00 AS Decimal(8, 2)), CAST(5.00 AS Decimal(8, 2)), 8, 0, CAST(N'2024-12-03T03:29:06.400' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tblCoupons] OFF
GO
SET IDENTITY_INSERT [dbo].[tblGetProduct] ON 

INSERT [dbo].[tblGetProduct] ([getId], [cuponId], [productId], [quantity]) VALUES (1, 12, 3, 1)
SET IDENTITY_INSERT [dbo].[tblGetProduct] OFF
GO
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (1, CAST(20.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (2, CAST(30.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (3, CAST(30.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (4, CAST(30.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (5, CAST(40.50 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (6, CAST(90.89 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (7, CAST(250.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (8, CAST(150.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (9, CAST(150.00 AS Decimal(8, 2)))
INSERT [dbo].[tblProducts] ([productId], [price]) VALUES (10, CAST(360.00 AS Decimal(8, 2)))
GO
ALTER TABLE [dbo].[tblCoupons] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[tblBuyProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblBuyProducts_tblCoupons] FOREIGN KEY([cuponId])
REFERENCES [dbo].[tblCoupons] ([cuponId])
GO
ALTER TABLE [dbo].[tblBuyProducts] CHECK CONSTRAINT [FK_tblBuyProducts_tblCoupons]
GO
ALTER TABLE [dbo].[tblBuyProducts]  WITH CHECK ADD  CONSTRAINT [FK_tblBuyProducts_tblProducts] FOREIGN KEY([productId])
REFERENCES [dbo].[tblProducts] ([productId])
GO
ALTER TABLE [dbo].[tblBuyProducts] CHECK CONSTRAINT [FK_tblBuyProducts_tblProducts]
GO
ALTER TABLE [dbo].[tblGetProduct]  WITH CHECK ADD  CONSTRAINT [FK_tblGetProduct_tblCoupons] FOREIGN KEY([cuponId])
REFERENCES [dbo].[tblCoupons] ([cuponId])
GO
ALTER TABLE [dbo].[tblGetProduct] CHECK CONSTRAINT [FK_tblGetProduct_tblCoupons]
GO
ALTER TABLE [dbo].[tblGetProduct]  WITH CHECK ADD  CONSTRAINT [FK_tblGetProduct_tblProducts] FOREIGN KEY([productId])
REFERENCES [dbo].[tblProducts] ([productId])
GO
ALTER TABLE [dbo].[tblGetProduct] CHECK CONSTRAINT [FK_tblGetProduct_tblProducts]
GO
/****** Object:  StoredProcedure [dbo].[sp_CreateCoupons]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CreateCoupons]
    @type VARCHAR(20),
    @expiresOn DATETIME,
    @productId INT = NULL,
    @threshold DECIMAL(8,2) = NULL,
    @discount DECIMAL(8,2) = NULL,
    @repitionLimit INT = NULL,
    @buyProducts dbo.type_BuyProducts READONLY,
    @getProducts dbo.type_GetProducts READONLY,
    @result INT OUTPUT  
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @couponId INT;

    INSERT INTO TblCoupons (
        [Type],
        ExpiryDate,
        ProductId,
        Threshold,
        Discount,
        RepitionLimit,
        Status
    )
    VALUES (
        @type,
        @expiresOn,
        @productId,
        @threshold,
        @discount,
        @repitionLimit,
        1 
    );
    
    SET @couponId = SCOPE_IDENTITY();

    IF @couponId IS NOT NULL
    BEGIN
        IF LOWER(@type) = 'bxgy'
        BEGIN
            IF EXISTS (SELECT 1 FROM @buyProducts bp WHERE NOT EXISTS (SELECT 1 FROM TblProducts p WHERE p.productId = bp.ProductId))
            BEGIN
                SET @result = 0;  
                RETURN;
            END

            INSERT INTO TblBuyProducts (cuponId, ProductId, quantity)
            SELECT @couponId, ProductId, Quantity
            FROM @buyProducts;

            IF EXISTS (SELECT 1 FROM @getProducts gp WHERE NOT EXISTS (SELECT 1 FROM TblProducts p WHERE p.productId = gp.ProductId))
            BEGIN
                SET @result = 0; 
            END

            INSERT INTO TblGetProduct (cuponId, ProductId, Quantity)
            SELECT @couponId, ProductId, Quantity
            FROM @getProducts;
        END
        
        SET @result = 1;  -- Success
    END
    ELSE
    BEGIN
        SET @result = 0;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteCoupon]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_DeleteCoupon]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM TblCoupons WHERE CuponId = @id)
    BEGIN
        UPDATE TblCoupons
        SET Status = 0
        WHERE CuponId = @id;

        SELECT CAST(1 AS BIT) AS IsDeleted;
    END
    ELSE
    BEGIN
        SELECT CAST(0 AS BIT) AS IsDeleted;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_getAllCoupons]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getAllCoupons]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.Type,
        c.CuponId AS Id,
        c.ExpiryDate AS Expiry
    FROM TblCoupons c
    WHERE c.ExpiryDate > GETUTCDATE() 
    AND ISNULL(c.Status, 0) <> 0
	order by c.cuponId asc

    SELECT 
        c.CuponId AS CouponId,
        c.Threshold,
        c.Discount,
        c.ProductId,
        c.RepitionLimit
    FROM TblCoupons c
    WHERE c.ExpiryDate > GETUTCDATE() 
    AND ISNULL(c.Status, 0) <> 0

    SELECT 
        bp.BuyId,
        bp.CuponId AS CouponId,
        bp.ProductId,
        bp.Quantity
    FROM tblBuyProducts bp
    INNER JOIN TblCoupons c ON bp.CuponId = c.CuponId
    WHERE c.ExpiryDate > GETUTCDATE() 
    AND ISNULL(c.Status, 0) <> 0

    SELECT 
        gp.GetId,
        gp.CuponId AS CouponId,
        gp.ProductId,
        gp.Quantity
    FROM tblGetProduct gp
    INNER JOIN TblCoupons c ON gp.CuponId = c.CuponId
    WHERE c.ExpiryDate > GETUTCDATE() 
    AND ISNULL(c.Status, 0) <> 0
END;
GO
/****** Object:  StoredProcedure [dbo].[sp_getApplicableCoupons]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_getApplicableCoupons]
    @CartItems AS dbo.CartItemsTableType READONLY
AS
BEGIN
    SET NOCOUNT ON
    
    CREATE TABLE #CartSummary
    (
        ProductId INT,
        TotalQuantity INT,
        TotalAmount DECIMAL(8,2)
    );

    CREATE TABLE #ApplicableCoupons
    (
        CouponId INT,
        Type VARCHAR(20),
        Discount DECIMAL(18,2)
    );

    INSERT INTO #CartSummary (ProductId, TotalQuantity, TotalAmount)
    SELECT 
        ProductId,
        SUM(Quantity),
        SUM(Price * Quantity)
    FROM @CartItems
    GROUP BY ProductId;

    DECLARE @CartTotal DECIMAL(8,2) = (SELECT SUM(TotalAmount) FROM #CartSummary);

    WITH AllCouponTypes AS (
        -- Cart-wise coupons
        SELECT 
            c.CuponId,
            'cart-wise' as Type,
            (@CartTotal * c.discount / 100) as Discount
        FROM tblCoupons c
        WHERE lower(c.Type) = 'cart-wise'
            AND c.status = 1 
	    and c.expiryDate > GETUTCDATE()
            AND c.threshold <= @CartTotal

        UNION ALL

        -- Product-wise  
		SELECT 
		    c.CuponId,
		   'product-wise' as type,
		    (cs.TotalAmount * c.discount / 100) as discount
		FROM tblCoupons c
		INNER JOIN #CartSummary cs ON c.ProductId = cs.ProductId
		WHERE lower(c.Type) = 'product-wise' 
		    AND c.status = 1 
		    AND c.expiryDate > GETUTCDATE()


        UNION ALL

		--bxgy type
		SELECT DISTINCT
            c.CuponId,
            'bxgy' as Type,
            (
                SELECT TOP 1
                    (cs4.TotalAmount / cs4.TotalQuantity) * 
                    CASE 
                        WHEN EXISTS (
                            SELECT 1
                            FROM tblBuyProducts bp2
                            INNER JOIN #CartSummary cs2 ON bp2.ProductId = cs2.ProductId
                            WHERE bp2.CuponId = c.CuponId
                            AND cs2.TotalQuantity >= (bp2.quantity * c.repitionLimit)
                        ) THEN c.repitionLimit
                        ELSE (
                            SELECT 
                                LEAST(
                                    MIN(FLOOR(cs3.TotalQuantity / bp3.quantity)),
                                    c.repitionLimit
                                )
                            FROM tblBuyProducts bp3
                            INNER JOIN #CartSummary cs3 ON bp3.ProductId = cs3.ProductId
                            WHERE bp3.CuponId = c.CuponId
                            HAVING MIN(FLOOR(cs3.TotalQuantity / bp3.quantity)) > 0
                        )
                    END as Discount
                FROM tblGetProduct gp
                INNER JOIN #CartSummary cs4 ON gp.ProductId = cs4.ProductId
                WHERE gp.CuponId = c.CuponId
                ORDER BY (cs4.TotalAmount / cs4.TotalQuantity)
            ) as Discount
        FROM tblCoupons c
        INNER JOIN tblBuyProducts bp ON c.CuponId = bp.CuponId
        INNER JOIN #CartSummary cs ON bp.ProductId = cs.ProductId
        WHERE lower(c.Type) = 'bxgy'
            AND c.status = 1 
            AND c.expiryDate > GETUTCDATE()
    )

    INSERT INTO #ApplicableCoupons (CouponId, Type, Discount)
    SELECT 
        CuponId,
        Type,
        Discount
    FROM AllCouponTypes
    WHERE Discount > 0;

    SELECT 
        CouponId,
        Type,
        MAX(Discount) as Discount 
    FROM #ApplicableCoupons
    GROUP BY CouponId, Type
    ORDER BY MAX(Discount) DESC;

    DROP TABLE #CartSummary;
    DROP TABLE #ApplicableCoupons;
ENd


GO
/****** Object:  StoredProcedure [dbo].[sp_getCouponById]    Script Date: 11/25/2024 03:19:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_getCouponById]
    @CouponId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.Type,
        c.CuponId AS CouponId,
        c.Threshold,
		c.Discount,
        c.ProductId,
        c.ExpiryDate AS Expiry,
        c.RepitionLimit
    INTO #tempCoupon
    FROM TblCoupons c
    WHERE c.cuponId = @CouponId 
	AND c.ExpiryDate > GETUTCDATE() 
	and isnull(c.status,0) <> 0

    IF NOT EXISTS (SELECT 1 FROM #tempCoupon)
    BEGIN
        DROP TABLE #tempCoupon;
        RETURN;
    END

    SELECT c.CouponId as Id ,
	c.type, c.Expiry from #tempCoupon c;
    SELECT * from #tempCoupon
    SELECT 
        bp.BuyId,
        bp.CuponId AS CouponId,
        bp.ProductId,
        bp.quantity
    FROM tblBuyProducts bp
    WHERE bp.CuponId = @CouponId;

    SELECT 
        gp.GetId,
        gp.CuponId AS CouponId,
        gp.ProductId,
        gp.Quantity
    FROM tblGetProduct gp
    WHERE gp.CuponId = @CouponId;

    DROP TABLE #tempCoupon
END;
GO
USE [master]
GO
ALTER DATABASE [MonkDB] SET  READ_WRITE 
GO
