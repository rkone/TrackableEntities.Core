﻿using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.Contexts;

namespace TrackableEntities.EF.Core.Tests.Helpers;

public class NorthwindDbContextFixture : IDisposable
{
    private NorthwindDbContext? _context;
    private DbConnection? _connection;
    private DbContextOptions<NorthwindDbContext>? _options;

    public void Initialize(bool useInMemory = true, Action? seedData = null)
    {
        if (useInMemory)
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<NorthwindDbContext>()
                .UseSqlite(_connection)
                .Options;
        }
        else
        {
            _options = new DbContextOptionsBuilder<NorthwindDbContext>()
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NorthwindTest; Integrated Security=True; MultipleActiveResultSets=True")
                .Options;
        }
        _context = new NorthwindDbContext(_options);
        _context.Database.EnsureCreated(); // If login error, manually create NorthwindTest database
        seedData?.Invoke();
    }

    public NorthwindDbContext GetContext()
    {
        if (_context == null)
            throw new InvalidOperationException("You must first call Initialize before getting the context.");
        return _context;
    }

    public void Dispose()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
            _connection.Close();
    }
}
