CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE categories (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" character varying(500) NOT NULL,
        slug character varying(300) NOT NULL,
        "ImageUrl" character varying(500),
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        "SortOrder" integer NOT NULL DEFAULT 0,
        "ParentId" uuid,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_categories" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_categories_categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES categories ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE coupons (
        "Id" uuid NOT NULL,
        "Code" character varying(50) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "Type" character varying(20) NOT NULL,
        "DiscountValue" numeric(10,4) NOT NULL,
        max_discount_amount numeric(18,2),
        max_discount_currency character varying(3),
        min_order_amount numeric(18,2),
        min_order_currency character varying(3),
        "MaxUsageCount" integer,
        "MaxUsagePerUser" integer,
        "CurrentUsageCount" integer NOT NULL,
        "ExpiresAt" timestamp with time zone,
        "IsActive" boolean NOT NULL DEFAULT TRUE,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_coupons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "FirstName" character varying(50) NOT NULL,
        "LastName" character varying(50) NOT NULL,
        email character varying(256) NOT NULL,
        "PasswordHash" character varying(256) NOT NULL,
        "Role" character varying(20) NOT NULL,
        "IsEmailVerified" boolean NOT NULL,
        "EmailVerificationToken" text,
        "RefreshToken" character varying(512),
        "RefreshTokenExpiry" timestamp with time zone,
        "PasswordResetToken" text,
        "PasswordResetTokenExpiry" timestamp with time zone,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE products (
        id uuid NOT NULL,
        name character varying(200) NOT NULL,
        description character varying(5000) NOT NULL,
        slug character varying(300) NOT NULL,
        price_amount numeric(18,2) NOT NULL,
        price_currency character varying(3) NOT NULL,
        compare_price_amount numeric(18,2),
        compare_price_currency character varying(3),
        stock_quantity integer NOT NULL,
        "LowStockThreshold" integer NOT NULL,
        is_active boolean NOT NULL,
        is_featured boolean NOT NULL,
        "CategoryId" uuid NOT NULL,
        image_urls jsonb NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        is_deleted boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_products" PRIMARY KEY (id),
        CONSTRAINT "FK_products_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES categories ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE carts (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_carts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_carts_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE orders (
        "Id" uuid NOT NULL,
        "OrderNumber" character varying(50) NOT NULL,
        "UserId" uuid NOT NULL,
        "Status" character varying(20) NOT NULL,
        shipping_line1 character varying(200) NOT NULL,
        shipping_line2 character varying(200),
        shipping_city character varying(100) NOT NULL,
        shipping_state character varying(100) NOT NULL,
        shipping_postal_code character varying(20) NOT NULL,
        shipping_country character varying(2) NOT NULL,
        subtotal_amount numeric(18,2) NOT NULL,
        currency character varying(3) NOT NULL,
        tax_amount numeric(18,2) NOT NULL,
        shipping_cost numeric(18,2) NOT NULL,
        discount_amount numeric(18,2) NOT NULL,
        total_amount numeric(18,2) NOT NULL,
        "CouponCode" text,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_orders" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_orders_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE user_addresses (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        line1 character varying(200) NOT NULL,
        line2 character varying(200),
        city character varying(100) NOT NULL,
        state character varying(100) NOT NULL,
        postal_code character varying(20) NOT NULL,
        country character varying(2) NOT NULL,
        "Label" character varying(50) NOT NULL,
        "IsDefault" boolean NOT NULL DEFAULT FALSE,
        CONSTRAINT "PK_user_addresses" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_user_addresses_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE reviews (
        "Id" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        rating integer NOT NULL,
        "Title" character varying(100) NOT NULL,
        "Body" character varying(2000) NOT NULL,
        "Status" character varying(20) NOT NULL,
        "IsVerifiedPurchase" boolean NOT NULL DEFAULT FALSE,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL DEFAULT FALSE,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_reviews" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_reviews_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products (id) ON DELETE CASCADE,
        CONSTRAINT "FK_reviews_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE cart_items (
        "Id" uuid NOT NULL,
        "CartId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "ProductName" character varying(200) NOT NULL,
        "ProductImageUrl" character varying(500),
        "Quantity" integer NOT NULL,
        unit_price_amount numeric(18,2) NOT NULL,
        unit_price_currency character varying(3) NOT NULL,
        CONSTRAINT "PK_cart_items" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_cart_items_carts_CartId" FOREIGN KEY ("CartId") REFERENCES carts ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE order_items (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "ProductName" character varying(200) NOT NULL,
        "Quantity" integer NOT NULL,
        unit_price_amount numeric(18,2) NOT NULL,
        unit_price_currency character varying(3) NOT NULL,
        CONSTRAINT "PK_order_items" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_order_items_orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES orders ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE payments (
        "Id" uuid NOT NULL,
        "OrderId" uuid NOT NULL,
        amount numeric(18,2) NOT NULL,
        currency character varying(3) NOT NULL,
        "Status" character varying(20) NOT NULL,
        stripe_payment_intent_id character varying(200),
        stripe_charge_id character varying(200),
        "PaymentMethod" character varying(50) NOT NULL,
        failure_reason character varying(500),
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        "CreatedBy" text,
        "UpdatedBy" text,
        "IsDeleted" boolean NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_payments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_payments_orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES orders ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE TABLE review_helpful_votes (
        "Id" uuid NOT NULL,
        "ReviewId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "IsHelpful" boolean NOT NULL,
        "VotedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_review_helpful_votes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_review_helpful_votes_reviews_ReviewId" FOREIGN KEY ("ReviewId") REFERENCES reviews ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_cart_items_cart_product ON cart_items ("CartId", "ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_carts_user ON carts ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_categories_parent ON categories ("ParentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_categories_slug ON categories (slug);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_coupons_active_expiry ON coupons ("IsActive", "ExpiresAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_coupons_code ON coupons ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_order_items_order ON order_items ("OrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX "IX_orders_CreatedAt" ON orders ("CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_orders_OrderNumber" ON orders ("OrderNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX "IX_orders_UserId_Status" ON orders ("UserId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_payments_order ON payments ("OrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_payments_stripe_intent ON payments (stripe_payment_intent_id) WHERE stripe_payment_intent_id IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_products_category_active ON products ("CategoryId", is_active, is_deleted);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_products_slug ON products (slug);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_votes_review_user ON review_helpful_votes ("ReviewId", "UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_reviews_product_status ON reviews ("ProductId", "Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_reviews_user_product ON reviews ("UserId", "ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_user_addresses_user_default ON user_addresses ("UserId", "IsDefault");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE UNIQUE INDEX idx_users_email ON users (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    CREATE INDEX idx_users_refresh_token ON users ("RefreshToken");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260517113925_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260517113925_InitialCreate', '8.0.15');
    END IF;
END $EF$;
COMMIT;

