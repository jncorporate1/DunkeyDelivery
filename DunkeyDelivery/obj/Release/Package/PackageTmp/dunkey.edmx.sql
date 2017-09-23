
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/15/2017 18:56:51
-- Generated from EDMX file: D:\Git\DunkeyDelivery\DunkeyDelivery\dunkey.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DunkeyDelivery];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UsersPayment_Details]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Payment_Details] DROP CONSTRAINT [FK_UsersPayment_Details];
GO
IF OBJECT_ID(N'[dbo].[FK_OrdersOrder_Items]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order_Items] DROP CONSTRAINT [FK_OrdersOrder_Items];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductsOrder_Items]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Order_Items] DROP CONSTRAINT [FK_ProductsOrder_Items];
GO
IF OBJECT_ID(N'[dbo].[FK_CategoriesProducts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_CategoriesProducts];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreCategories]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [FK_StoreCategories];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreStore_Rating]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Store_Rating] DROP CONSTRAINT [FK_StoreStore_Rating];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreStore_Timings]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Store_Timings] DROP CONSTRAINT [FK_StoreStore_Timings];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersOrders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_UsersOrders];
GO
IF OBJECT_ID(N'[dbo].[FK_StorePayment_Method]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Payment_Method] DROP CONSTRAINT [FK_StorePayment_Method];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreDriver]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Drivers] DROP CONSTRAINT [FK_StoreDriver];
GO
IF OBJECT_ID(N'[dbo].[FK_DriverDriver_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Driver_Orders] DROP CONSTRAINT [FK_DriverDriver_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_OrdersDriver_Orders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Driver_Orders] DROP CONSTRAINT [FK_OrdersDriver_Orders];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersNotification]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notifications] DROP CONSTRAINT [FK_UsersNotification];
GO
IF OBJECT_ID(N'[dbo].[FK_UsersNotification1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notifications] DROP CONSTRAINT [FK_UsersNotification1];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreOrders]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_StoreOrders];
GO
IF OBJECT_ID(N'[dbo].[FK_StoreProducts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_StoreProducts];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Payment_Details]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Payment_Details];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[Order_Items]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Order_Items];
GO
IF OBJECT_ID(N'[dbo].[Stores]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stores];
GO
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Categories];
GO
IF OBJECT_ID(N'[dbo].[Products]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Products];
GO
IF OBJECT_ID(N'[dbo].[Payment_Method]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Payment_Method];
GO
IF OBJECT_ID(N'[dbo].[Store_Timings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Store_Timings];
GO
IF OBJECT_ID(N'[dbo].[Store_Rating]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Store_Rating];
GO
IF OBJECT_ID(N'[dbo].[Drivers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Drivers];
GO
IF OBJECT_ID(N'[dbo].[Notifications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notifications];
GO
IF OBJECT_ID(N'[dbo].[Driver_Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Driver_Orders];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [FullName] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [City] nvarchar(max)  NULL,
    [State] nvarchar(max)  NULL,
    [Country] nvarchar(max)  NULL,
    [Dob] datetime  NULL,
    [Role] smallint  NOT NULL,
    [Username] nvarchar(max)  NULL,
    [Status] smallint  NOT NULL,
    [ForgetPasswordToken] nvarchar(max)  NULL,
    [VerificationToken] nvarchar(max)  NULL,
    [ProfilePictureUrl] nvarchar(max)  NULL
);
GO

-- Creating table 'Payment_Details'
CREATE TABLE [dbo].[Payment_Details] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BankName] nvarchar(max)  NOT NULL,
    [CardNo] nvarchar(max)  NOT NULL,
    [CVV] nvarchar(max)  NOT NULL,
    [ValidUpto] datetime  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OrderNo] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Status] smallint  NULL,
    [DeliveryDateTime] datetime  NOT NULL,
    [User_Id] int  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Order_Items'
CREATE TABLE [dbo].[Order_Items] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Quantity] int  NOT NULL,
    [Price] int  NOT NULL,
    [Order_Id] int  NOT NULL,
    [Product_Id] int  NOT NULL
);
GO

-- Creating table 'Stores'
CREATE TABLE [dbo].[Stores] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [BusinessType] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NULL,
    [ZipCode] smallint  NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [BusinessName] nvarchar(max)  NOT NULL,
    [Latitude] float  NULL,
    [Longitude] float  NULL,
    [ImageUrl] nvarchar(max)  NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Status] smallint  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Price] int  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Image] nvarchar(max)  NULL,
    [Status] smallint  NOT NULL,
    [Category_Id] int  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Payment_Method'
CREATE TABLE [dbo].[Payment_Method] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BankName] nvarchar(max)  NOT NULL,
    [AccountNo] nvarchar(max)  NOT NULL,
    [CVV] nvarchar(max)  NOT NULL,
    [ValidUpto] datetime  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Store_Timings'
CREATE TABLE [dbo].[Store_Timings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Shift] nvarchar(max)  NOT NULL,
    [Timing] nvarchar(max)  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Store_Rating'
CREATE TABLE [dbo].[Store_Rating] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Rating] smallint  NOT NULL,
    [Feedback] nvarchar(max)  NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Drivers'
CREATE TABLE [dbo].[Drivers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [VehicleType] nvarchar(max)  NOT NULL,
    [LicenseNo] nvarchar(max)  NOT NULL,
    [ExpiryDate] datetime  NOT NULL,
    [LicenseImage] nvarchar(max)  NOT NULL,
    [Store_Id] int  NOT NULL
);
GO

-- Creating table 'Notifications'
CREATE TABLE [dbo].[Notifications] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Body] nvarchar(max)  NOT NULL,
    [User_Id] int  NOT NULL,
    [User1_Id] int  NOT NULL
);
GO

