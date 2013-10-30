
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Vsla_Region]') AND parent_object_id = OBJECT_ID('[Vsla]'))
alter table [Vsla]  drop constraint FK_Vsla_Region


    if exists (select * from dbo.sysobjects where id = object_id(N'[AdminUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [AdminUser]

    if exists (select * from dbo.sysobjects where id = object_id(N'[DataSubmission]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [DataSubmission]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Vsla]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Vsla]

    if exists (select * from dbo.sysobjects where id = object_id(N'[VslaRegion]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [VslaRegion]

    create table [AdminUser] (
        UserId INT IDENTITY NOT NULL,
       Username NVARCHAR(50) not null,
       Surname NVARCHAR(50) null,
       OtherNames NVARCHAR(65) null,
       SecurityToken NVARCHAR(65) null,
       Password NVARCHAR(65) null,
       ActivationDate DATETIME null,
       ActivationPhoneImei NVARCHAR(255) null,
       EMailAddress NVARCHAR(255) null,
       FaxNo NVARCHAR(255) null,
       PostalAddress NVARCHAR(255) null,
       PostalCode NVARCHAR(255) null,
       MobilePhoneNo1 NVARCHAR(255) null,
       MobilePhoneNo2 NVARCHAR(255) null,
       TelephoneNo1 NVARCHAR(255) null,
       TelephoneNo2 NVARCHAR(255) null,
       Town NVARCHAR(255) null,
       primary key (UserId),
      unique (Username)
    )

    create table [DataSubmission] (
        SubmissionId INT IDENTITY NOT NULL,
       SourceVslaCode NVARCHAR(20) null,
       SourcePhoneImei NVARCHAR(20) null,
       SourceNetworkOperator NVARCHAR(50) null,
       SourceNetworkType NVARCHAR(20) null,
       SubmissionTimestamp DATETIME null,
       Data nvarchar(max) null,
       ProcessedFlag BIT null,
       primary key (SubmissionId)
    )

    create table [Vsla] (
        VslaId INT IDENTITY NOT NULL,
       VslaCode NVARCHAR(255) not null,
       VslaName NVARCHAR(255) not null,
       VslaPhoneMsisdn NVARCHAR(20) null,
       PhysicalAddress NVARCHAR(255) null,
       GpsLocation NVARCHAR(100) null,
       DateRegistered DATETIME null,
       DateLinked DATETIME null,
       RegionId INT null,
       primary key (VslaId),
      unique (VslaCode),
      unique (VslaName)
    )

    create table [VslaRegion] (
        RegionId INT IDENTITY NOT NULL,
       RegionCode NVARCHAR(255) not null,
       RegionName NVARCHAR(255) not null,
       primary key (RegionId),
      unique (RegionCode),
      unique (RegionName)
    )

    alter table [Vsla] 
        add constraint FK_Vsla_Region 
        foreign key (RegionId) 
        references [VslaRegion]
