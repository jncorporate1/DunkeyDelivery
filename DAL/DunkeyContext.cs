namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DunkeyContext : DbContext
    {
        public DunkeyContext()
           : base("name=DunkeyContextDev")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DunkeyContext, DAL.Migrations.Configuration>());
            Configuration.ProxyCreationEnabled = false;
            //Configuration.Aut
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Driver_Orders> Driver_Orders { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order_Items> Order_Items { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Payment_Details> Payment_Details { get; set; }
        public virtual DbSet<Payment_Method> Payment_Method { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<StoreRatings> StoreRatings { get; set; }
        public virtual DbSet<StoreOrder> StoreOrders { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }

        //public virtual DbSet<Store_Timings> Store_Timings { get; set; }
        public virtual DbSet<OrderPayment> OrderPayments { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<ForgetPasswordTokens> ForgotPasswordTokens { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Offer_Products> Offer_Products { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Offer_Packages> Offer_Packages { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Package_Products> Package_Products { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<Rider> Rider { get; set; }
        public virtual DbSet<StoreTags> StoreTags { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<BlogPosts> BlogPosts { get; set; }
        public virtual DbSet<BlogComments> BlogComments { get; set; }
        public virtual DbSet<StoreDeliveryHours> StoreDeliveryHours { get; set; }
        public virtual DbSet<RewardMilestones> RewardMilestones { get; set; }
        public virtual DbSet<UserRewards> UserRewards { get; set; }
        public virtual DbSet<RewardPrize> RewardPrize { get; set; }





        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasMany(e => e.CreditCards)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.UserAddresses)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_ID)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.Admins)
                .WithOptional(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);
            // new modifications 

            modelBuilder.Entity<RewardPrize>()
            .HasMany(e => e.RewardMilestones)
            .WithOptional(e => e.RewardPrize)
            .HasForeignKey(e => e.RewardPrize_Id)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.UserRewards)
              .WithRequired(e => e.User)
              .HasForeignKey(e => e.User_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<RewardMilestones>()
              .HasMany(e => e.UserRewards)
              .WithRequired(e => e.RewardMilestones)
              .HasForeignKey(e => e.RewardMilestones_Id)
              .WillCascadeOnDelete(false);


            modelBuilder.Entity<BlogPosts>()
                .HasMany(e => e.BlogComments)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.BlogComments)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);



            modelBuilder.Entity<User>()
               .HasMany(e => e.BlogPosts)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_ID)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                 .HasMany(e => e.Packages)
                 .WithRequired(e => e.Store)
                 .HasForeignKey(e => e.Store_Id)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order_Items>()
                 .HasOptional(x => x.Offer_Product);

            modelBuilder.Entity<Order_Items>()
                .HasOptional(x => x.Offer_Package);

            modelBuilder.Entity<Order_Items>()
                .HasOptional(x => x.Product);

            modelBuilder.Entity<Order_Items>()
                .HasOptional(x => x.Package);

      


            modelBuilder.Entity<StoreOrder>()
                .HasMany(e => e.Order_Items)
                .WithRequired(e => e.StoreOrder)
                .HasForeignKey(e => e.StoreOrder_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.StoreOrders)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.Order_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderPayment>()
                .HasRequired(e => e.Order)
                .WithRequiredDependent(e => e.OrderPayment)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Product>()
                .HasMany(e => e.Favourites)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.Product_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.Favourites)
              .WithRequired(e => e.User)
              .HasForeignKey(e => e.User_ID)
              .WillCascadeOnDelete(false);



            modelBuilder.Entity<Store>()
                .HasOptional(s => s.StoreDeliveryHours)
                .WithRequired(ad => ad.Store);
            // new modifications end


            modelBuilder.Entity<Store>()
                .HasMany(e => e.StoreTags)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasOptional(s => s.StoreDeliveryHours);



            modelBuilder.Entity<Package>()
                          .HasMany(e => e.Package_Products)
                          .WithRequired(e => e.Package)
                          .HasForeignKey(e => e.Package_Id)
                          .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Package_Products)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.Product_Id)
                .WillCascadeOnDelete(false);

    

            modelBuilder.Entity<Offer>()
               .HasMany(e => e.Offer_Products)
               .WithRequired(e => e.Offer)
               .HasForeignKey(e => e.Offer_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Offer>()
                .HasMany(e => e.Offer_Packages)
                .WithRequired(e => e.Offer)
                .HasForeignKey(e => e.Offer_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Store>()
                .HasMany(e => e.Offers)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Offer_Products)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.Product_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ForgetPasswordToken)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            //Above are custom added

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Category)
                .HasForeignKey(e => e.Category_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Driver>()
                .HasMany(e => e.Driver_Orders)
                .WithRequired(e => e.Driver)
                .HasForeignKey(e => e.Driver_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Order>()
            //    .HasMany(e => e.Driver_Orders)
            //    .WithRequired(e => e.Order)
            //    .HasForeignKey(e => e.Order_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Order>()
            //    .HasMany(e => e.Order_Items)
            //    .WithRequired(e => e.Order)
            //    .HasForeignKey(e => e.Order_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.Order_Items)
            //    .WithRequired(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .Property(e => e.Latitude);
            //.HasPrecision(18, 0);

            modelBuilder.Entity<Store>()
                .Property(e => e.Longitude);
            //.HasPrecision(18, 0);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.Categories)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Store>()
                .HasMany(e => e.StoreOrders)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);
            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Orders)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.Payment_Method)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
                .HasMany(e => e.StoreRatings)
                .WithRequired(e => e.Store)
                .HasForeignKey(e => e.Store_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.StoreDeliveryHours)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);




            modelBuilder.Entity<User>()
                .HasMany(e => e.Notifications)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notifications1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.User1_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Payment_Details)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);
        }
    }
}
