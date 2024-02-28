create database ecommerce_529
use ecommerce_529

create table [user] (id int primary key identity,first_name varchar(30),
last_name varchar(30),email varchar(50),password varchar(30),
gender varchar(6),mobile bigint,role varchar(10));



create table shipping(id int primary key identity,
userid int foreign key references [user](id),
name varchar(30),address varchar(120),city varchar(30),
pincode int,country varchar(30),mobile bigint,state varchar(40)); 


create table product(id int primary key identity,brand_name varchar(30),
image varchar(max),description varchar(150),
category varchar(40),sub_category varchar(40),color varchar(50),
vendor_id int foreign key references vendor(id),status varchar(10));

create table inventory(id int primary key identity,
product_id int foreign key references product(id),
product_size varchar(30),price decimal(10,2),stock int);

create table cart(id int primary key identity ,
[user_id] int foreign key references [user](id),
product_id int foreign key references product(id),
quantity int,inventory_id int foreign key references inventory(id));

create table wishlist(id int primary key identity ,
[user_id] int foreign key references [user](id),
product_id int foreign key references product(id));


create table orders(id int primary key identity,
[user_id] int foreign key references [user](id),
payment_id int ,amount decimal(10,2),status varchar(30),payment_type varchar(30),
date date); 



create table ordered_items(id int primary key identity,
order_id int foreign key references orders(id),
product_id int foreign key references product(id),Quantity int); 

create table Vendor(id int primary key identity,name varchar(30),mobile bigint,
email varchar(30),password varchar(50),address varchar(60),
city varchar(50),state varchar(30),pincode int,isapproved bit,status varchar(10));




select * from wishlist
select * from cart
select * from orders
select * from ordered_items
select * from shipping

select * from Vendor
select * from [user] 
select * from inventory
select * from product



EXEC sp_rename 'product.name', 'brand_name', 'COLUMN';


alter table vendor add status varchar(10); 

alter table ordered_items add  shipping_id int foreign key references shipping(id); 


truncate table cart

