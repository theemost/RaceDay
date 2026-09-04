create database RaceDay;

use RaceDay;

create table Users(
	userId int identity(1000, 1) primary key,
	firstName varchar(50) not null,
	lastName varchar(50) not null,
	emailAddress varchar(100) not null,
	password varchar(100),
	username varchar(50),
	role varchar(50)
);

