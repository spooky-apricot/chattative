--"Data Source=SQL1002.site4now.net;Initial Catalog=db_ab214d_yy;User Id=db_ab214d_yy_admin;Password=Password1!
--USE master;
--GO
--ALTER DATABASE db_ab214d_zz SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

--drop database db_ab214d_zz;
--go

--use [master];
--go
-- db_ab214d_ee 
-- dbChattative
--drop database if exists db_ab214d_yy;
--go

use db_ab214d_yy;
go

create table tblUser
(
	[ID]			bigint			primary key identity not null,
	[UserName]		nvarchar(18)	not null unique,
	[Password]		nvarchar(20)	not null
);
go

create table tblGuyGal
(
	[ID]			bigint			primary key identity not null,
	[FirstName]		nvarchar(25)	not null unique,
	[LastName]		nvarchar(25)	not null,
	[FullName] AS (FirstName + ' ' + LastName),
	[SkinColour]	nvarchar(16)	not null,
	[Age]			smallint		not null,
	[Score]			int				not null,
	[FkUserID]		bigint			foreign key (FkUserID) references tblUser(ID)
);
go

create table tblMessage
(
	[ID]			bigint			 primary key identity not null,
	[Message]		nvarchar(512)	 not null,
	[SendTime]		datetime default getdate(), 
	[FkSenderID]	bigint			 not null foreign key (FkSenderID) references tblUser(ID),
	[FkReceiverID]  bigint			 not null foreign key (FkReceiverID) references tblUser(ID)
);
go

create table tblCustom -- Account for every custom option in-game. 
(
	[ID]			bigint			primary key identity not null,
	[CustomName]	nvarchar(50)    not null,
	[CustomType]	nvarchar(30)	not null,
	CONSTRAINT UQ_CustomName_CustomType UNIQUE (CustomName, CustomType) 
);
go

create table tblUnlocks -- Reference Custom and User. These are unlocked.
(
	[ID]			bigint		not null primary key identity,	
	[FkUserID]		bigint		not null foreign key references tblUser(ID),
	[FkCustomID]	bigint		not null foreign key references tblCustom(ID)
);
go

create table tblPosts -- If custom is unlocked, they can choose it for their post. -- Save these options after post is posted for later viewing...
(
	[ID]			bigint			not null primary key identity,	
	[FkUserID]		bigint			not null foreign key references tblUser(ID),
	[PostText]		nvarchar(512),
	[BorderWidth]	nvarchar(5)		not null,
	[BorderColour]	nvarchar(25)	not null,
	[BGroundColour] nvarchar(25)	not null,
	[FontStyle]		nvarchar(35),	
	[FontColour]	nvarchar(25),	
	[Padding]		nvarchar(5),
	[PostDate]		DATETIME DEFAULT GETDATE(),
	[IsDefault]		bit				not null default 0 -- IsDefault means it's the current theme, and not an actual post. 	
)

create table tblFriends
(
	[ID]			bigint			not null primary key identity,	
	[FkUserID1]		bigint			not null foreign key references tblUser(ID),
	[FkUserID2]		bigint			not null foreign key references tblUser(ID)
)







/*                                   
use [db_ab214d_ee]

SELECT Message, SendTime
FROM [tblMessage]
WHERE FkReceiverID = 18
ORDER BY SendTime Desc


*/


