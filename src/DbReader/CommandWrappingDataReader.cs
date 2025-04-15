using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DbReader;
public class CommandWrappingDataReader : DbDataReader
{
    private readonly DbCommand _command;
    private readonly DbDataReader _reader;

    public CommandWrappingDataReader(DbCommand command, DbDataReader reader)
    {
        _command = command;
        _reader = reader;
    }

    /// <inheritdoc />
    public override void Close() => _reader.Close();

    /// <inheritdoc />
    public override async Task<bool> ReadAsync(CancellationToken cancellationToken) =>
        await _reader.ReadAsync(cancellationToken);

    /// <inheritdoc />
    public override bool Read() => _reader.Read();

    /// <inheritdoc />
    public override int FieldCount => _reader.FieldCount;

    /// <inheritdoc />
    public override object this[int ordinal] => _reader[ordinal];

    /// <inheritdoc />
    public override object this[string name] => _reader[name];

    /// <inheritdoc />
    public override bool HasRows => _reader.HasRows;

    /// <inheritdoc />
    public override bool IsClosed => _reader.IsClosed;

    /// <inheritdoc />
    public override int RecordsAffected => _reader.RecordsAffected;

    /// <inheritdoc />
    public override int Depth => _reader.Depth;

    /// <inheritdoc />
    public override bool GetBoolean(int ordinal) => _reader.GetBoolean(ordinal);

    /// <inheritdoc />
    public override byte GetByte(int ordinal) => _reader.GetByte(ordinal);

    /// <inheritdoc />
    public override char GetChar(int ordinal) => _reader.GetChar(ordinal);

    /// <inheritdoc />
    public override DateTime GetDateTime(int ordinal) => _reader.GetDateTime(ordinal);

    /// <inheritdoc />
    public override decimal GetDecimal(int ordinal) => _reader.GetDecimal(ordinal);

    /// <inheritdoc />
    public override double GetDouble(int ordinal) => _reader.GetDouble(ordinal);

    /// <inheritdoc />
    public override float GetFloat(int ordinal) => _reader.GetFloat(ordinal);

    /// <inheritdoc />
    public override Guid GetGuid(int ordinal) => _reader.GetGuid(ordinal);

    /// <inheritdoc />
    public override short GetInt16(int ordinal) => _reader.GetInt16(ordinal);

    /// <inheritdoc />
    public override int GetInt32(int ordinal) => _reader.GetInt32(ordinal);

    /// <inheritdoc />
    public override long GetInt64(int ordinal) => _reader.GetInt64(ordinal);

    /// <inheritdoc />
    public override string GetString(int ordinal) => _reader.GetString(ordinal);

    /// <inheritdoc />
    public override object GetValue(int ordinal) => _reader.GetValue(ordinal);

    /// <inheritdoc />
    public override bool IsDBNull(int ordinal) => _reader.IsDBNull(ordinal);

    /// <inheritdoc />
    public override int GetValues(object[] values) => _reader.GetValues(values);

    /// <inheritdoc />
    public override string GetName(int ordinal) => _reader.GetName(ordinal);

    /// <inheritdoc />
    public override string GetDataTypeName(int ordinal) => _reader.GetDataTypeName(ordinal);

    /// <inheritdoc />
    public override Type GetFieldType(int ordinal) => _reader.GetFieldType(ordinal);

    /// <inheritdoc />
    public override int GetOrdinal(string name) => _reader.GetOrdinal(name);

    /// <inheritdoc />
    public override DataTable GetSchemaTable() => _reader.GetSchemaTable();

    /// <inheritdoc />
    public override bool NextResult() => _reader.NextResult();

    /// <inheritdoc />
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
        _reader.NextResultAsync(cancellationToken);

    /// <inheritdoc />
    public override T GetFieldValue<T>(int ordinal) => _reader.GetFieldValue<T>(ordinal);

    /// <inheritdoc />
    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
        _reader.GetFieldValueAsync<T>(ordinal, cancellationToken);

    /// <inheritdoc />
    public override IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)_reader).GetEnumerator();

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _reader.Dispose();
            _command.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        => _reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

    /// <inheritdoc />
    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        => _reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
}