-- Creating table 'Driver_Orders'
CREATE TABLE [dbo].[Driver_Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeliveryTime] datetime  NULL,
    [Driver_Id] int  NOT NULL,
    [Order_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Payment_Details'
ALTER TABLE [dbo].[Payment_Details]
ADD CONSTRAINT [PK_Payment_Details]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Order_Items'
ALTER TABLE [dbo].[Order_Items]
ADD CONSTRAINT [PK_Order_Items]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Stores'
ALTER TABLE [dbo].[Stores]
ADD CONSTRAINT [PK_Stores]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Payment_Method'
ALTER TABLE [dbo].[Payment_Method]
ADD CONSTRAINT [PK_Payment_Method]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Store_Timings'
ALTER TABLE [dbo].[Store_Timings]
ADD CONSTRAINT [PK_Store_Timings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Store_Rating'
ALTER TABLE [dbo].[Store_Rating]
ADD CONSTRAINT [PK_Store_Rating]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Drivers'
ALTER TABLE [dbo].[Drivers]
ADD CONSTRAINT [PK_Drivers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [PK_Notifications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Driver_Orders'
ALTER TABLE [dbo].[Driver_Orders]
ADD CONSTRAINT [PK_Driver_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'Payment_Details'
ALTER TABLE [dbo].[Payment_Details]
ADD CONSTRAINT [FK_UsersPayment_Details]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersPayment_Details'
CREATE INDEX [IX_FK_UsersPayment_Details]
ON [dbo].[Payment_Details]
    ([User_Id]);
GO

-- Creating foreign key on [Order_Id] in table 'Order_Items'
ALTER TABLE [dbo].[Order_Items]
ADD CONSTRAINT [FK_OrdersOrder_Items]
    FOREIGN KEY ([Order_Id])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrdersOrder_Items'
CREATE INDEX [IX_FK_OrdersOrder_Items]
ON [dbo].[Order_Items]
    ([Order_Id]);
GO

-- Creating foreign key on [Product_Id] in table 'Order_Items'
ALTER TABLE [dbo].[Order_Items]
ADD CONSTRAINT [FK_ProductsOrder_Items]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductsOrder_Items'
CREATE INDEX [IX_FK_ProductsOrder_Items]
ON [dbo].[Order_Items]
    ([Product_Id]);
GO

-- Creating foreign key on [Category_Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_CategoriesProducts]
    FOREIGN KEY ([Category_Id])
    REFERENCES [dbo].[Categories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CategoriesProducts'
CREATE INDEX [IX_FK_CategoriesProducts]
ON [dbo].[Products]
    ([Category_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [FK_StoreCategories]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreCategories'
CREATE INDEX [IX_FK_StoreCategories]
ON [dbo].[Categories]
    ([Store_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Store_Rating'
ALTER TABLE [dbo].[Store_Rating]
ADD CONSTRAINT [FK_StoreStore_Rating]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreStore_Rating'
CREATE INDEX [IX_FK_StoreStore_Rating]
ON [dbo].[Store_Rating]
    ([Store_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Store_Timings'
ALTER TABLE [dbo].[Store_Timings]
ADD CONSTRAINT [FK_StoreStore_Timings]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreStore_Timings'
CREATE INDEX [IX_FK_StoreStore_Timings]
ON [dbo].[Store_Timings]
    ([Store_Id]);
GO

-- Creating foreign key on [User_Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_UsersOrders]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersOrders'
CREATE INDEX [IX_FK_UsersOrders]
ON [dbo].[Orders]
    ([User_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Payment_Method'
ALTER TABLE [dbo].[Payment_Method]
ADD CONSTRAINT [FK_StorePayment_Method]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StorePayment_Method'
CREATE INDEX [IX_FK_StorePayment_Method]
ON [dbo].[Payment_Method]
    ([Store_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Drivers'
ALTER TABLE [dbo].[Drivers]
ADD CONSTRAINT [FK_StoreDriver]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreDriver'
CREATE INDEX [IX_FK_StoreDriver]
ON [dbo].[Drivers]
    ([Store_Id]);
GO

-- Creating foreign key on [Driver_Id] in table 'Driver_Orders'
ALTER TABLE [dbo].[Driver_Orders]
ADD CONSTRAINT [FK_DriverDriver_Orders]
    FOREIGN KEY ([Driver_Id])
    REFERENCES [dbo].[Drivers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DriverDriver_Orders'
CREATE INDEX [IX_FK_DriverDriver_Orders]
ON [dbo].[Driver_Orders]
    ([Driver_Id]);
GO

-- Creating foreign key on [Order_Id] in table 'Driver_Orders'
ALTER TABLE [dbo].[Driver_Orders]
ADD CONSTRAINT [FK_OrdersDriver_Orders]
    FOREIGN KEY ([Order_Id])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrdersDriver_Orders'
CREATE INDEX [IX_FK_OrdersDriver_Orders]
ON [dbo].[Driver_Orders]
    ([Order_Id]);
GO

-- Creating foreign key on [User_Id] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [FK_UsersNotification]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersNotification'
CREATE INDEX [IX_FK_UsersNotification]
ON [dbo].[Notifications]
    ([User_Id]);
GO

-- Creating foreign key on [User1_Id] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [FK_UsersNotification1]
    FOREIGN KEY ([User1_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UsersNotification1'
CREATE INDEX [IX_FK_UsersNotification1]
ON [dbo].[Notifications]
    ([User1_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_StoreOrders]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreOrders'
CREATE INDEX [IX_FK_StoreOrders]
ON [dbo].[Orders]
    ([Store_Id]);
GO

-- Creating foreign key on [Store_Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_StoreProducts]
    FOREIGN KEY ([Store_Id])
    REFERENCES [dbo].[Stores]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StoreProducts'
CREATE INDEX [IX_FK_StoreProducts]
ON [dbo].[Products]
    ([Store_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------