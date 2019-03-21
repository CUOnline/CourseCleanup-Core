IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181114235037_Initial')
BEGIN
    CREATE TABLE [CourseSearchQueues] (
        [Id] int NOT NULL IDENTITY,
        [DateCreated] datetime2 NOT NULL,
        [LastUpdated] datetime2 NULL,
        [StartTermId] nvarchar(max) NULL,
        [EndTermId] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [StatusMessage] nvarchar(max) NULL,
        [SubmittedByEmail] nvarchar(max) NULL,
        CONSTRAINT [PK_CourseSearchQueues] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181114235037_Initial')
BEGIN
    CREATE TABLE [UnusedCourses] (
        [Id] int NOT NULL IDENTITY,
        [DateCreated] datetime2 NOT NULL,
        [LastUpdated] datetime2 NULL,
        [CourseId] nvarchar(max) NULL,
        [CourseName] nvarchar(max) NULL,
        [CourseSISID] nvarchar(max) NULL,
        [CourseCode] nvarchar(max) NULL,
        [TermId] nvarchar(max) NULL,
        [Term] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [CourseSearchQueueId] int NOT NULL,
        CONSTRAINT [PK_UnusedCourses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UnusedCourses_CourseSearchQueues_CourseSearchQueueId] FOREIGN KEY ([CourseSearchQueueId]) REFERENCES [CourseSearchQueues] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181114235037_Initial')
BEGIN
    CREATE INDEX [IX_UnusedCourses_CourseSearchQueueId] ON [UnusedCourses] ([CourseSearchQueueId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181114235037_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20181114235037_Initial', N'2.1.4-rtm-31024');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [UnusedCourses] DROP CONSTRAINT [FK_UnusedCourses_CourseSearchQueues_CourseSearchQueueId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [UnusedCourses] DROP CONSTRAINT [PK_UnusedCourses];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [CourseSearchQueues] DROP CONSTRAINT [PK_CourseSearchQueues];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CourseSearchQueues]') AND [c].[name] = N'EndTermId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CourseSearchQueues] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [CourseSearchQueues] DROP COLUMN [EndTermId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    EXEC sp_rename N'[UnusedCourses]', N'UnusedCourse';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    EXEC sp_rename N'[CourseSearchQueues]', N'CourseSearchQueue';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    EXEC sp_rename N'[UnusedCourse].[IX_UnusedCourses_CourseSearchQueueId]', N'IX_UnusedCourse_CourseSearchQueueId', N'INDEX';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    EXEC sp_rename N'[CourseSearchQueue].[StartTermId]', N'TermList', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [UnusedCourse] ADD CONSTRAINT [PK_UnusedCourse] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [CourseSearchQueue] ADD CONSTRAINT [PK_CourseSearchQueue] PRIMARY KEY ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    ALTER TABLE [UnusedCourse] ADD CONSTRAINT [FK_UnusedCourse_CourseSearchQueue_CourseSearchQueueId] FOREIGN KEY ([CourseSearchQueueId]) REFERENCES [CourseSearchQueue] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20181214165551_nonplural')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20181214165551_nonplural', N'2.1.4-rtm-31024');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190208202734_AccountId')
BEGIN
    ALTER TABLE [UnusedCourse] ADD [AccountId] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190208202734_AccountId')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190208202734_AccountId', N'2.1.4-rtm-31024');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190208204306_AccountIdtoString')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UnusedCourse]') AND [c].[name] = N'AccountId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [UnusedCourse] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [UnusedCourse] ALTER COLUMN [AccountId] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190208204306_AccountIdtoString')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190208204306_AccountIdtoString', N'2.1.4-rtm-31024');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190214183645_AddCourseCanvasId')
BEGIN
    ALTER TABLE [UnusedCourse] ADD [CourseCanvasId] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190214183645_AddCourseCanvasId')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190214183645_AddCourseCanvasId', N'2.1.4-rtm-31024');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190222191806_AddDeleteAllRequestToCourseSearchQueue')
BEGIN
    ALTER TABLE [CourseSearchQueue] ADD [DeleteAllRequested] bit NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190222191806_AddDeleteAllRequestToCourseSearchQueue')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190222191806_AddDeleteAllRequestToCourseSearchQueue', N'2.1.4-rtm-31024');
END;

GO

