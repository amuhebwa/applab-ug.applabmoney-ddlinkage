
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Attendance_Meeting]') AND parent_object_id = OBJECT_ID('[Attendance]'))
alter table [Attendance]  drop constraint FK_Attendance_Meeting


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Attendance_Member]') AND parent_object_id = OBJECT_ID('[Attendance]'))
alter table [Attendance]  drop constraint FK_Attendance_Member


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Fine_Meeting_Issued]') AND parent_object_id = OBJECT_ID('[Fine]'))
alter table [Fine]  drop constraint FK_Fine_Meeting_Issued


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Fine_Meeting_Paid]') AND parent_object_id = OBJECT_ID('[Fine]'))
alter table [Fine]  drop constraint FK_Fine_Meeting_Paid


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Fine_Member]') AND parent_object_id = OBJECT_ID('[Fine]'))
alter table [Fine]  drop constraint FK_Fine_Member


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA66C912E75B3F070]') AND parent_object_id = OBJECT_ID('[Fine]'))
alter table [Fine]  drop constraint FKA66C912E75B3F070


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LoanIssue_Meeting]') AND parent_object_id = OBJECT_ID('[LoanIssue]'))
alter table [LoanIssue]  drop constraint FK_LoanIssue_Meeting


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LoanIssue_Member]') AND parent_object_id = OBJECT_ID('[LoanIssue]'))
alter table [LoanIssue]  drop constraint FK_LoanIssue_Member


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LoanRepayment_Meeting]') AND parent_object_id = OBJECT_ID('[LoanRepayment]'))
alter table [LoanRepayment]  drop constraint FK_LoanRepayment_Meeting


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LoanRepayment_Member]') AND parent_object_id = OBJECT_ID('[LoanRepayment]'))
alter table [LoanRepayment]  drop constraint FK_LoanRepayment_Member


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LoanRepayment_LoanIssue]') AND parent_object_id = OBJECT_ID('[LoanRepayment]'))
alter table [LoanRepayment]  drop constraint FK_LoanRepayment_LoanIssue


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Meeting_VslaCycle]') AND parent_object_id = OBJECT_ID('[Meeting]'))
alter table [Meeting]  drop constraint FK_Meeting_VslaCycle


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Member_Vsla]') AND parent_object_id = OBJECT_ID('[Member]'))
alter table [Member]  drop constraint FK_Member_Vsla


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Saving_Meeting]') AND parent_object_id = OBJECT_ID('[Saving]'))
alter table [Saving]  drop constraint FK_Saving_Meeting


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Saving_Member]') AND parent_object_id = OBJECT_ID('[Saving]'))
alter table [Saving]  drop constraint FK_Saving_Member


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_VslaCycle_Vsla]') AND parent_object_id = OBJECT_ID('[VslaCycle]'))
alter table [VslaCycle]  drop constraint FK_VslaCycle_Vsla


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_DdActivation_Vsla]') AND parent_object_id = OBJECT_ID('[VslaDdActivation]'))
alter table [VslaDdActivation]  drop constraint FK_DdActivation_Vsla


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Vsla_Region]') AND parent_object_id = OBJECT_ID('[Vsla]'))
alter table [Vsla]  drop constraint FK_Vsla_Region


    if exists (select * from dbo.sysobjects where id = object_id(N'[AdminUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [AdminUser]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Attendance]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Attendance]

    if exists (select * from dbo.sysobjects where id = object_id(N'[DataSubmission]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [DataSubmission]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Fine]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Fine]

    if exists (select * from dbo.sysobjects where id = object_id(N'[LoanIssue]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [LoanIssue]

    if exists (select * from dbo.sysobjects where id = object_id(N'[LoanRepayment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [LoanRepayment]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Meeting]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Meeting]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Member]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Member]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Saving]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Saving]

    if exists (select * from dbo.sysobjects where id = object_id(N'[VslaCycle]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [VslaCycle]

    if exists (select * from dbo.sysobjects where id = object_id(N'[VslaDdActivation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [VslaDdActivation]

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
       EMailAddress NVARCHAR(50) null,
       FaxNo NVARCHAR(50) null,
       PostalAddress NVARCHAR(50) null,
       PostalCode NVARCHAR(50) null,
       MobilePhoneNo1 NVARCHAR(50) null,
       MobilePhoneNo2 NVARCHAR(50) null,
       TelephoneNo1 NVARCHAR(50) null,
       TelephoneNo2 NVARCHAR(50) null,
       Town NVARCHAR(50) null,
       primary key (UserId),
      unique (Username)
    )

    create table [Attendance] (
        AttendanceId INT IDENTITY NOT NULL,
       AttendanceIdEx INT null,
       Comments NVARCHAR(50) null,
       IsPresent BIT null,
       MeetingId INT null,
       MemberId INT null,
       primary key (AttendanceId)
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

    create table [Fine] (
        FineId INT IDENTITY NOT NULL,
       FineIdEx INT null,
       Amount decimal(18,2) null,
       ExpectedDate DATETIME null,
       IsCleared BIT null,
       DateCleared DATETIME null,
       IssuedInMeetingId INT null,
       PaidInMeetingId INT null,
       MemberId INT null,
       MeetingId INT null,
       primary key (FineId)
    )

    create table [LoanIssue] (
        LoanId INT IDENTITY NOT NULL,
       LoanIdEx INT null,
       LoanNo INT null,
       PrincipalAmount decimal(18,2) null,
       InterestAmount decimal(18,2) null,
       Balance decimal(18,2) null,
       Comments NVARCHAR(50) null,
       DateCleared DATETIME null,
       DateDue DATETIME null,
       IsCleared BIT null,
       IsDefaulted BIT null,
       TotalRepaid decimal(18,2) null,
       IsWrittenOff BIT null,
       MeetingId INT null,
       MemberId INT null,
       primary key (LoanId)
    )

    create table [LoanRepayment] (
        RepaymentId INT IDENTITY NOT NULL,
       RepaymentIdEx INT null,
       Amount decimal(18,2) null,
       BalanceAfter decimal(18,2) null,
       BalanceBefore decimal(18,2) null,
       Comments NVARCHAR(50) null,
       LastDateDue DATETIME null,
       NextDateDue DATETIME null,
       InterestAmount decimal(18,2) null,
       RolloverAmount decimal(18,2) null,
       MeetingId INT null,
       MemberId INT null,
       LoanId INT null,
       primary key (RepaymentId)
    )

    create table [Meeting] (
        MeetingId INT IDENTITY NOT NULL,
       MeetingIdEx INT null,
       CashExpenses decimal(18,2) null,
       CashFines decimal(18,2) null,
       CashFromBank decimal(18,2) null,
       CashFromBox decimal(18,2) null,
       CashSavedBank decimal(18,2) null,
       CashSavedBox decimal(18,2) null,
       CashWelfare decimal(18,2) null,
       DateSent DATETIME null,
       IsCurrent BIT null,
       IsDataSent BIT null,
       MeetingDate DATETIME null,
       CountOfMembersPresent INT null,
       SumOfSavings decimal(18,2) null,
       SumOfLoanIssues decimal(18,2) null,
       SumOfLoanRepayments decimal(18,2) null,
       CycleId INT null,
       primary key (MeetingId)
    )

    create table [Member] (
        MemberId INT IDENTITY NOT NULL,
       MemberIdEx INT null,
       MemberNo INT null,
       CyclesCompleted INT null,
       Surname NVARCHAR(30) not null,
       OtherNames NVARCHAR(50) null,
       Gender NVARCHAR(10) null,
       Occupation NVARCHAR(50) null,
       DateArchived DATETIME null,
       DateOfBirth DATETIME null,
       IsActive BIT null,
       IsArchived BIT null,
       PhoneNo NVARCHAR(20) null,
       VslaId INT null,
       primary key (MemberId)
    )

    create table [Saving] (
        SavingId INT IDENTITY NOT NULL,
       SavingIdEx INT null,
       Amount decimal(18,2) null,
       MeetingId INT null,
       MemberId INT null,
       primary key (SavingId)
    )

    create table [VslaCycle] (
        CycleId INT IDENTITY NOT NULL,
       CycleIdEx INT null,
       DateEnded DATETIME null,
       EndDate DATETIME null,
       CycleCode NVARCHAR(20) null,
       InterestRate decimal(18,2) null,
       IsEnded BIT null,
       MaxShareQuantity INT null,
       MaxStartShare decimal(18,2) null,
       SharedAmount decimal(18,2) null,
       SharePrice decimal(18,2) null,
       StartDate DATETIME null,
       MigratedInterest decimal(18,2) null,
       MigratedFines decimal(18,2) null,
       VslaId INT null,
       primary key (CycleId)
    )

    create table [VslaDdActivation] (
        ActivationId INT IDENTITY NOT NULL,
       ActivationDate DATETIME null,
       IsActive BIT null,
       PassKey NVARCHAR(255) null,
       PhoneImei01 NVARCHAR(255) null,
       PhoneImei02 NVARCHAR(255) null,
       SimImsiNo01 NVARCHAR(255) null,
       SimImsiNo02 NVARCHAR(255) null,
       SimNetworkOperator01 NVARCHAR(255) null,
       SimNetworkOperator02 NVARCHAR(255) null,
       SimSerialNo01 NVARCHAR(255) null,
       SimSerialNo02 NVARCHAR(255) null,
       VslaId INT null,
       primary key (ActivationId)
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

    alter table [Attendance] 
        add constraint FK_Attendance_Meeting 
        foreign key (MeetingId) 
        references [Meeting]

    alter table [Attendance] 
        add constraint FK_Attendance_Member 
        foreign key (MemberId) 
        references [Member]

    alter table [Fine] 
        add constraint FK_Fine_Meeting_Issued 
        foreign key (IssuedInMeetingId) 
        references [Meeting]

    alter table [Fine] 
        add constraint FK_Fine_Meeting_Paid 
        foreign key (PaidInMeetingId) 
        references [Meeting]

    alter table [Fine] 
        add constraint FK_Fine_Member 
        foreign key (MemberId) 
        references [Member]

    alter table [Fine] 
        add constraint FKA66C912E75B3F070 
        foreign key (MeetingId) 
        references [Meeting]

    alter table [LoanIssue] 
        add constraint FK_LoanIssue_Meeting 
        foreign key (MeetingId) 
        references [Meeting]

    alter table [LoanIssue] 
        add constraint FK_LoanIssue_Member 
        foreign key (MemberId) 
        references [Member]

    alter table [LoanRepayment] 
        add constraint FK_LoanRepayment_Meeting 
        foreign key (MeetingId) 
        references [Meeting]

    alter table [LoanRepayment] 
        add constraint FK_LoanRepayment_Member 
        foreign key (MemberId) 
        references [Member]

    alter table [LoanRepayment] 
        add constraint FK_LoanRepayment_LoanIssue 
        foreign key (LoanId) 
        references [LoanIssue]

    alter table [Meeting] 
        add constraint FK_Meeting_VslaCycle 
        foreign key (CycleId) 
        references [VslaCycle]

    alter table [Member] 
        add constraint FK_Member_Vsla 
        foreign key (VslaId) 
        references [Vsla]

    alter table [Saving] 
        add constraint FK_Saving_Meeting 
        foreign key (MeetingId) 
        references [Meeting]

    alter table [Saving] 
        add constraint FK_Saving_Member 
        foreign key (MemberId) 
        references [Member]

    alter table [VslaCycle] 
        add constraint FK_VslaCycle_Vsla 
        foreign key (VslaId) 
        references [Vsla]

    alter table [VslaDdActivation] 
        add constraint FK_DdActivation_Vsla 
        foreign key (VslaId) 
        references [Vsla]

    alter table [Vsla] 
        add constraint FK_Vsla_Region 
        foreign key (RegionId) 
        references [VslaRegion]
