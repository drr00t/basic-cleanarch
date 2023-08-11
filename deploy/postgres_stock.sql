-- Create the schema that we'll use to populate data and watch the effect in the binlog
CREATE SCHEMA IF NOT EXISTS stock;

SET
    search_path TO stock;

-- enable uuidv4
create extension IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS audit
(
    id          UUID NOT NULL PRIMARY KEY,
    is_deleted  BOOLEAN NOT NULL,
    create_at   timestamp with time zone NOT NULL,
    update_at   timestamp with time zone NOT NULL
);

-- Create and populate our products using a single insert with many rows
CREATE TABLE IF NOT EXISTS products
(
    product_id  UUID NOT NULL,
    name        VARCHAR(255) NOT NULL,
    description VARCHAR(512) NOT NULL,
    weight      FLOAT NOT NULL,
    price       FLOAT NOT NULL,
    quantity    INT NOT NULL
)INHERITS (audit);

CREATE INDEX IF NOT EXISTS IDX_STATE_CHANGE ON stock.products (product_id ASC, row_version DESC);

INSERT INTO products
VALUES ('09ed82f6-4469-40ec-b601-943c7c848d6b', false,1,1687144819,1687144819,'09ed82f6-4469-40ec-b601-943c7c848d6b','scooter', 'Small 2-wheel scooter', 3.14,3,1),
       ('72234aa3-a1d6-483f-821a-ec541e045372', false,1,1687144819,1687144819,'72234aa3-a1d6-483f-821a-ec541e045372','car battery', '12V car battery', 8.1, 4,1),
       ('c254e819-7976-4f37-bd86-e2a8a14c3762', false,1,1687144819,1687144819,'c254e819-7976-4f37-bd86-e2a8a14c3762','12-pack drill bits','12-pack of drill bits with sizes ranging from #40 to #3', 0.8,5,1),
       ('5fcb33ee-7592-4e14-8473-69329ada6d81', false,1,1687144819,1687144819,'5fcb33ee-7592-4e14-8473-69329ada6d81','hammer', '12oz carpenter''s hammer', 0.75,6,1),
       ('d4a8d6ad-6680-491a-ac64-d7b48da522b3', false,1,1687144819,1687144819,'d4a8d6ad-6680-491a-ac64-d7b48da522b3','hammer', '14oz carpenter''s hammer', 0.875,7,1),
       ('5db3e6c6-ad06-455c-a537-fb80f66777d3', false,1,1687144819,1687144819,'5db3e6c6-ad06-455c-a537-fb80f66777d3','hammer', '16oz carpenter''s hammer', 1.0,8,1),
       ('fde79f2c-ee82-4f76-a77f-0e0a033475c7', false,1,1687144819,1687144819,'fde79f2c-ee82-4f76-a77f-0e0a033475c7','rocks', 'box of assorted rocks', 5.3,2,1),
       ('441e2178-ef95-4c81-b043-8d5156a9f035', false,1,1687144819,1687144819,'441e2178-ef95-4c81-b043-8d5156a9f035','jacket', 'water resistent black wind breaker', 0.1,6,1),
       ('e33a923b-48a2-45b4-ad32-9a2b436ce1bd', false,1,1687144819,1687144819,'e33a923b-48a2-45b4-ad32-9a2b436ce1bd','spare tire', '24 inch spare tire', 22.2,8,1);


CREATE TABLE IF NOT EXISTS stock.products_outbox
(
    id uuid NOT NULL,
    aggregate_id uuid NOT NULL,
    aggregation_type character varying(255) NOT NULL,
    event_type character varying(255) NOT NULL,
    event_data jsonb NOT NULL,
    event_time timestamp with time zone NOT NULL,
    CONSTRAINT products_outbox_pkey PRIMARY KEY (id)
);

