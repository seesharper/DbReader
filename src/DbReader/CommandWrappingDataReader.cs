using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DbReader;
public class CommandWrappingDataReader(DbCommand command, DbDataReader reader) : DbDataReader
{

    /// <summary>
    /// Gets the inner <see cref="DbDataReader"/> that this reader wraps.
    /// </summary>
    public DbDataReader InnerReader => reader;


    /// <inheritdoc />
    public override void Close() => reader.Close();

    /// <inheritdoc />
    public override async Task<bool> ReadAsync(CancellationToken cancellationToken) =>
        await reader.ReadAsync(cancellationToken);

    /// <inheritdoc />
    public override bool Read() => reader.Read();

    /// <inheritdoc />
    public override int FieldCount => reader.FieldCount;

    /// <inheritdoc />
    public override object this[int ordinal] => reader[ordinal];

    /// <inheritdoc />
    public override object this[string name] => reader[name];

    /// <inheritdoc />
    public override bool HasRows => reader.HasRows;

    /// <inheritdoc />
    public override bool IsClosed => reader.IsClosed;

    /// <inheritdoc />
    public override int RecordsAffected => reader.RecordsAffected;

    /// <inheritdoc />
    public override int Depth => reader.Depth;

    /// <inheritdoc />
    public override bool GetBoolean(int ordinal) => reader.GetBoolean(ordinal);

    /// <inheritdoc />
    public override byte GetByte(int ordinal) => reader.GetByte(ordinal);

    /// <inheritdoc />
    public override char GetChar(int ordinal) => reader.GetChar(ordinal);

    /// <inheritdoc />
    public override DateTime GetDateTime(int ordinal) => reader.GetDateTime(ordinal);

    /// <inheritdoc />
    public override decimal GetDecimal(int ordinal) => reader.GetDecimal(ordinal);

    /// <inheritdoc />
    public override double GetDouble(int ordinal) => reader.GetDouble(ordinal);

    /// <inheritdoc />
    public override float GetFloat(int ordinal) => reader.GetFloat(ordinal);

    /// <inheritdoc />
    public override Guid GetGuid(int ordinal) => reader.GetGuid(ordinal);

    /// <inheritdoc />
    public override short GetInt16(int ordinal) => reader.GetInt16(ordinal);

    /// <inheritdoc />
    public override int GetInt32(int ordinal) => reader.GetInt32(ordinal);

    /// <inheritdoc />
    public override long GetInt64(int ordinal) => reader.GetInt64(ordinal);

    /// <inheritdoc />
    public override string GetString(int ordinal) => reader.GetString(ordinal);

    /// <inheritdoc />
    public override object GetValue(int ordinal) => reader.GetValue(ordinal);

    /// <inheritdoc />
    public override bool IsDBNull(int ordinal) => reader.IsDBNull(ordinal);

    /// <inheritdoc />
    public override int GetValues(object[] values) => reader.GetValues(values);

    /// <inheritdoc />
    public override string GetName(int ordinal) => reader.GetName(ordinal);

    /// <inheritdoc />
    public override string GetDataTypeName(int ordinal) => reader.GetDataTypeName(ordinal);

    /// <inheritdoc />
    public override Type GetFieldType(int ordinal) => reader.GetFieldType(ordinal);

    /// <inheritdoc />
    public override int GetOrdinal(string name) => reader.GetOrdinal(name);

    /// <inheritdoc />
    public override DataTable GetSchemaTable() => reader.GetSchemaTable();

    /// <inheritdoc />
    public override bool NextResult() => reader.NextResult();

    /// <inheritdoc />
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
        reader.NextResultAsync(cancellationToken);

    /// <inheritdoc />
    public override T GetFieldValue<T>(int ordinal) => reader.GetFieldValue<T>(ordinal);

    /// <inheritdoc />
    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
        reader.GetFieldValueAsync<T>(ordinal, cancellationToken);

    /// <inheritdoc />
    public override IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)reader).GetEnumerator();

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            reader.Dispose();
            command.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        => reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

    /// <inheritdoc />
    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        => reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
}
