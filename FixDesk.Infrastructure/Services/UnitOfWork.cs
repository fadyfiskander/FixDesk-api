
using FixDesk.Application.Common.Interfaces;
using FixDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Services;

internal sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public int SaveChanges()
    {
        ThrowIfDisposed();

        try
        {
            return _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw CreateConcurrencyException(ex);
        }
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw CreateConcurrencyException(ex);
        }
    }

    public void BeginTransaction()
    {
        ThrowIfDisposed();

        if (_transaction is not null)
            return;

        _transaction = _context.Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_transaction is not null)
            return;

        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }

    public void CommitTransaction()
    {
        ThrowIfDisposed();

        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            transaction.Commit();
        }
        finally
        {
            _transaction = null;
            transaction.Dispose();
        }
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            _transaction = null;
            await transaction.DisposeAsync();
        }
    }

    public void RollbackTransaction()
    {
        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            transaction.Rollback();
        }
        finally
        {
            _transaction = null;
            transaction.Dispose();
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction = null;
            await transaction.DisposeAsync();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        var transaction = _transaction;
        _transaction = null;

        transaction?.Dispose();

        // DbContext is owned by the DI container.
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static InvalidOperationException CreateConcurrencyException(
        DbUpdateConcurrencyException exception)
    {
        return new InvalidOperationException(
            "The record was modified by another operation while this request was running. Please retry the operation.",
            exception);
    }
}
