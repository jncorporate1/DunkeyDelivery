namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storedeli : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        FullName = c.String(),
                        BusinessName = c.String(nullable: false),
                        BusinessType = c.String(nullable: false),
                        ZipCode = c.String(),
                        Email = c.String(nullable: false),
                        Phone = c.String(),
                        Role = c.Short(nullable: false),
                        Password = c.String(),
                        AccountNo = c.String(),
                        Store_Id = c.Int(),
                        Status = c.Short(),
                        IsDeleted = c.Boolean(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessType = c.String(nullable: false),
                        Description = c.String(),
                        BusinessName = c.String(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Open_From = c.Time(nullable: false, precision: 7),
                        Open_To = c.Time(nullable: false, precision: 7),
                        ImageUrl = c.String(),
                        Address = c.String(),
                        ContactNumber = c.String(),
                        MinDeliveryTime = c.Int(nullable: false),
                        MinDeliveryCharges = c.Decimal(precision: 18, scale: 2),
                        MinOrderPrice = c.Single(),
                        IsDeleted = c.Boolean(nullable: false),
                        Location = c.Geography(),
                        ImageDeletedOnEdit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Status = c.Short(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        ParentCategoryId = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Description = c.String(),
                        Image = c.String(),
                        Status = c.Short(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Category_Id = c.Int(),
                        Store_Id = c.Int(nullable: false),
                        Size = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Product_Id = c.Int(nullable: false),
                        User_ID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        FullName = c.String(),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Address = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Dob = c.DateTime(),
                        Role = c.Short(nullable: false),
                        Username = c.String(),
                        Status = c.Short(nullable: false),
                        VerificationToken = c.String(),
                        ProfilePictureUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CreditCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CCNo = c.String(nullable: false),
                        ExpiryDate = c.String(nullable: false),
                        CCV = c.String(nullable: false),
                        BillingCode = c.String(nullable: false),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.ForgetPasswordTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        User_Id = c.Int(nullable: false),
                        User1_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Users", t => t.User1_Id)
                .Index(t => t.User_Id)
                .Index(t => t.User1_Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        OrderDateTime = c.DateTime(nullable: false),
                        DeliveryTime_From = c.DateTime(),
                        DeliveryTime_To = c.DateTime(),
                        AdditionalNote = c.String(),
                        PaymentMethod = c.Int(nullable: false),
                        Subtotal = c.Double(nullable: false),
                        ServiceFee = c.Double(nullable: false),
                        DeliveryFee = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        User_ID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        OrderPayment_Id = c.Int(),
                        PaymentStatus = c.Short(nullable: false),
                        DeliveryAddress = c.String(),
                        DeliveryMan_Id = c.Int(),
                        RemoveFromDelivererHistory = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.OrderPayments",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Amount = c.String(nullable: false),
                        PaymentType = c.String(nullable: false),
                        CashCollected = c.String(nullable: false),
                        Status = c.String(nullable: false),
                        Order_Id = c.String(nullable: false),
                        AccountNo = c.String(nullable: false),
                        DeliveryMan_Id = c.Int(),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.StoreOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        Subtotal = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Order_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.Order_Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Qty = c.Int(nullable: false),
                        Product_Id = c.Int(),
                        Package_Id = c.Int(),
                        Offer_Product_Id = c.Int(),
                        Offer_Package_Id = c.Int(),
                        StoreOrder_Id = c.Int(nullable: false),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offer_Packages", t => t.Offer_Package_Id)
                .ForeignKey("dbo.Offer_Products", t => t.Offer_Product_Id)
                .ForeignKey("dbo.Packages", t => t.Package_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.StoreOrders", t => t.StoreOrder_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Package_Id)
                .Index(t => t.Offer_Product_Id)
                .Index(t => t.Offer_Package_Id)
                .Index(t => t.StoreOrder_Id);
            
            CreateTable(
                "dbo.Offer_Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Offer_Id = c.Int(nullable: false),
                        Name = c.String(),
                        ValidUpto = c.DateTime(nullable: false),
                        DiscountedPrice = c.Int(nullable: false),
                        Description = c.String(),
                        DiscountPercentage = c.Int(nullable: false),
                        SlashPrice = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Package_Id = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Offer_Package_Id = c.Int(),
                        Offer_Product_Id = c.Int(),
                        Package_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.Offer_Id)
                .ForeignKey("dbo.Offer_Packages", t => t.Offer_Package_Id)
                .ForeignKey("dbo.Offer_Products", t => t.Offer_Product_Id)
                .ForeignKey("dbo.Packages", t => t.Package_Id1)
                .Index(t => t.Offer_Id)
                .Index(t => t.Offer_Package_Id)
                .Index(t => t.Offer_Product_Id)
                .Index(t => t.Package_Id1);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidUpto = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        Status = c.String(nullable: false),
                        ImageUrl = c.String(),
                        Store_Id = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Offer_Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Offer_Id = c.Int(nullable: false),
                        Name = c.String(),
                        Product_Id = c.Int(nullable: false),
                        ValidUpto = c.DateTime(nullable: false),
                        descript = c.String(),
                        DiscountedPrice = c.Int(nullable: false),
                        DiscountPercentage = c.Int(nullable: false),
                        SlashPrice = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.Offer_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Offer_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Status = c.String(nullable: false),
                        Price = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Package_Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Qty = c.String(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        Package_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Packages", t => t.Package_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Package_Id);
            
            CreateTable(
                "dbo.Payment_Details",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankName = c.String(nullable: false),
                        CardNo = c.String(nullable: false),
                        CVV = c.String(nullable: false),
                        ValidUpto = c.DateTime(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_ID = c.Int(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Telephone = c.String(nullable: false),
                        FullAddress = c.String(nullable: false),
                        PostalCode = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        City = c.String(nullable: false),
                        VehicleType = c.String(nullable: false),
                        LicenseNo = c.String(),
                        LicenseImage = c.String(),
                        HearFrom = c.String(),
                        Store_Id = c.Int(nullable: false),
                        Store_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id1)
                .Index(t => t.Store_Id1);
            
            CreateTable(
                "dbo.Driver_Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeliveryTime = c.DateTime(),
                        Driver_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Drivers", t => t.Driver_Id)
                .Index(t => t.Driver_Id);
            
            CreateTable(
                "dbo.Payment_Method",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankName = c.String(nullable: false),
                        AccountNo = c.String(nullable: false),
                        CVV = c.String(nullable: false),
                        ValidUpto = c.DateTime(nullable: false),
                        Store_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.StoreDeliveryHours",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Monday_From = c.Time(nullable: false, precision: 7),
                        Monday_To = c.Time(nullable: false, precision: 7),
                        Tuesday_From = c.Time(nullable: false, precision: 7),
                        Tuesday_To = c.Time(nullable: false, precision: 7),
                        Wednesday_From = c.Time(nullable: false, precision: 7),
                        Wednesday_To = c.Time(nullable: false, precision: 7),
                        Thursday_From = c.Time(nullable: false, precision: 7),
                        Thursday_To = c.Time(nullable: false, precision: 7),
                        Friday_From = c.Time(nullable: false, precision: 7),
                        Friday_To = c.Time(nullable: false, precision: 7),
                        Saturday_From = c.Time(nullable: false, precision: 7),
                        Saturday_To = c.Time(nullable: false, precision: 7),
                        Sunday_From = c.Time(nullable: false, precision: 7),
                        Sunday_To = c.Time(nullable: false, precision: 7),
                        Store_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.StoreRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        User_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id1)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id)
                .Index(t => t.User_Id1);
            
            CreateTable(
                "dbo.StoreTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tag = c.String(),
                        Store_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.BlogComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        CategoryType = c.String(),
                        ImageUrl = c.String(),
                        Description = c.String(nullable: false),
                        DateOfPosting = c.DateTime(nullable: false),
                        is_popular = c.Short(nullable: false),
                        Comments_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogComments", t => t.Comments_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Comments_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ContactUs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Riders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        BusinessName = c.String(),
                        BusinessType = c.String(),
                        Email = c.String(nullable: false),
                        ZipCode = c.String(),
                        Status = c.Short(),
                        Phone = c.String(nullable: false),
                        SignInType = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeliveryFee = c.Double(nullable: false),
                        Currency = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogPosts", "User_Id", "dbo.Users");
            DropForeignKey("dbo.BlogPosts", "Comments_Id", "dbo.BlogComments");
            DropForeignKey("dbo.BlogComments", "User_Id", "dbo.Users");
            DropForeignKey("dbo.StoreTags", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreRatings", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreRatings", "User_Id1", "dbo.Users");
            DropForeignKey("dbo.StoreOrders", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreDeliveryHours", "Id", "dbo.Stores");
            DropForeignKey("dbo.Products", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Payment_Method", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Packages", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Offers", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Drivers", "Store_Id1", "dbo.Stores");
            DropForeignKey("dbo.Driver_Orders", "Driver_Id", "dbo.Drivers");
            DropForeignKey("dbo.Categories", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Package_Products", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Offer_Products", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Favourites", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.UserAddresses", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Payment_Details", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Orders", "User_ID", "dbo.Users");
            DropForeignKey("dbo.StoreOrders", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Order_Items", "StoreOrder_Id", "dbo.StoreOrders");
            DropForeignKey("dbo.Order_Items", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Order_Items", "Package_Id", "dbo.Packages");
            DropForeignKey("dbo.Order_Items", "Offer_Product_Id", "dbo.Offer_Products");
            DropForeignKey("dbo.Order_Items", "Offer_Package_Id", "dbo.Offer_Packages");
            DropForeignKey("dbo.Offer_Packages", "Package_Id1", "dbo.Packages");
            DropForeignKey("dbo.Package_Products", "Package_Id", "dbo.Packages");
            DropForeignKey("dbo.Offer_Packages", "Offer_Product_Id", "dbo.Offer_Products");
            DropForeignKey("dbo.Offer_Packages", "Offer_Package_Id", "dbo.Offer_Packages");
            DropForeignKey("dbo.Offer_Products", "Offer_Id", "dbo.Offers");
            DropForeignKey("dbo.Offer_Packages", "Offer_Id", "dbo.Offers");
            DropForeignKey("dbo.OrderPayments", "Id", "dbo.Orders");
            DropForeignKey("dbo.Notifications", "User1_Id", "dbo.Users");
            DropForeignKey("dbo.Notifications", "User_Id", "dbo.Users");
            DropForeignKey("dbo.ForgetPasswordTokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Favourites", "User_ID", "dbo.Users");
            DropForeignKey("dbo.CreditCards", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Admins", "Store_Id", "dbo.Stores");
            DropIndex("dbo.BlogPosts", new[] { "User_Id" });
            DropIndex("dbo.BlogPosts", new[] { "Comments_Id" });
            DropIndex("dbo.BlogComments", new[] { "User_Id" });
            DropIndex("dbo.StoreTags", new[] { "Store_Id" });
            DropIndex("dbo.StoreRatings", new[] { "User_Id1" });
            DropIndex("dbo.StoreRatings", new[] { "Store_Id" });
            DropIndex("dbo.StoreDeliveryHours", new[] { "Id" });
            DropIndex("dbo.Payment_Method", new[] { "Store_Id" });
            DropIndex("dbo.Driver_Orders", new[] { "Driver_Id" });
            DropIndex("dbo.Drivers", new[] { "Store_Id1" });
            DropIndex("dbo.UserAddresses", new[] { "User_ID" });
            DropIndex("dbo.Payment_Details", new[] { "User_Id" });
            DropIndex("dbo.Package_Products", new[] { "Package_Id" });
            DropIndex("dbo.Package_Products", new[] { "Product_Id" });
            DropIndex("dbo.Packages", new[] { "Store_Id" });
            DropIndex("dbo.Offer_Products", new[] { "Product_Id" });
            DropIndex("dbo.Offer_Products", new[] { "Offer_Id" });
            DropIndex("dbo.Offers", new[] { "Store_Id" });
            DropIndex("dbo.Offer_Packages", new[] { "Package_Id1" });
            DropIndex("dbo.Offer_Packages", new[] { "Offer_Product_Id" });
            DropIndex("dbo.Offer_Packages", new[] { "Offer_Package_Id" });
            DropIndex("dbo.Offer_Packages", new[] { "Offer_Id" });
            DropIndex("dbo.Order_Items", new[] { "StoreOrder_Id" });
            DropIndex("dbo.Order_Items", new[] { "Offer_Package_Id" });
            DropIndex("dbo.Order_Items", new[] { "Offer_Product_Id" });
            DropIndex("dbo.Order_Items", new[] { "Package_Id" });
            DropIndex("dbo.Order_Items", new[] { "Product_Id" });
            DropIndex("dbo.StoreOrders", new[] { "Order_Id" });
            DropIndex("dbo.StoreOrders", new[] { "Store_Id" });
            DropIndex("dbo.OrderPayments", new[] { "Id" });
            DropIndex("dbo.Orders", new[] { "User_ID" });
            DropIndex("dbo.Notifications", new[] { "User1_Id" });
            DropIndex("dbo.Notifications", new[] { "User_Id" });
            DropIndex("dbo.ForgetPasswordTokens", new[] { "User_Id" });
            DropIndex("dbo.CreditCards", new[] { "User_ID" });
            DropIndex("dbo.Favourites", new[] { "User_ID" });
            DropIndex("dbo.Favourites", new[] { "Product_Id" });
            DropIndex("dbo.Products", new[] { "Store_Id" });
            DropIndex("dbo.Products", new[] { "Category_Id" });
            DropIndex("dbo.Categories", new[] { "Store_Id" });
            DropIndex("dbo.Admins", new[] { "Store_Id" });
            DropTable("dbo.Settings");
            DropTable("dbo.Riders");
            DropTable("dbo.ContactUs");
            DropTable("dbo.BlogPosts");
            DropTable("dbo.BlogComments");
            DropTable("dbo.StoreTags");
            DropTable("dbo.StoreRatings");
            DropTable("dbo.StoreDeliveryHours");
            DropTable("dbo.Payment_Method");
            DropTable("dbo.Driver_Orders");
            DropTable("dbo.Drivers");
            DropTable("dbo.UserAddresses");
            DropTable("dbo.Payment_Details");
            DropTable("dbo.Package_Products");
            DropTable("dbo.Packages");
            DropTable("dbo.Offer_Products");
            DropTable("dbo.Offers");
            DropTable("dbo.Offer_Packages");
            DropTable("dbo.Order_Items");
            DropTable("dbo.StoreOrders");
            DropTable("dbo.OrderPayments");
            DropTable("dbo.Orders");
            DropTable("dbo.Notifications");
            DropTable("dbo.ForgetPasswordTokens");
            DropTable("dbo.CreditCards");
            DropTable("dbo.Users");
            DropTable("dbo.Favourites");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
            DropTable("dbo.Stores");
            DropTable("dbo.Admins");
        }
    }
}
